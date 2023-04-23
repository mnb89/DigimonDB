using DigimonDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
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
            //backgroundWorker1.DoWork +=
            //    new DoWorkEventHandler(backgroundWorker1_DoWork);
            //backgroundWorker1.RunWorkerCompleted +=
            //    new RunWorkerCompletedEventHandler(
            //backgroundWorker1_RunWorkerCompleted);
            //backgroundWorker1.ProgressChanged +=
            //    new ProgressChangedEventHandler(
            //backgroundWorker1_ProgressChanged);
        }


        private void GetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(conn != null)
                {
                    var _crdBoxs = InternalFunctions.GetAndCheckNewCardBoxs(conn);

                    _crdBoxs = _crdBoxs.OrderBy(x => x.Tag).ToList();

                    InternalFunctions.GetAndCreateCards(conn, ProgBar, _crdBoxs);
                    
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
