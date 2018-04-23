using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CreateMask.Gui.Annotations;
using Microsoft.Win32;

namespace CreateMask.Gui.Controls
{
    /// <summary>
    /// Interaction logic for SelectFile.xaml
    /// </summary>
    public partial class SelectFile : INotifyPropertyChanged
    {
        public string DefaultExtension
        {
            get { return (string)GetValue(DefaultExtensionProperty); }
            set
            {
                SetValue(DefaultExtensionProperty, value);
                OnPropertyChanged(nameof(Label));
            }
        }

        public static readonly DependencyProperty DefaultExtensionProperty = DependencyProperty.Register(
            nameof(DefaultExtension),
            typeof(string),
            typeof(SelectFile),
            new PropertyMetadata("")
        );

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set
            {
                SetValue(LabelProperty, value);
                OnPropertyChanged(nameof(Label));
            }
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
            set
            {
                SetValue(SelectedFilePathProperty, value);
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }

        public static readonly DependencyProperty SelectedFilePathProperty = DependencyProperty.Register(
            nameof(SelectedFilePath),
            typeof(string),
            typeof(SelectFile),
            new PropertyMetadata("")
        );

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            nameof(Filter),
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
            var dlg = new OpenFileDialog
            {
                DefaultExt = DefaultExtension,
                Filter = Filter,
                CheckFileExists = false
            };
            
            var result = dlg.ShowDialog();
            if (result != true) return;
            
            SelectedFilePath = dlg.FileName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
