#pragma checksum "..\..\ControlProgramNavigator.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A5A1C6BA6A2FC7A32A3BF2B95347E3B764BBBB4DF75703BA142EFDC456C8AEC5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Fusion_PDO;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Fusion_PDO {
    
    
    /// <summary>
    /// ControlProgramNavigator
    /// </summary>
    public partial class ControlProgramNavigator : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 35 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgView1;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtPath;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtReferenceId;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRemoteRequestId;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtControlProgramGroup;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtAssociatedCustomers;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBoxSearch;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RichTextBox txtRich;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtLastModified;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\ControlProgramNavigator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtFileSize;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Fusion_PDO;component/controlprogramnavigator.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ControlProgramNavigator.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\ControlProgramNavigator.xaml"
            ((Fusion_PDO.ControlProgramNavigator)(target)).Loaded += new System.Windows.RoutedEventHandler(this.LoadData);
            
            #line default
            #line hidden
            return;
            case 2:
            this.dgView1 = ((System.Windows.Controls.DataGrid)(target));
            
            #line 35 "..\..\ControlProgramNavigator.xaml"
            this.dgView1.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.dgClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtPath = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.txtReferenceId = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.txtRemoteRequestId = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.txtControlProgramGroup = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.txtAssociatedCustomers = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.txtBoxSearch = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            
            #line 77 "..\..\ControlProgramNavigator.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSearch);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 80 "..\..\ControlProgramNavigator.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnClearSearch);
            
            #line default
            #line hidden
            return;
            case 11:
            this.txtRich = ((System.Windows.Controls.RichTextBox)(target));
            return;
            case 12:
            this.txtLastModified = ((System.Windows.Controls.TextBox)(target));
            return;
            case 13:
            this.txtFileSize = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

