using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigimonDB.Models
{
    public static class InternalFunctions
    {

        public static void DownloadImageFile(SQLiteConnection conn, BackgroundWorker worker, DoWorkEventArgs e)
        {
            var _basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var _list = DBFunctions.GetAllBoxImage(conn, true, worker, e);

            worker.ReportProgress(50); //Lettura immagini box completata. Inizio download immagini...

            var _perc = _list.Count > 0 ? (14 / _list.Count): 14;
            var _actPerc = 50;

            foreach (var item in _list)
            {
                if (!string.IsNullOrWhiteSpace(item.ImgUrl))
                {
                    var _fileExtension = item.ImgUrl.Split(".")[1];

                    if (_fileExtension.Contains('?'))
                        _fileExtension = _fileExtension.Split("?")[0];

                    var _imgPath = string.Format("\\Media\\BT\\{0}.{1}", item.Code, _fileExtension);
                    var _fullPath = _basePath + _imgPath;

                    if (WebFunctions.DownloadImage(item.ImgUrl, _fullPath, worker, e))
                        item.ImgPath = _imgPath;

                    _actPerc += _perc;
                    worker.ReportProgress(_actPerc);
                }
            }

            Thread.Sleep(100); //Download immagini box completato. 

            DBFunctions.UpdateBoxImages(conn, _list, worker, e);

            worker.ReportProgress(66); //Inizio lettura immagini carte...

            Thread.Sleep(100);

            _list = DBFunctions.GetAllCardImage(conn, true, worker, e);

            worker.ReportProgress(68); //Lettura immagini carte completata. Download immagini...

            _perc = _list.Count > 0 ? (28 / _list.Count) : 28;
            _actPerc = 68;

            foreach (var item in _list)
            {
                if (!string.IsNullOrWhiteSpace(item.ImgUrl))
                {
                    var _fileName = item.ImgUrl.Split("/").Last();

                    var _imgPath = string.Format("\\Media\\IMG\\{0}", _fileName);
                    var _fullPath = _basePath + _imgPath;

                    if (WebFunctions.DownloadImage(item.ImgUrl, _fullPath, worker, e))
                        item.ImgPath = _imgPath;

                    _actPerc += _perc;
                    worker.ReportProgress(_actPerc);
                }
            }
            Thread.Sleep(100); //Download immagini carte completato. 

            DBFunctions.UpdateCardImages(conn, _list, worker, e);

            worker.ReportProgress(98); //Ultimi accorgimenti...

        }
    }
}
