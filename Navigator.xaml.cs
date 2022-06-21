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
    /// Interaction logic for Navigator.xaml
    /// </summary>
    public partial class Navigator : UserControl
    {
        BackgroundWorker worker;
        public Navigator()
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
                    //labelNoData.Visibility = Visibility.Visible;
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
            cmbBoxViewList.Items.Add("Control Program Filename");
            cmbBoxViewList.Items.Add("Reference ID");
            cmbBoxViewList.Items.Add("Remote Request ID");
            cmbBoxViewList.SelectedIndex = 0;
            dgProgramFiles.ItemsSource = GetDataDG().DefaultView;
            dgProgramFiles.SelectedIndex = 0;

            dgProgramFiles.Focus();

            LoadToTextBoxes();
            
        }


        private void LoadToTextBoxes()
        {
            //txtAssociatedCustomers.Items.Clear();
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
                    txtPath.Text = "";
                    txtReferenceId.Text = "";
                    txtRemoteRequestId.Text = "";
                    txtLastModified.Text = "";
                    txtTopViewOfFile.Text = "";
                    txtControlProgramGroup.Text = "";
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

        private void dgProgramFiles_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView row = dataGrid.SelectedItem as DataRowView;
            //string myCellValue = rowView.Row[0].ToString();
            string condition = row[0].ToString();

            //txtAssociatedCustomers.Items.Clear();
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
