using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;

namespace DigimonDB.Models
{
    public static class WebFunctions
    {
        public const string _baseUrl = "https://en.digimoncard.com";

        internal static HtmlDocument? ReadHtml(string url)
        {
            HtmlDocument? _result = new HtmlDocument();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string html = client.GetStringAsync(url).Result;
                    _result.LoadHtml(html);
                }
            }
            catch (Exception)
            {
                _result = null;
            }

            return _result;
        }


        public static List<CardBox> GetWebCardBoxs()
        {
            var _boxPath = "/cardlist/?search=true&category=508008";
            var _imagePath = "/products/";

            var _result = new List<CardBox>();

            try
            {
                var _boxPage = ReadHtml(_baseUrl + _boxPath);
                if (_boxPage == null)
                    return _result;

                var _cardBoxs = _boxPage.DocumentNode.SelectSingleNode("//li[@class='schCategory']//select").ChildNodes.ToList();

                var _prodImgs = new List<HtmlNode>();

                var _imgPage = ReadHtml(_baseUrl + _imagePath);
                if (_imgPage != null)
                    _prodImgs = _imgPage.DocumentNode.SelectNodes("//div[@class='itembox']").ToList();

                foreach (var btName in _cardBoxs)
                {
                    if (string.IsNullOrWhiteSpace(btName.Attributes[0].Value))
                        continue;

                    var _text = btName.InnerText;
                    var _tag = string.Empty;
                    var _type = string.Empty;

                    if (_text.Contains("["))
                    {
                        _tag = _text.Split('[', ']')[1];
                        _text = _text.Substring(0, _text.IndexOf("["));
                    }
                    else
                        _tag = "PROMO";

                    var _crdBox = new CardBox()
                    {
                        Id = btName.Attributes[0].Value.Trim(),
                        Name = _text.ToUpper().TrimEnd(),
                        Tag = _tag.Trim(),
                    };

                    foreach (var prodImg in _prodImgs)
                    {


                        if (prodImg.SelectSingleNode(".//h4[@class='prodname']").InnerText.Contains(_tag))
                        {
                           var _imgNode = prodImg.SelectSingleNode(".//div[@class='image']//img");

                            _crdBox.ImageUrl = _imgNode.Attributes[0].Value.Replace("..","");
                        }
                    }


                    _crdBox.GenerateType();

                    _result.Add(_crdBox);
                }
            }
            catch (Exception)
            {
               //TODO CREARE IL LOG
            }

            return _result;
        }

        public static List<Card> GetWebCardsByBox(CardBox bt, BackgroundWorker worker, DoWorkEventArgs e)
        {
            var _crdsPath = string.Format("/cardlist/?search=true&category={0}",bt.Id);

            var _result = new List<Card>();

            try
            {
                var _crdsPage = ReadHtml(_baseUrl + _crdsPath);
                if (_crdsPage == null)
                    return _result;

                var _crds = _crdsPage.DocumentNode.SelectNodes("//li[contains(@class, 'image_lists_item')]//div[@class='popup']").ToList();

                foreach (var crd in _crds)
                {
                    var _card = new Card()
                    {
                        BoxCode = bt.Id,
                        BoxTag = bt.Tag
                    };

                    var _cardColor = crd.SelectSingleNode(".//div[contains(@class, 'card_detail')]");
                    var _clss = _cardColor.Attributes[0].Value.Split(" ");
                    var _colors = _clss[1].Replace("card_detail_", "").Split("_");
                    _card.Color1 = _colors[0].ToUpper();

                    if(_colors.Length > 1)
                        _card.Color2 = _colors[1].ToUpper();

                    var _cardInfoHead = crd.SelectSingleNode(".//ul[@class='cardinfo_head']").SelectNodes(".//li").ToList();
                    foreach (var item in _cardInfoHead)
                    {
                        if(item.Attributes.Count > 0)
                        {
                            if (item.Attributes[0].Value.Equals("cardno"))
                                _card.Code = item.InnerText.Trim();
                            else if (item.Attributes[0].Value.Equals("cardtype"))
                                _card.Ctype = item.InnerText.Trim();
                            else if (item.Attributes[0].Value.Equals("cardlv"))
                            {
                                string _string = item.InnerText.Replace("-", "").Trim();
                                if(!string.IsNullOrEmpty(_string))
                                    _string = Regex.Replace(_string, "\\D+", "");

                                _card.Level = _string;
                            }
                            else if (item.Attributes[0].Value.Contains("cardParallel"))
                                _card.IsParallel = true;
                        }
                        else
                        {
                            _card.Rarity = item.InnerText.Trim();
                        }
                    }

                    _card.Name = crd.SelectSingleNode(".//div[@class='card_name']").InnerText;

                    var _cardInfoTop = crd.SelectSingleNode(".//div[@class='cardinfo_top']");

                    _card.Imgs.Add(new GenericImage()
                    {
                        ImgUrl = _cardInfoTop.SelectSingleNode(".//div[@class='card_img']//img").Attributes[0].Value.Replace("..", "")
                    });

                    var _crdTopBody = _cardInfoTop.SelectSingleNode(".//div[@class='cardinfo_top_body']").SelectNodes(".//dl").ToList();
                    foreach (var dl in _crdTopBody)
                    {
                        if (dl.SelectSingleNode(".//dt").InnerText.Equals("Form"))
                        {
                            //RookieRoo
                            var _form = dl.SelectSingleNode(".//dd").InnerText.Trim().Replace("-", "");
                            if (_form.Equals("RookieRoo"))
                                _form = "Rookie";

                            _card.Form = _form;
                        }
                        else if(dl.SelectSingleNode(".//dt").InnerText.Equals("Attribute"))
                            _card.Attribute = dl.SelectSingleNode(".//dd").InnerText.Trim().Replace("-", "");
                        else if (dl.SelectSingleNode(".//dt").InnerText.Equals("Type"))
                        {
                            var _digiType = dl.SelectSingleNode(".//dd").InnerText;

                            if (!_digiType.Contains('-'))
                                _card.DigimonTypes = _digiType.Trim().Split("/").ToList();
                        }
                        else if (dl.SelectSingleNode(".//dt").InnerText.Equals("DP"))
                            _card.DP = dl.SelectSingleNode(".//dd").InnerText.Trim().Replace("-", "");
                        else if (dl.SelectSingleNode(".//dt").InnerText.Equals("Play Cost"))
                            _card.PlayCost = dl.SelectSingleNode(".//dd").InnerText.Trim().Replace("ー","").Replace("-", "");
                        else if (dl.SelectSingleNode(".//dt").InnerText.Equals("Digivolve Cost 1"))
                            _card.DigivolveCost1 = dl.SelectSingleNode(".//dd").InnerText.Trim().Replace("-", "");
                        else if (dl.SelectSingleNode(".//dt").InnerText.Equals("Digivolve Cost 2"))
                            _card.DigivolveCost2 = dl.SelectSingleNode(".//dd").InnerText.Trim().Replace("-", "");
                    }

                    var _cardInfoBottom = crd.SelectSingleNode(".//div[@class='cardinfo_bottom']").SelectNodes(".//dl").ToList();

                    foreach (var dl in _cardInfoBottom)
                    {
                        if (dl.SelectSingleNode(".//dt").InnerText.Equals("Effect"))
                            _card.Effect = dl.SelectSingleNode(".//dd").InnerText.Replace("-", "");
                        else if (dl.SelectSingleNode(".//dt").InnerText.Equals("Digivolve effect"))
                            _card.DigivolveEffect = dl.SelectSingleNode(".//dd").InnerText.Replace("-", "");
                        else if (dl.SelectSingleNode(".//dt").InnerText.Equals("Security effect"))
                            _card.SecurityEffect = dl.SelectSingleNode(".//dd").InnerText.Replace("-", "");
                    }

                    _result.Add(_card);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return _result;
        }

        public static bool DownloadImage(string url, string path, BackgroundWorker worker, DoWorkEventArgs e)
        {
            var _result = false;

            try
            {
                var client = new HttpClient();
                using var s = client.GetStreamAsync(_baseUrl + url);
                using var fs = new FileStream(path, FileMode.OpenOrCreate);
                s.Result.CopyTo(fs);
                _result = true;
            }
            catch (Exception)
            {
                //TODO
                throw;
            }

            return _result;
        }

    }
}
