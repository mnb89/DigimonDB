using DigimonDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DIgimonDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection? conn;
        private BackgroundWorker backgroundWorker1;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundWorker();

            conn = DBFunctions.CreateConnection();
        }

        // Set up the BackgroundWorker object by 
        // attaching event handlers. 
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;

            backgroundWorker1.DoWork +=
                new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker1_ProgressChanged);
        }

        // This event handler is where the actual,
        // potentially time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender,
            DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            try
            {
                if (conn != null)
                {
                    var _crdBoxs = GetAndCheckNewCardBoxs(conn, worker, e);

                    _crdBoxs = _crdBoxs.OrderBy(x => x.Tag).ToList();

                    var _perc = _crdBoxs.Count > 0 ? (40 / _crdBoxs.Count ) / 2 : 40;
                    var _actPerc = 8;
                    foreach (var bt in _crdBoxs)
                    {
                        try
                        {
                            var _wCards = WebFunctions.GetWebCardsByBox(bt, worker, e);

                            _actPerc += _perc;
                            worker.ReportProgress(_actPerc);

                            DBFunctions.WriteCards(conn, _wCards, worker, e);

                        }
                        catch (Exception)
                        {
                            //TODO
                            continue;
                        }

                        _actPerc += _perc;
                        worker.ReportProgress(_actPerc);
                    }

                    worker.ReportProgress(48); //Aggiornamento carte completato. inizio download immagini...                    

                    InternalFunctions.DownloadImageFile(conn, worker, e);

                    Thread.Sleep(100);

                    worker.ReportProgress(100); //Aggiornamento completato.
                    conn.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void backgroundWorker1_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            var _value = e.ProgressPercentage;

            if (_value == 1)
                StatusLabel.Content = "Lettura online box...";
            else if (_value == 2)
                StatusLabel.Content = "Estrazione nuovi box...";
            else if (_value == 7)
                StatusLabel.Content = "Aggiunta nuovi box...";
            else if (_value == 8)
                StatusLabel.Content = "Aggiunta box completata. Aggiornamento carte...";
            else if (_value == 48)
                StatusLabel.Content = "Aggiornamento carte completato. Inizio lettura immagini box... ";
            else if (_value == 50)
                StatusLabel.Content = "Lettura immagini box completata. Inizio download immagini...";
            else if(_value == 64)
                StatusLabel.Content = "Download immagini box completato.";
            else if (_value == 66)
                StatusLabel.Content = "Inizio lettura immagini carte...";
            else if (_value == 68)
                StatusLabel.Content = "Lettura immagini carte completata. Download immagini...";
            else if (_value == 96)
                StatusLabel.Content = "Download immagini carte completato.";
            else if (_value == 98)
                StatusLabel.Content = "Ultimi accorgimenti...";
            else if (_value == 100)
                StatusLabel.Content = "Aggiornamento completato.";

            ProgBar.Value = _value;
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(100);
            ProgBar.Value = 0;
            GetData.IsEnabled = true;
        }


        public static List<CardBox> GetAndCheckNewCardBoxs(SQLiteConnection conn, BackgroundWorker worker, DoWorkEventArgs e)
        {

            var _wCrdBoxs = WebFunctions.GetWebCardBoxs();

            worker.ReportProgress(1); //Lettura online box...

            Thread.Sleep(100);

            var _dCrdBoxs = DBFunctions.GetDBCardBoxs(conn, false);

            worker.ReportProgress(2); //Estrazione nuovi box...

            Thread.Sleep(100);

            var _dCodes = _dCrdBoxs.Select(bt => bt.Id).ToList();
            var _crdBoxs = new List<CardBox>();

            var _perc = 4 / _wCrdBoxs.Count;
            var _actPerc = 2;
            foreach (var bt in _wCrdBoxs)
            {
                bt.Status = _dCodes.Contains(bt.Id) ? STATUS.UPDATE : STATUS.NEW;

                if (bt.Status.Equals(STATUS.NEW))
                    _crdBoxs.Add(bt);

                _actPerc += _perc;
                worker.ReportProgress(_actPerc);

            }

            worker.ReportProgress(7); //Aggiunta nuovi box...

            if (_crdBoxs.Count > 0)
                DBFunctions.WriteCardBoxs(conn, _crdBoxs);

            worker.ReportProgress(8); //Aggiunta box completata. Aggiornamento carte...

            return _crdBoxs;
        }

        private void GetData_Click(object sender, RoutedEventArgs e)
        {
            GetData.IsEnabled = false;
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
