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
using Fusion_PDO.Properties;
using System.Reflection;

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

            LoadDataFromFile();
            connectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username + ";Password=" + _decryptedPassword;
            conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                Settings.Default["connString"] = connectionString;
                Settings.Default.Save();
                conn.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show("Cannot establish database connection.\n\n" + ee.Message, "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //Application.Current.Shutdown();
            }
        }
        public void LoadDataFromFile()
        {
            try
            {
                string pathSource = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string filePath = pathSource + @"\Nexas America\db.ini";
                if (File.Exists(filePath))
                {
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
                else
                {
                    MessageBox.Show("Cannot find configuration file.\n\n Setup Database first.", "Fusion", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while getting file.\n\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
