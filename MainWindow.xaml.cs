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
using Fusion_PDO.ViewMenu;
using MaterialDesignThemes.Wpf;
using Fusion_PDO.Class;
using System.IO;
using System.Windows.Media.Animation;
using System.Configuration;
namespace Fusion_PDO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var SetUp = new List<SubMenu>();
            SetUp.Add(new SubMenu("Licence", new LicenceItem()));
            SetUp.Add(new SubMenu("Application"));
            SetUp.Add(new SubMenu("Service"));
            SetUp.Add(new SubMenu("DNC Events"));
            SetUp.Add(new SubMenu("Users"));
            var item1 = new Menus("Setup", SetUp, PackIconKind.Settings);

            var menuHelp = new List<SubMenu>();
            menuHelp.Add(new SubMenu("User's Guide"));
            menuHelp.Add(new SubMenu("Diagnostics"));
            var item2 = new Menus("Help", menuHelp, PackIconKind.Help);

            var menuProcess = new List<SubMenu>();
            menuProcess.Add(new SubMenu("Process and Operations"));
            menuProcess.Add(new SubMenu("Parts, Operations and Customers"));
            menuProcess.Add(new SubMenu("Customer Manager"));
            var item3 = new Menus("Process", menuProcess, PackIconKind.Circle);

            var menuMachines = new List<SubMenu>();
            menuMachines.Add(new SubMenu("Setup"));
            var item4 = new Menus("Machines", menuMachines, PackIconKind.StateMachine);

            var menuControlPrograms = new List<SubMenu>();
            
            menuControlPrograms.Add(new SubMenu("Navigator", new ControlProgramNavigator2()));
            var item5 = new Menus("Control Programs", menuControlPrograms, PackIconKind.SettingsApplications);

            Menu.Children.Add(new UserControlMenuItem(item1, this));
            Menu.Children.Add(new UserControlMenuItem(item2, this));
            Menu.Children.Add(new UserControlMenuItem(item3, this));
            Menu.Children.Add(new UserControlMenuItem(item4, this));
            Menu.Children.Add(new UserControlMenuItem(item5, this));

            Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close fusion?", "Fusion Closing", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        internal void SwitchScreen(object sender)
        {
            var screen = ((UserControl)sender);

            if (screen != null)
            {
                StackPanelMain.Children.Clear();
                StackPanelMain.Children.Add(screen);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void gridHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        db connect = new db();
        public List<string> myList = new List<string>();

        //temporary connection
        private string _server;
        private string _database;
        private string _username;
        private string _password;
        private string _decryptedPassword;
        void LoadDataFromFile()
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

        //DECRYPT FUNCTION
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

        //MAIN WINDOW LOADED FUNCTION
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadDataFromFile();
            connect.server = _server;
            connect.database = _database;
            connect.username = _username;
            connect.password = _decryptedPassword;
            connect.getData();
            try
            {
                connect.conn.Open();
                //MessageBox.Show("Connected" + connect.password.ToString());
                connect.conn.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show("Not connected" + ee.ToString());
                Application.Current.Shutdown();
            }
        }
        bool MenuClosed = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MenuClosed)
            {
                Storyboard openMenu = (Storyboard)button.FindResource("OpenMenu");
                openMenu.Begin();
                button.Content = FindResource("close");
            }
            else
            {
                Storyboard closeMenu = (Storyboard)button.FindResource("CloseMenu");
                closeMenu.Begin();
                button.Content = FindResource("open");
            }
            MenuClosed = !MenuClosed;
        }
    }
}
