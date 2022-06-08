using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
namespace Datenbank_Beispiel
{
    public class DBConnector : IDisposable
    {
        private MySqlConnection _mySqlConnection = null;
        public DBConnector(string login, string password) 
        {
            string connectionString = "Server=localhost;Port=3306;Database=BeispielDB;Uid=" + login + ";password=" + password + ";";
            _mySqlConnection = new MySqlConnection(connectionString);        
        }

        public bool TestConnetion()
        {
            try
            {
                _mySqlConnection.Open();
                _mySqlConnection.Close();
                return true;
            }
            catch (Exception)
            {                
                return false;
            }
        }

        public string InsertAddressData(AddressData addressData)
        {
            string ret = "";
            try
            {
                if (NoZipData(addressData.ZipCode, addressData.Country, addressData.City))
                {
                    string sqlCmd1 = "INSERT INTO Zip VALUES (" +
                                "'" + addressData.ZipCode + "', " +
                                "'" + addressData.Country + "'," +
                                "'" + addressData.City + "');";
                
                    using (MySqlCommand sql = new MySqlCommand(sqlCmd1, _mySqlConnection))
                    {
                        _mySqlConnection.Open();
                        int rCode = sql.ExecuteNonQuery();
                        ret += sql.CommandText + Environment.NewLine;
                        ret += "Resultcode: " + rCode.ToString() + Environment.NewLine;
                        _mySqlConnection.Close();
                    }
                  
                }

                string sqlCmd2 = "INSERT INTO AddressData VALUES (" +
                                "NULL, " +
                                "'" + addressData.FirstName + "', " +
                                "'" + addressData.LastName + "', " +
                                "'" + addressData.Street + "', " +
                                "'" + addressData.ZipCode + "', " +
                                "'" + addressData.Country + "');";

                using (MySqlCommand sql = new MySqlCommand(sqlCmd2, _mySqlConnection))
                {
                    _mySqlConnection.Open();
                    int rCode = sql.ExecuteNonQuery();
                    ret += sql.CommandText + Environment.NewLine;
                    ret += "Resultcode: " + rCode.ToString() + Environment.NewLine;
                    _mySqlConnection.Close();
                }
            }
            catch (Exception e)
            {
                ret += e.ToString() + Environment.NewLine;
            }

            return ret;
        }

        public List<AddressData> LoadAddressData()
        {
            List<AddressData> ret = new List<AddressData>();
            string sqlCmd = "SELECT ad.Firstname, ad.Lastname, ad.Street, z.City, z.Country, ad.ZipCode, ad.ID FROM AddressData ad LEFT OUTER JOIN Zip z ON (ad.ZipCode, ad.Country) = (z.ZipCode, z.Country);";
            using (MySqlCommand sql = new MySqlCommand(sqlCmd, _mySqlConnection))
            {
                _mySqlConnection.Open();
                MySqlDataReader result = sql.ExecuteReader();
                while (result.Read())
                {
                    AddressData data = new AddressData(result.GetString(0), result.GetString(1), result.GetString(2), result.GetString(3), result.GetString(4), result.GetString(5), result.GetInt32(6));

                    ret.Add(data);
                }
                _mySqlConnection.Close();
            }

            return ret;
        }

        public AddressData LoadAddressData(string id)
        {
            id = id.Replace('\'', ' '); // ' durch space ersetzen

            AddressData ret = null;
            string sqlCmd = "SELECT ad.Firstname, ad.Lastname, ad.Street, z.City, z.Country, ad.ZipCode, ad.ID FROM AddressData ad LEFT OUTER JOIN Zip z ON (ad.ZipCode, ad.Country) = (z.ZipCode, z.Country) WHERE ad.ID = " + id + ";";
            using (MySqlCommand sql = new MySqlCommand(sqlCmd, _mySqlConnection))
            {
                _mySqlConnection.Open();
                MySqlDataReader result = sql.ExecuteReader();
                if (result.Read())
                {
                    ret = new AddressData(result.GetString(0), result.GetString(1), result.GetString(2), result.GetString(3), result.GetString(4), result.GetString(5), result.GetInt32(6));
                }
                _mySqlConnection.Close();
            }

            return ret;
        }

        private bool NoZipData(string zipCode, string country, string city)
        {
            bool ok = false;

            string sqlCmd = "SELECT ZipCode, Country, City FROM Zip WHERE " +
                             "ZipCode='" + zipCode + "' AND " +
                             "Country='" + country + "';";
            using (MySqlCommand sql = new MySqlCommand(sqlCmd, _mySqlConnection))
            {
                _mySqlConnection.Open();
                MySqlDataReader result = sql.ExecuteReader();
                if (result.Read())
                {
                    // Eintrag für ZipCode + Country ist schon vorhanden
                    ok = false;

                    // Prüfen, ob der Eintrag richtig ist
                    string dbCity = result.GetString(2); // Dritte Spalte holen
                    if (!city.Equals(dbCity))
                    {
                        _mySqlConnection.Close();
                        // Eintrag für ZipCode + Country ist einem anderen Wert für City zugeordnet
                        throw new Exception("Zip: " + zipCode + ", Country: " + country + " ist schon dem Wert " + dbCity + " zugeordnet.");
                    }
                }
                else
                {
                    // ok, kein Eintrag für ZipCode + Country. Eintrag kann angelegt werden.
                    ok = true;
                }
                _mySqlConnection.Close();
            }

            return ok;
        }

        private bool existsZipData(string zipCode, string country)
        {
            bool ok;

            string sqlCmd = "SELECT COUNT(*) FROM AddressData WHERE " +
                             "ZipCode='" + zipCode + "' AND " +
                             "Country='" + country + "';";
            using (MySqlCommand sql = new MySqlCommand(sqlCmd, _mySqlConnection))
            {
                _mySqlConnection.Open();
                Int64 count = (Int64) sql.ExecuteScalar();
                if (count > 0)
                {
                    ok = true;
                }
                else
                {                 
                    ok = false;
                }
                _mySqlConnection.Close();
            }

            return ok;
        }

        public string  DeleteAddressData(string id)
        {
            AddressData addressData = LoadAddressData(id);
            if (addressData == null)
            {
                return "Adresse mit ID " + id + " nicht vorhanden";
            }
            string ret = "";

            id = id.Replace('\'', ' '); // ' durch space ersetzen

            string sqlCmd = "DELETE FROM AddressData WHERE ID = " + id + ";";
            using (MySqlCommand sql = new MySqlCommand(sqlCmd, _mySqlConnection))
            {
                _mySqlConnection.Open();
                int result = sql.ExecuteNonQuery();
                ret += "Gelöschte Adress-Datensätze: " + result.ToString() + Environment.NewLine;
                _mySqlConnection.Close();
            }

            if (!existsZipData(addressData.ZipCode, addressData.Country))
            {
                // Wenn es keine Adressdaten zur PLZ gibt, dann PLZ Eintag löschen.
                string sqlCmd2 = "DELETE FROM Zip WHERE ZipCode = '" + addressData.ZipCode + "' AND Country = '" + addressData.Country + "';";
                using (MySqlCommand sql = new MySqlCommand(sqlCmd2, _mySqlConnection))
                {
                    _mySqlConnection.Open();
                    int result = sql.ExecuteNonQuery();
                    ret += "Gelöschte PLZ-Datensätze: " + result.ToString() + Environment.NewLine;
                    _mySqlConnection.Close();
                }
            }

            return ret;
        }
        public void Dispose()
        {
            if (_mySqlConnection != null)
            {
                if (_mySqlConnection?.State == System.Data.ConnectionState.Open)
                {
                    _mySqlConnection.Close();
                }
            }
        }
    }
}
