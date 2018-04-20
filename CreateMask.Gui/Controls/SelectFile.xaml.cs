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

namespace CreateMask.Gui.Controls
{
    /// <summary>
    /// Interaction logic for SelectFile.xaml
    /// </summary>
    public partial class SelectFile
    {
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(SelectFile),
            new PropertyMetadata("File:")
        );

        public string SelectedFilePath
        {
            get { return (string)GetValue(SelectedFilePathProperty); }
            set { SetValue(SelectedFilePathProperty, value); }
        }

        public static readonly DependencyProperty SelectedFilePathProperty = DependencyProperty.Register(
            nameof(SelectedFilePath),
            typeof(string),
            typeof(SelectFile),
            new PropertyMetadata("")
        );

        public string Extension
        {
            get { return (string)GetValue(ExtensionProperty); }
            set { SetValue(ExtensionProperty, value); }
        }

        public static readonly DependencyProperty ExtensionProperty = DependencyProperty.Register(
            nameof(Extension),
            typeof(string),
            typeof(SelectFile),
            new PropertyMetadata("")
        );

        public SelectFile()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = Extension,
                Filter = "Files|*" + Extension
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            
            SelectedFilePath = dlg.FileName;
        }
    }
}
