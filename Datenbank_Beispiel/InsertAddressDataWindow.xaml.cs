using System.Windows;

namespace Datenbank_Beispiel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class InsertAddressDataWindow : Window
    {
        private DBConnector _dbConnector;
        public InsertAddressDataWindow(DBConnector dbConnector)
        {
            _dbConnector = dbConnector;
            InitializeComponent();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            AddressData addressData = new AddressData(FirstnameTextbox.Text, LastnameTextbox.Text, StreetTextbox.Text, CityTextbox.Text, CountryCombobox.Text, ZipcodeTextbox.Text);
            StatusTextBox.Text += "AdressData: " + addressData.ToString();
            string statusText = _dbConnector.InsertAddressData(addressData);
            StatusTextBox.Text += statusText;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
