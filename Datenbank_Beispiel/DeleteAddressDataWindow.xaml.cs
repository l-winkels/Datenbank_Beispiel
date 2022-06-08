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
using System.Windows.Shapes;

namespace Datenbank_Beispiel
{
    /// <summary>
    /// Interaktionslogik für CreateDB.xaml
    /// </summary>
    public partial class DeleteAddressDataWindow : Window
    {
        private DBConnector _dbConnector;

        public DeleteAddressDataWindow(DBConnector dbConnector)
        {
            _dbConnector = dbConnector;
            InitializeComponent();                 
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string result = _dbConnector.DeleteAddressData(IDTextBox.Text);
            StatusTextBox.Text = result;
        }
    }
}
