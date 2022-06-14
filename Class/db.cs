using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Configuration;
namespace Fusion_PDO.Class
{
    class db
    {
        public SqlConnection conn;
        public string server { set; get; }
        public string database { set; get; }
        public string username { set; get; }
        public string password { set; get; }

        public string decryptedPassword;

        public string connectionString;


        public List<string> myList = new List<string>();

        //temporary connection
        private string _server;
        private string _database;
        private string _username;
        private string _password;
        private string _decryptedPassword;

        public void getData()
        {

            // decryptedPassword = Decrypt(password);
            LoadDataFromFile();
            connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";User ID=" + username + ";Password=" + _decryptedPassword;
            conn = new SqlConnection(connectionString);
        }
        public void LoadDataFromFile()
        {
            try
            {
                string filePath = ConfigurationManager.AppSettings.Get("connString");
                using (var streamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        myList.Add(line);
                    }
                    _server = myList[0].ToString();
                    _database = myList[1].ToString();
                    _username = myList[2].ToString();
                    _password = myList[3].ToString();
                    _decryptedPassword = Decrypt(_password);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("No connection.", e.ToString());
            }
        }

        public string Decrypt(string strToDecrypt)
        {
            string res = string.Empty;
            try
            {
                string result = "";
                string dec = strToDecrypt;
                int i;

                for (i = 0; i < dec.Length - 1; i += 4)
                {
                    res = dec.Substring(i, 4);
                    result += Convert.ToChar(Convert.ToInt32(res, 16) / 114);
                }

                return result;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return res;
        }
    }
}
