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
using Fusion_PDO.Class;
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

            ProgressBar1.Minimum = 1;
            ProgressBar1.Maximum = 100;
            worker.RunWorkerAsync();
            dgProgramFiles.IsEnabled = false;
        }

        
        SqlConnection conn;
        db connect = new db();
        public void LoadDB()
        {
            connect.getData();
        }


        //FUNCTION TO LOAD FETCHED DATA TO DATAGRID
        private DataTable GetDataDG()
        {

            DataTable dt = new DataTable();
            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] ORDER BY filename ASC", connect.conn);
                connect.conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                dt.Load(reader);
                lblList.Text = "Showing List by Control Program(s)";
                lblGrid.Text = "Control Program Files";
                if (dt.Rows.Count > 0)
                {

                    return dt;
                }
                else
                {
                    labelNoData.Visibility = Visibility.Visible;
                    dgProgramFiles.IsEnabled = false;
                    
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
            }
           return dt;
        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 1; i <= 10; ++i)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(300);
                    worker.ReportProgress(i * 10);
                }
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
            cmbBoxViewList.Items.Add("Control Program Filename");
            cmbBoxViewList.Items.Add("Reference ID");
            cmbBoxViewList.Items.Add("Remote Request ID");
            cmbBoxViewList.SelectedIndex = 0;
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
                if (id != "")
                {
                    SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + id + "  ORDER BY filename ASC", connect.conn);
                    connect.conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtPath.Text = reader["programPointer"].ToString();
                        txtReferenceId.Text = reader["UniqueReference"].ToString();
                        txtRemoteRequestId.Text = reader["remoteCallId"].ToString();

                        string filePath = reader["programPointer"].ToString();
                        long fileSize = GetFileSize(filePath); //filesize of actual file
                        txtFileSize.Text = fileSize.ToString() + " bytes";

                        DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                        txtLastModified.Text = modification.ToString();

                        byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                        txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                        txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                        txtBottomViewOfFile.ScrollToEnd();

                        txtControlProgramGroup.Text = reader["machine_group_name"].ToString();

                       
                        reader.Close();
                        connect.conn.Close();
                    }

                }
                else
                {
                    txtPath.IsEnabled = false;
                    txtReferenceId.IsEnabled = false;
                    txtRemoteRequestId.IsEnabled = false;
                    txtLastModified.IsEnabled = false;
                    txtTopViewOfFile.IsEnabled = false;
                    txtControlProgramGroup.IsEnabled = false;
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
           
        }

        public static string BottomViewOfFile(string filename, int numChars)
        {
            var fileInfo = new FileInfo(filename);

            using (var stream = File.OpenRead(filename))
            {
                if (fileInfo.Length > numChars)
                {
                    stream.Seek(fileInfo.Length - numChars, SeekOrigin.Begin);
                    using (var textReader = new StreamReader(stream))
                    {
                        return textReader.ReadToEnd();
                    }
                }
                else
                {
                    //stream.Seek(fileInfo.Length, SeekOrigin.Begin);
                    using (var textReader = new StreamReader(stream))
                    {
                        return textReader.ReadToEnd();
                    }
                }


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

        //CONTROL PROGRAM VIEW
       
        private DataTable getReferenceID()
        {
            DataTable dt = new DataTable();

            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] ORDER BY filename ASC", connect.conn);
                connect.conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                dt.Load(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
            }
            return dt;
        }
        private DataTable getRemoteID()
        {
            DataTable dt = new DataTable();

            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] ORDER BY filename ASC", connect.conn);
                connect.conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
            }
            return dt;
        }

        //SEARCH BUTTON FUNCTION
        private void btnSearch(object sender, RoutedEventArgs e)
        {
            string search_value = txtSearch.Text;
            if (search_value == "")
            {
                MessageBox.Show("Please input necessary search term first.", "Control Program Navigator", MessageBoxButton.OK, MessageBoxImage.Information);
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
            string query = "SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id  WHERE NCPROG.id LIKE '%" + search_value + "%' ";

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
                SqlCommand cmd = new SqlCommand(query.ToString(), connect.conn);
                connect.conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dgProgramFiles.ItemsSource = dt.DefaultView;
                    dgReferenceId.ItemsSource = dt.DefaultView;
                    dgRemoteRequestId.ItemsSource = dt.DefaultView;
                    dgProgramFiles.SelectedIndex = 0;
                    dgProgramFiles.Focus();
                    dgReferenceId.SelectedIndex = 0;
                    dgReferenceId.Focus();
                    dgRemoteRequestId.SelectedIndex = 0;
                    dgRemoteRequestId.Focus();

                    txtPath.Text = dt.Rows[0]["programPointer"].ToString();
                    txtReferenceId.Text = dt.Rows[0]["UniqueReference"].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0]["remoteCallId"].ToString();

                    string filePath = dt.Rows[0]["programPointer"].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                    txtLastModified.Text = modification.ToString();

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                    txtBottomViewOfFile.ScrollToEnd();

                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();

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
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
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
                SqlCommand cmd = new SqlCommand("SELECT * from [Machine_Groups]", connect.conn);
                connect.conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["machine_group_name"].ToString();
                comboBoxName.SelectedValuePath = ds.Tables[0].Columns["machine_group_id"].ToString();
                //comboBoxName.Text = ds.Tables[0].Columns[1].ToStrin g();
                comboBoxName.SelectedIndex = 0;
                da.Dispose();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
            }

        }

        //COMBOBOX ASSOCIATED CUSTOMER 
        public void FillComboBoxAssocCustomer(ComboBox comboBoxName2)
        {

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [CUSTOMERS]", connect.conn);
                connect.conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                comboBoxName2.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName2.DisplayMemberPath = ds.Tables[0].Columns["customer_name"].ToString();
                comboBoxName2.SelectedValuePath = ds.Tables[0].Columns["customer_id"].ToString();
                //comboBoxName.Text = ds.Tables[0].Columns[1].ToStrin g();
                comboBoxName2.SelectedIndex = 0;
                da.Dispose();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
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
                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id  WHERE fkMachGroupId = " + mach_id + " ", connect.conn);
                connect.conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {

                    dgProgramFiles.ItemsSource = dt.DefaultView;
                    dgReferenceId.ItemsSource = dt.DefaultView;
                    dgRemoteRequestId.ItemsSource = dt.DefaultView;
                    dgProgramFiles.SelectedIndex = 0;
                    dgProgramFiles.Focus();

                    txtPath.Text = dt.Rows[0]["programPointer"].ToString();
                    txtReferenceId.Text = dt.Rows[0]["UniqueReference"].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0]["remoteCallId"].ToString();

                    string filePath = dt.Rows[0]["programPointer"].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                    txtLastModified.Text = modification.ToString();

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                    txtBottomViewOfFile.ScrollToEnd();

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
                connect.conn.Close();
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
                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id INNER JOIN [CtrlProgCustMGAssoc] ON NCPROG.UniqueReference=urid INNER JOIN [CUSTOMERS]  ON CtrlProgCustMGAssoc.custid=customer_id  WHERE customer_id = " + assoc_id + " ", connect.conn);
                connect.conn.Open();
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

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
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
                connect.conn.Close();
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

                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", connect.conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtPath.Text = dt.Rows[0]["programPointer"].ToString();
                    txtReferenceId.Text = dt.Rows[0]["UniqueReference"].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0]["remoteCallId"].ToString();

                    string filePath = dt.Rows[0]["programPointer"].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                    txtLastModified.Text = modification.ToString();

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                    txtBottomViewOfFile.ScrollToEnd();

                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                }
                else
                {
                    txtPath.Text = "";
                    txtReferenceId.Text = "";
                    txtRemoteRequestId.Text = "";
                    txtFileSize.Text = "";
                    txtLastModified.Text = "";

                    txtControlProgramGroup.Text = "";
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
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
                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", connect.conn);
                connect.conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtPath.Text = dt.Rows[0]["programPointer"].ToString();
                    txtReferenceId.Text = dt.Rows[0]["UniqueReference"].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0]["remoteCallId"].ToString();

                    string filePath = dt.Rows[0]["programPointer"].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                    txtLastModified.Text = modification.ToString();

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                    txtBottomViewOfFile.ScrollToEnd();

                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                }
                else
                {
                    txtPath.Text = "";
                    txtReferenceId.Text = "";
                    txtRemoteRequestId.Text = "";
                    txtFileSize.Text = "";
                    txtLastModified.Text = "";

                    txtControlProgramGroup.Text = "";
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
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
                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", connect.conn);
                connect.conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtPath.Text = dt.Rows[0]["programPointer"].ToString();
                    txtReferenceId.Text = dt.Rows[0]["UniqueReference"].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0]["remoteCallId"].ToString();

                    string filePath = dt.Rows[0]["programPointer"].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                    txtLastModified.Text = modification.ToString();

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                    txtBottomViewOfFile.ScrollToEnd();

                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                    Process.Start("notepad.exe", txtPath.Text);

                }
                else
                {
                    txtPath.Text = "";
                    txtReferenceId.Text = "";
                    txtRemoteRequestId.Text = "";
                    txtFileSize.Text = "";
                    txtLastModified.Text = "";

                    txtControlProgramGroup.Text = "";
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
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
                    SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", connect.conn);
                    connect.conn.Open();
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
                    connect.conn.Close();
                }
            }
        }

        private void ctxMenu_EditFile_Click(object sender, RoutedEventArgs e)
        {

            Process.Start("notepad.exe", txtPath.Text);
        }

        private void dgReference_LeftClick(object sender, MouseButtonEventArgs e)
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

                    SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", connect.conn);
                    connect.conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    reader.Read();

                    txtPath.Text = reader["programPointer"].ToString();
                    txtReferenceId.Text = reader["UniqueReference"].ToString();
                    txtRemoteRequestId.Text = reader["remoteCallId"].ToString();

                    string filePath = reader["programPointer"].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                    txtLastModified.Text = modification.ToString();

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                    txtBottomViewOfFile.ScrollToEnd();

                    txtControlProgramGroup.Text = reader["machine_group_name"].ToString();

                    reader.Close();
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                connect.conn.Close();
            }
        }

        private void dgRemoteRequest_LeftClick(object sender, MouseButtonEventArgs e)
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

                    SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", connect.conn);
                    connect.conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    reader.Read();
                    txtPath.Text = reader["programPointer"].ToString();
                    txtReferenceId.Text = reader["UniqueReference"].ToString();
                    txtRemoteRequestId.Text = reader["remoteCallId"].ToString();

                    string filePath = reader["programPointer"].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                    txtLastModified.Text = modification.ToString();

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                    txtBottomViewOfFile.ScrollToEnd();

                    txtControlProgramGroup.Text = reader["machine_group_name"].ToString();

                    reader.Close();
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                connect.conn.Close();
            }
        }

        private void dgReference_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView row = dataGrid.SelectedItem as DataRowView;
            //string myCellValue = rowView.Row[0].ToString();
            string condition = row[0].ToString();

            txtAssociatedCustomers.Items.Clear();
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", connect.conn);
                connect.conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtPath.Text = dt.Rows[0]["programPointer"].ToString();
                    txtReferenceId.Text = dt.Rows[0]["UniqueReference"].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0]["remoteCallId"].ToString();

                    string filePath = dt.Rows[0]["programPointer"].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                    txtLastModified.Text = modification.ToString();

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                    txtBottomViewOfFile.ScrollToEnd();

                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                    Process.Start("notepad.exe", txtPath.Text);

                }
                else
                {
                    txtPath.Text = "";
                    txtReferenceId.Text = "";
                    txtRemoteRequestId.Text = "";
                    txtFileSize.Text = "";
                    txtLastModified.Text = "";

                    txtControlProgramGroup.Text = "";
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
            }
        }

        private void dgRemoteRequest_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView row = dataGrid.SelectedItem as DataRowView;
            //string myCellValue = rowView.Row[0].ToString();
            string condition = row[0].ToString();

            txtAssociatedCustomers.Items.Clear();
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [NCPROG] INNER JOIN [Machine_Groups] ON NCPROG.fkMachGroupId=machine_group_id WHERE NCPROG.id = " + condition + " ", connect.conn);
                connect.conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtPath.Text = dt.Rows[0]["programPointer"].ToString();
                    txtReferenceId.Text = dt.Rows[0]["UniqueReference"].ToString();
                    txtRemoteRequestId.Text = dt.Rows[0]["remoteCallId"].ToString();

                    string filePath = dt.Rows[0]["programPointer"].ToString();
                    long fileSizeinBytes = GetFileSize(filePath);
                    txtFileSize.Text = fileSizeinBytes.ToString() + " bytes";

                    DateTime modification = File.GetLastWriteTime(filePath); //last modification date of actual file
                    txtLastModified.Text = modification.ToString();

                    byte[] test = File.ReadAllBytes(filePath).Skip(0).Take(512).ToArray();
                    txtTopViewOfFile.Text = Encoding.UTF8.GetString(test);

                    txtBottomViewOfFile.Text = BottomViewOfFile(filePath, 512);
                    txtBottomViewOfFile.ScrollToEnd();

                    txtControlProgramGroup.Text = dt.Rows[0]["machine_group_name"].ToString();
                    Process.Start("notepad.exe", txtPath.Text);

                }
                else
                {
                    txtPath.Text = "";
                    txtReferenceId.Text = "";
                    txtRemoteRequestId.Text = "";
                    txtFileSize.Text = "";
                    txtLastModified.Text = "";

                    txtControlProgramGroup.Text = "";
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                connect.conn.Close();
            }
        }

        private void cmbBoxViewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string cmb = cmbBoxViewList.SelectedItem.ToString();

            if (cmb == "Reference ID")
            {
                lblList.Text = "Showing List by Reference ID(s)";
                lblGrid.Text = "Reference ID(s)";
                if (txtSearch.Text == String.Empty)
                {
                    dgReferenceId.ItemsSource = getReferenceID().DefaultView;
                    dgReferenceId.SelectedIndex = 0;
                    dgReferenceId.Focus();
                    dgProgramFiles.Visibility = Visibility.Hidden;
                    dgReferenceId.Visibility = Visibility.Visible;
                    dgRemoteRequestId.Visibility = Visibility.Hidden;
                }
                else
                {
                    //Search();
                    dgProgramFiles.Visibility = Visibility.Hidden;
                    dgReferenceId.Visibility = Visibility.Visible;
                    dgRemoteRequestId.Visibility = Visibility.Hidden;
                }
            }
            else if (cmb == "Control Program Filename")
            {
                lblList.Text = "Showing List by Control Program(s)";
                lblGrid.Text = "Control Program Files";
                if (txtSearch.Text == String.Empty)
                {
                    dgProgramFiles.ItemsSource = GetDataDG().DefaultView;
                    dgProgramFiles.SelectedIndex = 0;
                    dgProgramFiles.Focus();
                    dgProgramFiles.Visibility = Visibility.Visible;
                    dgReferenceId.Visibility = Visibility.Hidden;
                    dgRemoteRequestId.Visibility = Visibility.Hidden;
                }
                else
                {
                    //Search();
                    dgProgramFiles.SelectedIndex = 0;
                    dgProgramFiles.Focus();
                    dgProgramFiles.Visibility = Visibility.Visible;
                    dgReferenceId.Visibility = Visibility.Hidden;
                    dgRemoteRequestId.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                lblList.Text = "Showing List by Remote Request ID(s)";
                lblGrid.Text = "Remote Request ID(s)";
                if (txtSearch.Text == String.Empty)
                {
                    dgRemoteRequestId.ItemsSource = getRemoteID().DefaultView;
                    dgRemoteRequestId.SelectedIndex = 0;
                    dgRemoteRequestId.Focus();
                    dgRemoteRequestId.Visibility = Visibility.Visible;
                    dgReferenceId.Visibility = Visibility.Hidden;
                    dgProgramFiles.Visibility = Visibility.Hidden;
                }
                else
                {
                    //Search();
                    dgRemoteRequestId.Visibility = Visibility.Visible;
                    dgReferenceId.Visibility = Visibility.Hidden;
                    dgProgramFiles.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
