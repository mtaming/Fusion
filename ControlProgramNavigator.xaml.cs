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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
namespace Fusion_PDO
{
    /* Notions 

 Need for Enhancement:

  1. Do not use static values. Database connections parameters are dynamically changing. Always think through if the parameters will be dynamically changing or just static as user will define it. 
  2. Use proper grammar and notifcation content. As much as possible do not use very technical jargons that ordinary users will not understand it. Use instruction or warning messages that users will easily understand what will be their next step. 
  3.For message box follow uniform format MessageBox.Show("The Message", "Title", Button design, MessageBoxImage Icon)
  4. Make sure to close sqlDataReader when opening it for a read. Anticapate if there is no data in the table. You should handle empty data.
  5. Make sure to close database connection after use.
  6. Use proper field name for better readability than the column index number. ex. reader[1].toString()  better for readability reader["MyFieldName"].
                
      */
    public partial class ControlProgramNavigator : UserControl
    {
        public ControlProgramNavigator()
        {
            InitializeComponent();
        }

        string connectionString = null;
        SqlConnection conn;

        public void LoadDB()
        {

            //!!! J Notation: Do not use static values. Database connections parameters are dynamically changing.
            connectionString = "Data Source=DESKTOP-KLRS7LV\\FUSION;Initial Catalog=Fusion_Database;User ID=FusionTester;Password=FusionTester1";
            conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                // MessageBox.Show("Connection Open ! ");
                conn.Close();


            }
            catch (Exception ex)
            {
                //!!! J Notation: Proper grammar and notification.
                ////For message box follow uniform format MessageBox.Show("The Message", "Title", Button design, MessageBoxImage Icon)
                MessageBox.Show("Can not open connection ! " + ex.ToString());
                Application.Current.Shutdown();
            }
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            LoadDB();
            dgView1.ItemsSource = GetDataDG().DefaultView;
            LoadToTextBoxes();
        }

        //ONCLICK FUNCTION OF CELLS IN DATAGRID
        private void dgClick(object sender, MouseButtonEventArgs e)
        {
            txtRich.Document.Blocks.Clear();

            DataRowView row = (DataRowView)dgView1.SelectedItem;
            string condition = row["id"].ToString();

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] WHERE id = " + condition + "  ORDER BY filename ASC", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                //!!! J Notation:  Make sure to close sqlDataReader when opening it for a read. Anticapate if there is no data in the table. You should handle empty data.
                reader.Read();

                txtPath.Text = reader[8].ToString();
                txtReferenceId.Text = reader[1].ToString();
                txtRemoteRequestId.Text = reader[2].ToString();
                txtLastModified.Text = reader[7].ToString();
                string filePath = reader[8].ToString();
                long fileSizeinBytes = GetFileSize(filePath);
                txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                var bytes = File.ReadAllBytes(filePath);
                var text = Encoding.UTF8.GetString(bytes);



                FlowDocument obj = new FlowDocument();

                Paragraph objPar1 = new Paragraph();
                objPar1.Inlines.Add(new Run(text));
                obj.Blocks.Add(objPar1);
                txtRich.Document = obj;



            }
            catch (Exception ex)
            {
                //!!! J Notation:  Message box format to be used
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        //FUNCTION TO LOAD FETCHED DATA TO DATAGRID
        private DataTable GetDataDG()
        {
            DataTable dt = new DataTable("filename");
            try
            {

                SqlCommand cmd = new SqlCommand("SELECT filename from [Fusion_Database].[dbo].[NCPROG] ORDER BY filename ASC", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                //!!! J Notation:  Close sqldata reader. ANticapate when it retuns empty value.
                dt.Load(reader);

            }
            catch (Exception ex)
            {
                //!!! J Notation:  Message box format to be used
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

        //FUNCTION TO LOAD FETCHED DATA TO TEXTBOXES
        private void LoadToTextBoxes()
        {
            try
            {

                string id = GetDataDG().Rows[0]["id"].ToString();
                //!!! J Notation:  Do not use static values
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] WHERE id = " + id + " ORDER BY filename ASC", conn);
                conn.Open();   
                //!!! J Notation:  Close sqldata reader. ANticapate when it retuns empty value.
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                //!!! J Notation: Close SQL Reader and use proper field name for better readability than the column index number. 
                txtPath.Text = reader[8].ToString();
                txtReferenceId.Text = reader[1].ToString();
                txtRemoteRequestId.Text = reader[2].ToString();

                string filePath = reader[3].ToString();

                long fileSizeinBytes = GetFileSize(filePath); //filesize of actual file
                txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                txtLastModified.Text = modification.ToString();


                var bytes = File.ReadAllBytes(filePath);
                var text = Encoding.UTF8.GetString(bytes);



                FlowDocument obj = new FlowDocument();

                Paragraph objPar1 = new Paragraph();
                objPar1.Inlines.Add(new Run(text));
                obj.Blocks.Add(objPar1);
                txtRich.Document = obj;



            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        //FUNCTION TO GET THE FILESIZE FROM ACTUAL FILE
        static long GetFileSize(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                return new FileInfo(FilePath).Length;     //!!! J Notation: 
            }
            return 0;
        }

        private void btnSearch(object sender, RoutedEventArgs e)
        {
            string search_value = txtBoxSearch.Text;
            string id = GetDataDG().Rows[0]["id"].ToString();
            DataTable dt = new DataTable();
            try
            {

                if (search_value != "")
                {
                    //!!! J Notation:  Remove all static values in the database connection string
                    SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] WHERE filename LIKE '%" + search_value + "%' ", conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        MessageBox.Show("No control programs found matching search criteria", "Control Program Navigator", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        //dt.Load(reader);
                        dgView1.ItemsSource = dt.DefaultView;
                        txtPath.Text = reader[8].ToString();
                        txtReferenceId.Text = reader[1].ToString();
                        txtRemoteRequestId.Text = reader[2].ToString();
                        dt.Load(reader);
                    }



                }
                else
                {
                    MessageBox.Show("No search value", "Search", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }

        }

        private void btnClearSearch(object sender, RoutedEventArgs e)
        {
            txtBoxSearch.Text = "";
            LoadToTextBoxes();
            dgView1.ItemsSource = GetDataDG().DefaultView;
        }
    }
}
