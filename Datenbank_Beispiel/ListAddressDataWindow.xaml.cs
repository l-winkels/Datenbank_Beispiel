using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Datenbank_Beispiel
{
    /// <summary>
    /// Interaktionslogik für ListAddressDataWindow.xaml
    /// </summary>
    public partial class ListAddressDataWindow : Window
    {
        private DBConnector _dbConnector;
        public ListAddressDataWindow(DBConnector dbConnector)
        {
            _dbConnector = dbConnector;
            InitializeComponent();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            List<AddressData> data = _dbConnector.LoadAddressData();
            DataGridAddressData.ItemsSource = data;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
