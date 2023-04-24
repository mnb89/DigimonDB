using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace DigimonDB.Models
{
    public static class DBFunctions
    {

        internal static string GetSqlString(string queryName, params string[] prms)
        {
            var _result = string.Empty;

            try
            {
                if (prms!= null && prms.Length > 0)
                    _result = string.Format(File.ReadAllText(queryName + ".sql"), prms);
                else
                    _result = File.ReadAllText(queryName + ".sql");
            }
            catch (Exception)
            {
                //TODO LOG
                throw;
            }
               
            return _result;
        }

        public static SQLiteConnection? CreateConnection()
        {
            SQLiteConnection sqlite_conn;

            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source= DTCGDB.sqlite; Version=3;");

            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception)
            {
                //TODO LOG
                if (sqlite_conn.State == ConnectionState.Open)
                    sqlite_conn.Close();    

                return null;
            }

            return sqlite_conn;
        }

        public static bool CreateTable(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            try
            {
                //TODO
                string _sql = GetSqlString("Query/CreateTables");

                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = _sql;
                sqlite_cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                //TODO LOG
                return false;
            }
        }

        private static List<GenericValue> GetDBEffect(SQLiteConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            var _result = new List<GenericValue>();

            try
            {
                SQLiteCommand _cmd = conn.CreateCommand();
                _cmd.CommandText = GetSqlString("Query/GetEffect");

                SQLiteDataReader _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        _result.Add(new GenericValue()
                        {
                            Id = _reader.GetInt32(0),
                            Name = _reader.GetValue(1).ToString(),
                            Value = _reader.GetValue(2).ToString()
                        });
                    }
                }
                return _result;
            }
            catch (Exception)
            {
                return _result;
            }
        }

        private static bool CheckCardExist(SQLiteConnection conn, string? code)
        {
            var _result = false;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            try
            {
                SQLiteCommand _cmd = conn.CreateCommand();
                _cmd.CommandText = GetSqlString("Query/GetCardWithCode", code);

                SQLiteDataReader _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                    _result = true;
            }
            catch (Exception)
            {
                conn.Close();
            }

            return _result;
        }

        public static List<CardBox> GetDBCardBoxs(SQLiteConnection conn, bool withCard, CLTYPE cltype = CLTYPE.NONE)
        {
            var _result = new List<CardBox>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteDataReader _reader;

            SQLiteCommand _cmd;

            try
            {
                _cmd = conn.CreateCommand();

                if(cltype.Equals(CLTYPE.NONE))
                {
                    _cmd.CommandText = GetSqlString("Query/GetCardBoxs");
                }
                else
                {
                    var _typeId = (int)cltype;
                    _cmd.CommandText = GetSqlString("Query/GetCardBoxsByType", _typeId.ToString());
                }
                
                _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        var _crdBox = new CardBox();
                        _crdBox.Id = _reader.GetValue(0).ToString();
                        _crdBox.Name = _reader.GetValue(1).ToString();
                        _crdBox.Tag = _reader.GetValue(2).ToString();
                        var _typeString =  _reader.GetValue(3).ToString();

                        if (_typeString != null)
                            _crdBox.Type = (CLTYPE) Enum.Parse(typeof(CLTYPE), _typeString);

                        _crdBox.ImageUrl = _reader.GetValue(4).ToString();

                        _result.Add( _crdBox );
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {
                //TODO LOG
                conn.Close();
            }

            return _result;
        }

        private static List<GenericValue> GetDBAttributes(SQLiteConnection conn)
        {
            var _result = new List<GenericValue>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteDataReader _reader;
            SQLiteCommand _cmd;

            try
            {
                _cmd = conn.CreateCommand();
                _cmd.CommandText = GetSqlString("Query/GetAttributes");
                

                _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        long.TryParse(_reader.GetValue(0).ToString(), out long _id);
                        
                        _result.Add(new GenericValue()
                        {
                            Id = _id,
                            Name = _reader.GetValue(1).ToString()
                        });
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {
                //TODO LOG
                conn.Close();
            }

            return _result;
        }

        private static List<GenericValue> GetDBColors(SQLiteConnection conn)
        {
            var _result = new List<GenericValue>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteDataReader _reader;
            SQLiteCommand _cmd;

            try
            {
                _cmd = conn.CreateCommand();
                _cmd.CommandText = GetSqlString("Query/GetColors");


                _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        long.TryParse(_reader.GetValue(0).ToString(), out long _id);

                        _result.Add(new GenericValue()
                        {
                            Id = _id,
                            Name = _reader.GetValue(1).ToString()
                        });
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {
                //TODO LOG
                conn.Close();
            }

            return _result;
        }

        private static List<GenericValue> GetDBForms(SQLiteConnection conn)
        {
            var _result = new List<GenericValue>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteDataReader _reader;
            SQLiteCommand _cmd;

            try
            {
                _cmd = conn.CreateCommand();
                _cmd.CommandText = GetSqlString("Query/GetForms");


                _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        long.TryParse(_reader.GetValue(0).ToString(), out long _id);

                        _result.Add(new GenericValue()
                        {
                            Id = _id,
                            Name = _reader.GetValue(1).ToString()
                        });
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {
                //TODO LOG
                conn.Close();
            }

            return _result;
        }

        private static List<GenericValue> GetDBDigimonTypes(SQLiteConnection conn)
        {
            var _result = new List<GenericValue>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteDataReader _reader;
            SQLiteCommand _cmd;

            try
            {
                _cmd = conn.CreateCommand();
                _cmd.CommandText = GetSqlString("Query/GetDigiTypes");


                _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        long.TryParse(_reader.GetValue(0).ToString(), out long _id);

                        _result.Add(new GenericValue()
                        {
                            Id = _id,
                            Name = _reader.GetValue(1).ToString()
                        });
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {
                //TODO LOG
                conn.Close();
            }

            return _result;
        }

        private static List<GenericValue> GetDBCardTypes(SQLiteConnection conn)
        {
            var _result = new List<GenericValue>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteDataReader _reader;
            SQLiteCommand _cmd;

            try
            {
                _cmd = conn.CreateCommand();
                _cmd.CommandText = GetSqlString("Query/GetCardTypes");


                _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        long.TryParse(_reader.GetValue(0).ToString(), out long _id);

                        _result.Add(new GenericValue()
                        {
                            Id = _id,
                            Name = _reader.GetValue(1).ToString()
                        });
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {
                //TODO LOG
                conn.Close();
            }

            return _result;
        }

        public static void WriteCardBoxs(SQLiteConnection conn, List<CardBox> nCrdBoxs)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand _cmd = conn.CreateCommand();

            List<string> _nSqls = new List<string>(), _uSqls = new List<string>();

            WriteDBBoxTypesIfNotExist(conn);

            foreach (var bt in nCrdBoxs)
            {
               var _typeId = (int) bt.Type;

               var _values = new string[]
                {
                    bt.Id ?? string.Empty,
                    bt.Name ?? string.Empty,
                    bt.Tag ?? string.Empty,
                    _typeId.ToString(),
                    bt.ImageUrl ?? string.Empty,
                };

                if(bt.Status.Equals(STATUS.NEW))
                    _nSqls.Add(GetSqlString("Query/AddCardBoxR", _values));
                else
                    _uSqls.Add(GetSqlString("Query/UpdCardBoxR", _values));
            }

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            if (_nSqls.Count > 0)
                _cmd.CommandText += string.Format(GetSqlString("Query/AddCardBoxH"), string.Join(",", _nSqls));
            
            if(_uSqls.Count > 0)
                _cmd.CommandText += string.Join("", _uSqls);

            _cmd.ExecuteNonQuery();
            conn.Close();
        }

        private static void WriteDBBoxTypesIfNotExist(SQLiteConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteDataReader _reader;
            SQLiteCommand _cmd = conn.CreateCommand();

            _cmd.CommandText = GetSqlString("Query/GetBoxTypes");

            var _dbBTypes = new List<GenericValue>();

            _reader = _cmd.ExecuteReader();

            if (_reader.HasRows)
            {
                while (_reader.Read())
                {
                    long.TryParse(_reader.GetValue(0).ToString(), out long _id);

                    _dbBTypes.Add(new GenericValue()
                    {
                        Id = _id,
                        Name = _reader.GetValue(1).ToString(),
                        Value = _reader.GetValue(2).ToString(),
                    });
                }
            }

            conn.Close();

            IList eList = Enum.GetValues(typeof(CLTYPE));

            if(_dbBTypes.Count < eList.Count)
            {
                var _sqls = new List<string>();

                for (int i = 0; i < eList.Count; i++)
                {

                    CLTYPE value = (CLTYPE) eList[i];
                    var _id = (int)value;

                    var _typeString = "null";

                    if (value.Equals(CLTYPE.BOOSTER))
                        _typeString = "'BT'";
                    else if (value.Equals(CLTYPE.THEME_BOOSTER))
                        _typeString = "'EX'";
                    else if (value.Equals(CLTYPE.REBOOT_BOOSTER))
                        _typeString = "'RB'";
                    else if (value.Equals(CLTYPE.STARTER_DECK))
                        _typeString = "'ST'";
                    else if (value.Equals(CLTYPE.ADVANCE_DECK))
                        _typeString = "'ST'";

                    var _values = new string[]
                    {
                        _id.ToString(),
                        value.ToString(),
                        _typeString
                    };

                    _sqls.Add(GetSqlString("Query/AddBoxType",_values));

                }

                if (_sqls.Count > 0)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    _cmd = conn.CreateCommand();
                    _cmd.CommandText =  string.Join(" ", _sqls);
                    _cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void WriteCards(SQLiteConnection conn, List<Card> wCards, BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand _cmd = conn.CreateCommand();

            try
            {
                var _oEffect = GetDBEffect(conn);
                var _effectId = _oEffect.Count > 0 ? _oEffect.Last().Id+1 : 0;

                var _nCrdType = wCards.Select(crd => crd.Ctype).Distinct().ToList();
                var _oCrdType = GetDBCardTypes(conn);
                if (_nCrdType.Count > _oCrdType.Count)
                    _oCrdType = WriteCardTypes(conn, _nCrdType);

                var _nDigiType = new List<string>();
                wCards.ForEach(crd => {
                    crd.DigimonTypes.ForEach(tp => _nDigiType.Add(tp));
                });

                _nDigiType = _nDigiType.Distinct().ToList();
                var _oDigiType = GetDBDigimonTypes(conn);
                _oDigiType = WriteDigimonTypes(conn, _nDigiType, _oDigiType);

                var _nForm = wCards.Where(crd => !string.IsNullOrEmpty(crd.Form)).Select(crd => crd.Form).Distinct().ToList();
                var _oForm = GetDBForms(conn);
                _oForm = WriteForms(conn, _nForm, _oForm);

                var _nAtb = wCards.Where(crd => !string.IsNullOrEmpty(crd.Attribute)).Select(crd => crd.Attribute).Distinct().ToList();
                var _oAtb = GetDBAttributes(conn);
                _oAtb = WriteAttributes(conn, _nAtb, _oAtb);

                var _nColor = wCards.Select(crd => crd.Color1).Distinct().ToList();
                var _oColor = GetDBColors(conn);
                if (_nColor.Count > _oColor.Count)
                    _oColor = WriteColors(conn, _nColor);

                List<string> _nSqls = new List<string>();
                List<string> _imgSqls = new List<string>();
                List<string> _effectSqls = new List<string>();

                foreach (var card in wCards)
                {
                    bool _oCardExist = CheckCardExist(conn, card.Code);

                    if (card.IsParallel || _oCardExist)
                    {
                        foreach (var img in card.Imgs)
                        {
                            var _imgValues = new string[]
                            {
                                card.IsParallel ? "Y":"N",
                                card.Rarity,
                                card.BoxCode,
                                card.Code,
                                img.ImgUrl
                            };

                            _imgSqls.Add(GetSqlString("Query/AddImageR",_imgValues));
                        }

                        continue;
                    }

                    var _crdType = _oCrdType.Where(tp => tp.Name.Equals(card.Ctype)).FirstOrDefault();
                    var _color1 = _oColor.Where(cl => cl.Name.Equals(card.Color1)).FirstOrDefault();
                    var _color2 = _oColor.Where(cl => cl.Name.Equals(card.Color2)).FirstOrDefault();
                    var _form = _oForm.Where(cl => cl.Name.Equals(card.Form)).FirstOrDefault();
                    var _atb = _oAtb.Where(cl => cl.Name.Equals(card.Attribute)).FirstOrDefault();

                    var _digiTypeIds = new List<string>();
                    card.DigimonTypes.ForEach(dg => {

                        _oDigiType.ForEach(oDg => {
                            if (oDg.Name.Equals(dg))
                                _digiTypeIds.Add(oDg.Id.ToString());
                        });
                    });

                    var _digiCost1 = new string[0];
                    if (string.IsNullOrWhiteSpace(card.DigivolveCost1))
                    {
                        var _string = card.DigivolveCost1;
                        _string = Regex.Replace(_string, "\\D+", ",");

                        _digiCost1 = _string.Split(",");
                    }

                    var _digiCost2 = new string[0];
                    if (string.IsNullOrWhiteSpace(card.DigivolveCost2))
                    {
                        var _string = card.DigivolveCost2;
                        _string = Regex.Replace(_string, "\\D+", ",");

                        _digiCost2 = _string.Split(",");
                    }

                    string _efctId = string.Empty, _digiEfctId = string.Empty, _secEfctId = string.Empty;

                    if (!string.IsNullOrWhiteSpace(card.Effect))
                    {
                        var _efct = card.Effect;
                        var _efctTp = EFTYPE.SIMPLE.ToString();
                        var _exist = _oEffect.Where(efct => efct.Name.Equals(_efct) && efct.Value.Equals(_efctTp)).FirstOrDefault();

                        if(_exist != null)
                            _efctId = _exist.Id.ToString();
                        else
                        {
                            _efctId = _effectId.ToString();

                            var _efctValues = new string[]
                            {
                                _efctId,
                                _efct.Replace("'", "''"),
                                _efctTp
                            };

                            _effectSqls.Add(GetSqlString("Query/AddEffect", _efctValues));
                            _effectId++;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(card.DigivolveEffect))
                    {
                        var _efct = card.DigivolveEffect;
                        var _efctTp = EFTYPE.DIGIEVOLVE.ToString();
                        var _exist = _oEffect.Where(efct => efct.Name.Equals(_efct) && efct.Value.Equals(_efctTp)).FirstOrDefault();

                        if (_exist != null)
                            _digiEfctId = _exist.Id.ToString();
                        else
                        {
                            _digiEfctId = _effectId.ToString();
                            var _efctValues = new string[]
                            {
                                _digiEfctId,
                                _efct.Replace("'", "''"),
                                _efctTp
                            };

                            _effectSqls.Add(GetSqlString("Query/AddEffect", _efctValues));
                            _effectId++;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(card.SecurityEffect))
                    {
                        var _efct = card.SecurityEffect;
                        var _efctTp = EFTYPE.SECURITY.ToString();
                        var _exist = _oEffect.Where(efct => efct.Name.Equals(_efct) && efct.Value.Equals(_efctTp)).FirstOrDefault();

                        if (_exist != null)
                            _secEfctId = _exist.Id.ToString();
                        else
                        {
                            _secEfctId = _effectId.ToString();
                            var _efctValues = new string[]
                            {
                                _secEfctId,
                                _efct.Replace("'", "''"),
                                _efctTp
                            };

                            _effectSqls.Add(GetSqlString("Query/AddEffect", _efctValues));
                            _effectId++;
                        }
                    }

                    if (_effectSqls.Count > 0)
                    {
                        try
                        {
                            if (conn.State == ConnectionState.Closed)
                                conn.Open();

                            _cmd.CommandText = string.Join(" ", _effectSqls);
                            _cmd.ExecuteNonQuery();

                            _effectSqls.Clear();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }

                    var _values = new List<string>()
                    {
                        string.IsNullOrWhiteSpace(card.Code) ? string.Empty : card.Code,
                        string.IsNullOrWhiteSpace(card.Name) ? string.Empty : card.Name.Replace("'","''"),
                        string.IsNullOrWhiteSpace(card.Level) ? "null" : card.Level,
                        _crdType != null ? _crdType.Id.ToString():"null",
                        _color1 != null ?_color1.Id.ToString():"null",
                        _color2 != null ?_color2.Id.ToString():"null",
                        _form != null ?_form.Id.ToString():"null",
                        _atb != null ?_atb.Id.ToString():"null"
                    };

                    if (_digiTypeIds.Count == 0)
                        _values.AddRange(new List<string>()
                    {
                        "null","null","null","null","null"
                    });
                    else if (_digiTypeIds.Count == 1)
                        _values.AddRange(new List<string>()
                    {
                        _digiTypeIds[0], "null", "null", "null", "null"
                    });
                    else if (_digiTypeIds.Count == 2)
                        _values.AddRange(new List<string>()
                    {
                        _digiTypeIds[0], _digiTypeIds[1], "null", "null", "null"
                    });
                    else if (_digiTypeIds.Count == 3)
                        _values.AddRange(new List<string>()
                    {
                        _digiTypeIds[0], _digiTypeIds[1], _digiTypeIds[2], "null", "null"
                    });
                    else if (_digiTypeIds.Count == 4)
                        _values.AddRange(new List<string>()
                    {
                        _digiTypeIds[0], _digiTypeIds[1], _digiTypeIds[2], _digiTypeIds[3], "null"
                    });
                    else if (_digiTypeIds.Count == 5)
                        _values.AddRange(new List<string>()
                    {
                       _digiTypeIds[0], _digiTypeIds[1], _digiTypeIds[2], _digiTypeIds[3], _digiTypeIds[4]
                    });


                    _values.AddRange(new List<string>()
                    {
                        string.IsNullOrWhiteSpace(card.BoxCode) ? "null" : card.BoxCode,
                        string.IsNullOrWhiteSpace(card.DP) ? "null" : card.DP,
                        string.IsNullOrWhiteSpace(card.PlayCost) ? "null" : card.PlayCost,
                        _digiCost1.Length > 0 ? string.IsNullOrWhiteSpace(_digiCost1[0]) ? "null" : _digiCost1[0]: "null",
                        _digiCost2.Length > 0 ? string.IsNullOrWhiteSpace(_digiCost2[0]) ? "null" : _digiCost2[0] :"null",
                        _digiCost1.Length > 1 ? string.IsNullOrWhiteSpace(_digiCost1[1]) ? "null" : _digiCost1[1] : "null",
                        _digiCost2.Length > 1 ? string.IsNullOrWhiteSpace(_digiCost2[1]) ? "null" : _digiCost2[1] :"null",
                        string.IsNullOrWhiteSpace(_efctId) ? "null": _efctId,
                        string.IsNullOrWhiteSpace(_digiEfctId) ? "null": _digiEfctId,
                        string.IsNullOrWhiteSpace(_secEfctId) ? "null": _secEfctId
                    });

                    _nSqls.Add(GetSqlString("Query/AddCardR", _values.ToArray()));

                    foreach (var img in card.Imgs)
                    {
                        var _imgValues = new string[]
                        {
                            card.IsParallel ? "Y":"N",
                            card.Rarity,
                            card.BoxCode,
                            card.Code,
                            img.ImgUrl
                        };

                        _imgSqls.Add(GetSqlString("Query/AddImageR", _imgValues));
                    }
                }

                if (_nSqls.Count > 0)
                {
                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        _cmd.CommandText = string.Format(GetSqlString("Query/AddCardH"), string.Join(",", _nSqls));
                        _cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                  
                }

                if (_imgSqls.Count > 0)
                {
                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        _cmd.CommandText = string.Format(GetSqlString("Query/AddImageH"), string.Join(",", _imgSqls));
                        _cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                   
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                //todo log
                conn.Close();
            }
        }

        private static List<GenericValue> WriteColors(SQLiteConnection conn, List<string?> nColor)
        {
            var _sqls = new List<string>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand _cmd = conn.CreateCommand();
            var _result = new List<GenericValue>();

            var _idx = 1;
            foreach (var col in nColor)
            {
                if (string.IsNullOrEmpty(col))
                    continue;

                var _values = new string[]
                {
                    _idx.ToString(),
                    col.Trim()
                };

                _sqls.Add(GetSqlString("Query/AddValueR", _values));
                _result.Add(new GenericValue()
                {
                    Id = _idx,
                    Name = col.Trim()
                });

                _idx++;
            }

            if (_sqls.Count > 0)
            {
                _cmd.CommandText = string.Format(GetSqlString("Query/AddColorH"), string.Join(",", _sqls));
                _cmd.ExecuteNonQuery();
                conn.Close();
            }

            return _result;
        }

        private static List<GenericValue> WriteAttributes(SQLiteConnection conn, List<string?> nAtb, List<GenericValue> oAtb)
        {
            var _sqls = new List<string>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand _cmd = conn.CreateCommand();
            var _result = oAtb;

            var _exists = oAtb.Select(atb => atb.Name).ToList();

            var _idx = oAtb.Count;
            foreach (var atb in nAtb)
            {
                if (string.IsNullOrEmpty(atb) || _exists.Contains(atb))
                    continue;

                var _values = new string[]
                {
                    _idx.ToString(),
                    atb
                };

                _sqls.Add(GetSqlString("Query/AddValueR", _values));
                _result.Add(new GenericValue()
                {
                    Id = _idx,
                    Name = atb
                });

                _idx++;
            }

            if (_sqls.Count > 0)
            {
                _cmd.CommandText = string.Format(GetSqlString("Query/AddAttributeH"), string.Join(",", _sqls));
                _cmd.ExecuteNonQuery();
                conn.Close();
            }

            return _result;
        }

        private static List<GenericValue> WriteForms(SQLiteConnection conn, List<string?> nForm, List<GenericValue> oForm)
        {
            var _sqls = new List<string>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand _cmd = conn.CreateCommand();
            var _result = oForm;

            var _exists = oForm.Select(atb => atb.Name).ToList();

            var _idx = oForm.Count;
            foreach (var form in nForm)
            {
                if (string.IsNullOrEmpty(form) || _exists.Contains(form))
                    continue;

                var _values = new string[]
                {
                    _idx.ToString(),
                    form
                };

                _sqls.Add(GetSqlString("Query/AddValueR", _values));
                _result.Add(new GenericValue()
                {
                    Id = _idx,
                    Name = form
                });

                _idx++;
            }

            if (_sqls.Count > 0)
            {
                _cmd.CommandText = string.Format(GetSqlString("Query/AddFormH"), string.Join(",", _sqls));
                _cmd.ExecuteNonQuery();
                conn.Close();
            }

            return _result;
        }

        private static List<GenericValue> WriteDigimonTypes(SQLiteConnection conn, List<string?> nDigiType, List<GenericValue> oDigiType)
        {
            var _sqls = new List<string>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand _cmd = conn.CreateCommand();
            var _result = oDigiType;

            var _exists = oDigiType.Select(atb => atb.Name).ToList();

            var _idx = oDigiType.Count;
            foreach (var dtype in nDigiType)
            {
                if (string.IsNullOrEmpty(dtype) || _exists.Contains(dtype))
                    continue;

                var _values = new string[]
                {
                    _idx.ToString(),
                    dtype.Trim()
                };

                _sqls.Add(GetSqlString("Query/AddValueR", _values));
                _result.Add(new GenericValue()
                {
                    Id = _idx,
                    Name = dtype.Trim()
                });

                _idx++;
            }

            if (_sqls.Count > 0)
            {
                _cmd.CommandText = string.Format(GetSqlString("Query/AddDigiTypeH"), string.Join(",", _sqls));
                _cmd.ExecuteNonQuery();
                conn.Close();
            }

            return _result;
        }

        private static List<GenericValue> WriteCardTypes(SQLiteConnection conn, List<string?> nCrdType)
        {
            var _sqls = new List<string>();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand _cmd = conn.CreateCommand();
            var _result = new List<GenericValue>();

            var _idx = 1;
            foreach (var type in nCrdType)
            {
                if (string.IsNullOrEmpty(type))
                    continue;

                var _values = new string[]
                {
                    _idx.ToString(),
                    type
                };

                _sqls.Add(GetSqlString("Query/AddValueR", _values));
                
                _idx++;
            }

            if (_sqls.Count > 0)
            {
                _cmd.CommandText = string.Format(GetSqlString("Query/AddCardTypeH"), string.Join(",", _sqls));
                _cmd.ExecuteNonQuery();
                conn.Close();
            }

            return _result;
        }

        public static List<GenericImage> GetAllBoxImage(SQLiteConnection conn, bool onlyNew, BackgroundWorker worker, DoWorkEventArgs e)
        {
            var _result = new List<GenericImage>();
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            try
            {
                SQLiteDataReader _reader;
                SQLiteCommand _cmd = conn.CreateCommand();

                var _filter = string.Empty;

                if (onlyNew)
                    _filter = "AND \"ImgPath\" IS NULL";

                _cmd.CommandText = GetSqlString("Query/GetAllBoxImages", _filter);
                _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        _result.Add(new GenericImage()
                        {
                            Id = _reader.GetInt32(0),
                            Code = _reader.GetValue(1).ToString(),
                            ImgUrl = _reader.GetValue(2).ToString(),
                            ImgPath = _reader.GetValue(3).ToString()
                        });
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {
                //TODO
                conn.Close();
            }

            return _result;
        }

        public static List<GenericImage> GetAllCardImage(SQLiteConnection conn, bool onlyNew, BackgroundWorker worker, DoWorkEventArgs e)
        {
            var _result = new List<GenericImage>();
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            try
            {
                SQLiteDataReader _reader;
                SQLiteCommand _cmd = conn.CreateCommand();

                var _filter = string.Empty;

                if (onlyNew)
                    _filter = "AND \"ImgPath\" IS NULL";

                _cmd.CommandText = GetSqlString("Query/GetAllCardImages", _filter);

                _reader = _cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        _result.Add(new GenericImage()
                        {
                            Id = _reader.GetInt32(0),
                            IsParallel = _reader.GetValue(1).ToString(),
                            Rarity = _reader.GetValue(2).ToString(),
                            BoxId = _reader.GetInt32(3),
                            Code = _reader.GetValue(4).ToString(),
                            ImgUrl = _reader.GetValue(5).ToString(),
                            ImgPath = _reader.GetValue(6).ToString()
                        });
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {

                conn.Close();
            }

            return _result;

        }

        internal static void UpdateBoxImages(SQLiteConnection conn, List<GenericImage> list, BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand _cmd = conn.CreateCommand();
            var _sqls = new List<string>();

            foreach (var img in list)
            {
                var _values = new string[]
                {
                    img.Id.ToString(),
                    img.ImgPath
                };

                _sqls.Add(GetSqlString("Query/UpdBoxImageR", _values));
            }

            if (_sqls.Count > 0)
            {
                _cmd.CommandText = string.Join(" ", _sqls);
                _cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        internal static void UpdateCardImages(SQLiteConnection conn, List<GenericImage> list, BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand _cmd = conn.CreateCommand();
            var _sqls = new List<string>();

            foreach (var img in list)
            {
                var _values = new string[]
                {
                    img.Id.ToString(),
                    img.ImgPath
                };

                _sqls.Add(GetSqlString("Query/UpdCardImageR", _values));
            }

            if (_sqls.Count > 0)
            {
                _cmd.CommandText = string.Join(" ", _sqls);
                _cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
