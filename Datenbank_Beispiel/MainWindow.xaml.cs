using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Datenbank_Beispiel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
   
        private string _dblogin;
        public string DBLogin
        {
            get { return _dblogin; }
            set
            {
                if (_dblogin != value)
                {
                    _dblogin = value;
                    OnPropertyChanged(nameof(DBLogin));
                }
            }
        }

        private string _dbpassword;
        public string DBPassword
        {
            get { return _dbpassword; }
            set
            {
                if (_dbpassword != value)
                {
                    _dbpassword = value;
                    OnPropertyChanged(nameof(DBPassword));
                }
            }
        }

        private DBConnector _dbConnector;

        public MainWindow()
        {             
            DBLogin = "root";
            DBPassword = "";
            InitializeComponent();
            myGrid.DataContext = this;

            _dbConnector = null;
            disableButtons();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_dbConnector == null)
            {
                _dbConnector = OpenConnection(_dblogin, _dbpassword);
                if (_dbConnector.TestConnetion())
                {
                    enableButtons();
                }
                else
                {
                    closeConection();
                    disableButtons();
                    _dbConnector = null;
                }
            }
            else
            {
                closeConection();
                disableButtons();
                _dbConnector = null;
            }
        }

        private void enableButtons()
        {
            DBButton.Content = "Trennen";
            AnzeigeButton.IsEnabled = true;
            EinfuegenButton.IsEnabled = true;
            LoeschenButton.IsEnabled = true;
            DBStatus.Background = Brushes.Green;
            DBStatus.Content = "Verbunden";
        }
        private void disableButtons()
        {
            DBButton.Content = "Verbinden";
            AnzeigeButton.IsEnabled = false;
            EinfuegenButton.IsEnabled = false;
            LoeschenButton.IsEnabled = false;
            DBStatus.Background = Brushes.Red;
            DBStatus.Content = "Nicht verbunden";
        }

        private DBConnector OpenConnection(string dblogin, string dbpassword)
        {
            return new DBConnector(_dblogin, _dbpassword);
        }

        private void closeConection()
        {
            if (_dbConnector != null)
            {
                _dbConnector.Dispose();
                _dbConnector = null;
            }
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            InsertAddressDataWindow insertAddressDataWindow = new InsertAddressDataWindow(_dbConnector);
            insertAddressDataWindow.ShowDialog();
            insertAddressDataWindow.Close();
        }

        private void AnzeigeButton_Click(object sender, RoutedEventArgs e)
        {
            ListAddressDataWindow listAddressDataWindow = new ListAddressDataWindow(_dbConnector);
            listAddressDataWindow.ShowDialog();
            listAddressDataWindow.Close();
        }

        private void LoeschenButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteAddressDataWindow listAddressDataWindow = new DeleteAddressDataWindow(_dbConnector);
            listAddressDataWindow.ShowDialog();
            listAddressDataWindow.Close();
        }
    }
}
