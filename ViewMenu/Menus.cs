using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Fusion_PDO.ViewMenu
{
    public class Menus
    {
        public Menus(string header, List<SubMenu> submenu, PackIconKind icon)
        {
            Header = header;
            SubMenus = submenu;
            Icon = icon;
         
        }

        public string Header { get; private set; }  
        public PackIconKind Icon { get; private set; }  
        public List<SubMenu> SubMenus { get; private set; }
        public UserControl Screen { get; private set; }
    }
}
