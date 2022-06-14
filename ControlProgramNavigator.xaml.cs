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
    /// <summary>
    /// Interaction logic for ControlProgramNavigator.xaml
    /// </summary>
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

                dt.Load(reader);

            }
            catch (Exception ex)
            {
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
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] WHERE id = " + id + " ORDER BY filename ASC", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

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
                return new FileInfo(FilePath).Length;
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
