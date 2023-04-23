using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigimonDB.Models
{
    public static class InternalFunctions
    {
        public static List<CardBox> GetAndCheckNewCardBoxs(SQLiteConnection conn)
        {
            var _wCrdBoxs = WebFunctions.GetWebCardBoxs();

            var _dCrdBoxs = DBFunctions.GetDBCardBoxs(conn, false);

            var _dCodes = _dCrdBoxs.Select(bt => bt.Id).ToList();
            var _crdBoxs = new List<CardBox>();

            foreach (var bt in _wCrdBoxs)
            {
                bt.Status = _dCodes.Contains(bt.Id) ?  STATUS.UPDATE: STATUS.NEW;
                _crdBoxs.Add(bt);
            }

            if (_crdBoxs.Count > 0)
                DBFunctions.WriteCardBoxs(conn, _crdBoxs);

            return _wCrdBoxs;
        }

        public static void GetAndCreateCards(SQLiteConnection conn,ProgressBar progBar, List<CardBox> crdBoxs)
        {

            foreach (var bt in crdBoxs)
            {
                try
                {
                    var _wCards = WebFunctions.GetWebCardsByBox(bt);

                    DBFunctions.WriteCards(conn, _wCards);

                }
                catch (Exception)
                {
                    //TODO
                    continue;
                }
               
            }

            conn.Close();
        }
    }
}
