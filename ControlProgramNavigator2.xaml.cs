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
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace Fusion_PDO
{
    /// <summary>
    /// Interaction logic for ControlProgramNavigator2.xaml
    /// </summary>
    public partial class ControlProgramNavigator2 : UserControl
    {
        BackgroundWorker worker;
        public ControlProgramNavigator2()
        {
            InitializeComponent();
            LoadDB();


            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            int maxItems = 15;
            ProgressBar1.Minimum = 1;
            ProgressBar1.Maximum = 100;
            worker.RunWorkerAsync(maxItems);
            dgProgramFiles.IsEnabled = false;
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
                MessageBox.Show("Can not open connection ! " + ex.ToString());
                Application.Current.Shutdown();
            }
        }


        //FUNCTION TO LOAD FETCHED DATA TO DATAGRID
        private DataTable GetDataDG()
        {

            DataTable dt = new DataTable();
            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] ORDER BY filename ASC", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                dt.Load(reader);
                lblList.Text = "Showing List by Control Program(s)";


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
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int? maxItems = e.Argument as int?;
            for (int i = 1; i <= maxItems.GetValueOrDefault(); ++i)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                Thread.Sleep(200);
                worker.ReportProgress(i);

            }
        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double percent = (e.ProgressPercentage * 100) / 50;

            ProgressBar1.Value = Math.Round(percent, 0);

        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            ProgressBar1.Visibility = Visibility.Hidden;
            Loading.Visibility = Visibility.Hidden;
            dgProgramFiles.IsEnabled = true;
            chkBoxAssociatedCustomer.IsChecked = true;
            chkBoxControlProgramGroup.IsChecked = true;
            chkBoxDescription.IsChecked = true;
            chkBoxFilename.IsChecked = true;
            chkBoxRemoteReferenceId.IsChecked = true;
            chkBoxRemoteRequestId.IsChecked = true;
            dgProgramFiles.ItemsSource = GetDataDG().DefaultView;
            dgProgramFiles.SelectedIndex = 0;

            dgProgramFiles.Focus();

            LoadToTextBoxes();
            FillComboBoxProgramGroup(comboProgramGroup);
            FillComboBoxAssocCustomer(assocCustomer);
        }

        //FUNCTION TO LOAD FETCHED DATA TO TEXTBOXES
        private void LoadToTextBoxes()
        {
            txtAssociatedCustomers.Items.Clear();
            try
            {
                string id = GetDataDG().Rows[0]["id"].ToString();

                //!!! J Notation:  Make sure to close sqlDataReader when opening it for a read. Anticapate if there is no data in the table. You should handle empty data.
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] INNER JOIN [Fusion_Database].[dbo].[Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + id + "  ORDER BY filename ASC", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                txtPath.Text = reader[8].ToString();
                txtReferenceId.Text = reader[1].ToString();
                txtRemoteRequestId.Text = reader[2].ToString();

                string filePath = reader[3].ToString();

                long fileSize = GetFileSize(filePath); //filesize of actual file
                txtFileSize.Text = fileSize.ToString() + " bytes";

                DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                txtLastModified.Text = modification.ToString();

               
                txtControlProgramGroup.Text = reader["machine_group_name"].ToString();
                //txtAssociatedCustomers.Items.Add(reader["custName"].ToString());
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

        private void btnControlProgram(object sender, RoutedEventArgs e)
        {
            lblList.Text = "Showing List by Control Program(s)";
            dgProgramFiles.ItemsSource = GetDataDG().DefaultView;
            dgProgramFiles.SelectedIndex = 0;
            dgProgramFiles.Focus();
            dgProgramFiles.Visibility = Visibility.Visible;
            dgReferenceId.Visibility = Visibility.Hidden;
            dgRemoteRequestId.Visibility = Visibility.Hidden;
        }

        private void btnReferenceId(object sender, RoutedEventArgs e)
        {
            lblList.Text = "Showing List by Reference ID(s)";
            dgReferenceId.ItemsSource = getReferenceID().DefaultView;
            dgProgramFiles.Visibility = Visibility.Hidden;
            dgReferenceId.Visibility = Visibility.Visible;
            dgRemoteRequestId.Visibility = Visibility.Hidden;
        }

        private void btnRemoteRequestId(object sender, RoutedEventArgs e)
        {
            lblList.Text = "Showing List by Remote Request ID(s)";
            dgRemoteRequestId.ItemsSource = getRemoteID().DefaultView;
            dgRemoteRequestId.Visibility = Visibility.Visible;
            dgReferenceId.Visibility = Visibility.Hidden;
            dgProgramFiles.Visibility = Visibility.Hidden;
        }
        private DataTable getReferenceID()
        {
            DataTable dt = new DataTable();

            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] ORDER BY filename ASC", conn);
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
        private DataTable getRemoteID()
        {
            DataTable dt = new DataTable();

            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] ORDER BY filename ASC", conn);
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

        //SEARCH BUTTON FUNCTION
        private void btnSearch(object sender, RoutedEventArgs e)
        {
            string search_value = txtSearch.Text;
            if (search_value == "")
            {

            }
            else
            {

                Search();
            }
        }

        //SEARCH FUNCTION
        private void Search()
        {
            
            string search_value = txtSearch.Text;
            string query = "SELECT * from [Fusion_Database].[dbo].[NCPROG] INNER JOIN [Fusion_Database].[dbo].[Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id  WHERE NCPROG.id LIKE '%" + search_value + "%' ";

            if (chkBoxFilename.IsChecked == true)
            {
                query += "OR filename LIKE '%" + search_value + "%' ";
            }
            else
            {
                query += "";
            }

            if (chkBoxRemoteReferenceId.IsChecked == true)
            {
                query += "OR UniqueReference LIKE '%" + search_value + "%' ";
            }
            else
            {
                query += "";
            }

            if (chkBoxRemoteRequestId.IsChecked == true)
            {
                query += "OR remoteCallId LIKE '%" + search_value + "%' ";
            }
            else
            {
                query += "";
            }

            if (chkBoxControlProgramGroup.IsChecked == true)
            {
                query += "OR machine_group_name LIKE '%" + search_value + "%' ";
            }
            else
            {
                query += "";
            }


            if (chkBoxDescription.IsChecked == true)
            {
                query += "OR descr LIKE '%" + search_value + "%' ";
            }
            else
            {
                query += "";
            }

            if (chkBoxAssociatedCustomer.IsChecked == false && chkBoxControlProgramGroup.IsChecked == false && chkBoxDescription.IsChecked == false && chkBoxFilename.IsChecked == false && chkBoxRemoteReferenceId.IsChecked == false && chkBoxRemoteRequestId.IsChecked == false)
            {
                txtSearch.IsEnabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.IsEnabled = true;

            }
            
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {

                    dgProgramFiles.ItemsSource = dt.DefaultView;
                    dgReferenceId.ItemsSource = dt.DefaultView;
                    dgRemoteRequestId.ItemsSource = dt.DefaultView;
                    dgProgramFiles.SelectedIndex = 0;
                    dgProgramFiles.Focus();

                    txtPath.Text = dt.Rows[0][8].ToString();
                    txtReferenceId.Text = dt.Rows[0][1].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0][2].ToString();
                    txtLastModified.Text = dt.Rows[0][7].ToString();
                    string filePath = dt.Rows[0][8].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    
                    //txtAssociatedCustomers.Text = dt.Rows[0]["custName"].ToString();
                    lblList.Text = "Showing List by Control Program(s) result for " + search_value.ToUpper() + " search.";
                }
                else
                {
                    MessageBox.Show("No Control Program found matching the search criteria.", "Control Program Navigator", MessageBoxButton.OK, MessageBoxImage.Information);
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

        //SEARCH CLEAR
        private void btnClear(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            lblList.Text = "";
            LoadToTextBoxes();
            dgProgramFiles.ItemsSource = GetDataDG().DefaultView;
            dgReferenceId.ItemsSource = getReferenceID().DefaultView;
            dgRemoteRequestId.ItemsSource = getRemoteID().DefaultView;
        }

        private void dgClickReference(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)dgReferenceId.SelectedItem;
            string condition = row["id"].ToString();

            try
            {
                if (condition == null)
                {
                    MessageBox.Show("No selected cell");
                }
                else
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

                    const int MAX_BUFFER = 512;
                    byte[] Buffer = new byte[MAX_BUFFER];
                    int BytesRead;
                    using (System.IO.FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        while ((BytesRead = fileStream.Read(Buffer, 0, MAX_BUFFER)) != 0)
                        {
                            string text = Encoding.UTF8.GetString(Buffer);
                            txtTopViewOfFile.Text = text.ToString();

                        }
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        private void dgClickRemoteRequestId(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)dgRemoteRequestId.SelectedItem;
            string condition = row["id"].ToString();

            try
            {
                if (condition == null)
                {
                    MessageBox.Show("No selected cell");
                }
                else
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

                    const int MAX_BUFFER = 512;
                    byte[] Buffer = new byte[MAX_BUFFER];
                    int BytesRead;
                    using (System.IO.FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        while ((BytesRead = fileStream.Read(Buffer, 0, MAX_BUFFER)) != 0)
                        {
                            string text = Encoding.UTF8.GetString(Buffer);
                            txtTopViewOfFile.Text = text.ToString();

                        }
                    txtControlProgramGroup.Text = reader["machine_group_name"].ToString();
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnCProgram(object sender, RoutedEventArgs e)
        {
            controlProgramPopup.Visibility = Visibility.Visible;
            btnCntrlProgramGroup.IsEnabled = false;
            btnCancel.Visibility = Visibility.Visible;
            assocCustomerGrid.Visibility = Visibility.Hidden;

        }

        //COMBOBOX CONTROL PROGRAM GROUP
        public void FillComboBoxProgramGroup(ComboBox comboBoxName)
        {

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[Machine_Groups]", conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns[1].ToString();
                comboBoxName.SelectedValuePath = ds.Tables[0].Columns[0].ToString();
                //comboBoxName.Text = ds.Tables[0].Columns[1].ToStrin g();
                comboBoxName.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                conn.Close();
            }

        }

        //COMBOBOX ASSOCIATED CUSTOMER 
        public void FillComboBoxAssocCustomer(ComboBox comboBoxName2)
        {

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[CUSTOMERS]", conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                comboBoxName2.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName2.DisplayMemberPath = ds.Tables[0].Columns[1].ToString();
                comboBoxName2.SelectedValuePath = ds.Tables[0].Columns[0].ToString();
                //comboBoxName.Text = ds.Tables[0].Columns[1].ToStrin g();
                comboBoxName2.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                conn.Close();
            }

        }

        private void btnClosePopUp(object sender, RoutedEventArgs e)
        {
            controlProgramPopup.Visibility = Visibility.Hidden;
            //comboProgramGroup.Items.Clear();
            btnCntrlProgramGroup.IsEnabled = true;
            btnCancel.Visibility = Visibility.Hidden;

        }

        private void btnOkPopUp(object sender, RoutedEventArgs e)
        {
            controlProgramPopup.Visibility = Visibility.Hidden;
            btnCntrlProgramGroup.IsEnabled = true;
            btnCancel.Visibility = Visibility.Visible;
            string mach_id = comboProgramGroup.SelectedValue.ToString();
            string mach_name = comboProgramGroup.Text;
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] INNER JOIN [Fusion_Database].[dbo].[Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id  WHERE fkMachGroupId = " + mach_id + " ", conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {

                    dgProgramFiles.ItemsSource = dt.DefaultView;
                    dgReferenceId.ItemsSource = dt.DefaultView;
                    dgRemoteRequestId.ItemsSource = dt.DefaultView;

                    txtPath.Text = dt.Rows[0][8].ToString();
                    txtReferenceId.Text = dt.Rows[0][1].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0][2].ToString();
                    txtLastModified.Text = dt.Rows[0][7].ToString();
                    string filePath = dt.Rows[0][8].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    var bytes = File.ReadAllBytes(filePath);
                    var text = Encoding.UTF8.GetString(bytes);
                    txtTopViewOfFile.Text = text.ToString();
                    txtBottomViewOfFile.Text = text.ToString();
                    txtBottomViewOfFile.ScrollToEnd();
                    //txtAssociatedCustomers.Text = dt.Rows[0]["custName"].ToString();
                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                }
                else
                {
                    MessageBox.Show("No Control Program found under " + mach_name + " group.", "Control Program Navigator", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnCancelCPG(object sender, RoutedEventArgs e)
        {
            btnCancel.Visibility = Visibility.Hidden;
            controlProgramPopup.Visibility = Visibility.Hidden;
            //comboProgramGroup.Items.Clear();
            btnCntrlProgramGroup.IsEnabled = true;
            LoadToTextBoxes();
            dgProgramFiles.ItemsSource = GetDataDG().DefaultView;
            dgReferenceId.ItemsSource = getReferenceID().DefaultView;
            dgRemoteRequestId.ItemsSource = getRemoteID().DefaultView;
            FillComboBoxProgramGroup(comboProgramGroup);
        }


        //uncheck checkboxes
        private void remoteRequestId_Unchecked(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void remoteReferenceId_Unchecked(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void controlProgramGroup_Unchecked(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void associatedCustomer_Unchecked(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void filename_Unchecked(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void description_Unchecked(object sender, RoutedEventArgs e)
        {
            Search();
        }

        //checked checkbox

        private void remoteRequestId_Checked(object sender, RoutedEventArgs e)
        {
            Search();
            txtSearch.IsEnabled = true;
        }

        private void referenceId_Checked(object sender, RoutedEventArgs e)
        {
            Search();
            txtSearch.IsEnabled = true;
        }

        private void controlProgram_Checked(object sender, RoutedEventArgs e)
        {
            Search();
            txtSearch.IsEnabled = true;
        }

        private void assocCustomer_Checked(object sender, RoutedEventArgs e)
        {
            Search();
            txtSearch.IsEnabled = true;
        }

        private void filename_Checked(object sender, RoutedEventArgs e)
        {
            Search();
            txtSearch.IsEnabled = true;
        }

        private void description_Checked(object sender, RoutedEventArgs e)
        {
            Search();
            txtSearch.IsEnabled = true;
        }

        private void btnOKAssocCustomer(object sender, RoutedEventArgs e)
        {
            assocCustomerGrid.Visibility = Visibility.Hidden;
            btnAssocCustomer.IsEnabled = true;
            
            string assoc_id = assocCustomer.SelectedValue.ToString();
            string assoc_name = assocCustomer.Text;
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] INNER JOIN [Fusion_Database].[dbo].[Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id INNER JOIN [Fusion_Database].[dbo].[CtrlProgCustMGAssoc] ON NCPROG.UniqueReference=urid INNER JOIN [Fusion_Database].[dbo].[CUSTOMERS]  ON CtrlProgCustMGAssoc.custid=customer_id  WHERE customer_id = " + assoc_id + " ", conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    
                    dgProgramFiles.ItemsSource = dt.DefaultView;
                    dgReferenceId.ItemsSource = dt.DefaultView;
                    dgRemoteRequestId.ItemsSource = dt.DefaultView;

                    txtPath.Text = dt.Rows[0][8].ToString();
                    txtReferenceId.Text = dt.Rows[0][1].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0][2].ToString();
                    txtLastModified.Text = dt.Rows[0][7].ToString();
                    string filePath = dt.Rows[0][8].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    var bytes = File.ReadAllBytes(filePath);
                    var text = Encoding.UTF8.GetString(bytes);
                    txtTopViewOfFile.Text = text.ToString();
                    txtBottomViewOfFile.Text = text.ToString();
                    txtBottomViewOfFile.ScrollToEnd();

                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                    //txtAssociatedCustomers.Text = dt.Rows[0]["custName"].ToString();
                }
                else
                {
                    MessageBox.Show("No Control Program found under " + assoc_name + " group.", "Control Program Navigator", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnCloseAssoc(object sender, RoutedEventArgs e)
        {
            assocCustomerGrid.Visibility = Visibility.Hidden;
            //comboProgramGroup.Items.Clear();
            btnAssocCustomer.IsEnabled = true;
           // btnCancel.Visibility = Visibility.Hidden;
        }

        private void btnAssocCustomer_Click(object sender, RoutedEventArgs e)
        {
            controlProgramPopup.Visibility = Visibility.Hidden;
            assocCustomerGrid.Visibility = Visibility.Visible;
            btnAssocCustomer.IsEnabled = false;
            
        }

        private void dgProgramFiles_rightClick(object sender, MouseButtonEventArgs e)
        {
            
            DataGrid dataGrid = sender as DataGrid;
            DataRowView row = dataGrid.SelectedItem as DataRowView;
            //string myCellValue = rowView.Row[0].ToString();
            string condition = row[0].ToString();

            txtAssociatedCustomers.Items.Clear();
            DataTable dt = new DataTable();
            try
            {
                
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] INNER JOIN [Fusion_Database].[dbo].[Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {

                    txtPath.Text = dt.Rows[0][8].ToString();
                    txtReferenceId.Text = dt.Rows[0][1].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0][2].ToString();
                    txtLastModified.Text = dt.Rows[0][7].ToString();
                    string filePath = dt.Rows[0][8].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    var bytes = File.ReadAllBytes(filePath);
                    var text = Encoding.UTF8.GetString(bytes);

                    //txtTopViewOfFile.Text = text.ToString();
                    //txtBottomViewOfFile.Text = text.ToString();
                    // txtBottomViewOfFile.ScrollToEnd();
                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                    //txtAssociatedCustomers.Items.Add(dt.Rows[0]["custName"].ToString());
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                conn.Close();
            }
        }

        private void dgProgramFIles_leftClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView row = dataGrid.SelectedItem as DataRowView;
            //string myCellValue = rowView.Row[0].ToString();
            string condition = row[0].ToString();

            txtAssociatedCustomers.Items.Clear();
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] INNER JOIN [Fusion_Database].[dbo].[Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {

                    txtPath.Text = dt.Rows[0][8].ToString();
                    txtReferenceId.Text = dt.Rows[0][1].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0][2].ToString();
                    txtLastModified.Text = dt.Rows[0][7].ToString();
                    string filePath = dt.Rows[0][8].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    var bytes = File.ReadAllBytes(filePath);
                    var text = Encoding.UTF8.GetString(bytes);
                    
                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                    //txtAssociatedCustomers.Items.Add(dt.Rows[0]["custName"].ToString());
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                conn.Close();
            }
        }

        private void searchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }

        private void copyPathClick(object sender, RoutedEventArgs e)
        {
            txtPath.SelectAll();
            txtPath.Copy();
        }

        private void dpPrgramFileDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView row = dataGrid.SelectedItem as DataRowView;
            //string myCellValue = rowView.Row[0].ToString();
            string condition = row[0].ToString();

            txtAssociatedCustomers.Items.Clear();
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] INNER JOIN [Fusion_Database].[dbo].[Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {

                    txtPath.Text = dt.Rows[0][8].ToString();
                    txtReferenceId.Text = dt.Rows[0][1].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0][2].ToString();
                    txtLastModified.Text = dt.Rows[0][7].ToString();
                    string filePath = dt.Rows[0][8].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    var bytes = File.ReadAllBytes(filePath);
                    var text = Encoding.UTF8.GetString(bytes);

                    //txtTopViewOfFile.Text = text.ToString();
                    //txtBottomViewOfFile.Text = text.ToString();
                    // txtBottomViewOfFile.ScrollToEnd();
                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                    //txtAssociatedCustomers.Items.Add(dt.Rows[0]["custName"].ToString());
                    //dgProgramFiles.ItemsSource = new DirectoryInfo(txtPath.Text).GetFiles();
                    Process.Start("notepad.exe", txtPath.Text);

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                conn.Close();
            }
            
        }

        private void txtTopViewOfFile_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start("notepad.exe", txtPath.Text);
        }

        private void txtBottomViewOfFile_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start("notepad.exe", txtPath.Text);
        }

        private void dgProgramFilesKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                DataGrid dataGrid = sender as DataGrid;
                DataRowView row = dataGrid.SelectedItem as DataRowView;
                //string myCellValue = rowView.Row[0].ToString();
                string condition = row[0].ToString();

                txtAssociatedCustomers.Items.Clear();
                DataTable dt = new DataTable();
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * from [Fusion_Database].[dbo].[NCPROG] INNER JOIN [Fusion_Database].[dbo].[Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", conn);
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {

                        txtPath.Text = dt.Rows[0][8].ToString();
                        txtReferenceId.Text = dt.Rows[0][1].ToString();
                        txtRemoteRequestId.Text = dt.Rows[0][2].ToString();
                        txtLastModified.Text = dt.Rows[0][7].ToString();
                        string filePath = dt.Rows[0][8].ToString();
                        long fileSizeinBytes = GetFileSize(filePath);
                        txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                        var bytes = File.ReadAllBytes(filePath);
                        var text = Encoding.UTF8.GetString(bytes);

                        byte[] buffer = new byte[1024];
                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            var bytes_read = fs.Read(buffer, 0, buffer.Length);
                            fs.Close();
                            txtTopViewOfFile.Text = Encoding.UTF8.GetString(buffer).ToString();
                        }

                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            var bytes_read = fs.Read(buffer, 0, buffer.Length);
                            fs.Close();
                            txtBottomViewOfFile.Text = Encoding.UTF8.GetString(bytes).ToString();
                            txtBottomViewOfFile.ScrollToEnd();

                        }
                        //txtTopViewOfFile.Text = text.ToString();
                        //txtBottomViewOfFile.Text = text.ToString();
                        // txtBottomViewOfFile.ScrollToEnd();
                        txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                        //txtAssociatedCustomers.Items.Add(dt.Rows[0]["custName"].ToString());
                        //dgProgramFiles.ItemsSource = new DirectoryInfo(txtPath.Text).GetFiles();
                        Process.Start("notepad.exe", txtPath.Text);

                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void ctxMenu_EditFile_Click(object sender, RoutedEventArgs e)
        {

            Process.Start("notepad.exe", txtPath.Text);
        }
    }
}

