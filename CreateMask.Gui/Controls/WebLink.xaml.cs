using System.Windows;
using System.Windows.Input;
using CreateMask.Utilities;

namespace CreateMask.Gui.Controls
{
    /// <summary>
    /// Interaction logic for WebLink.xaml
    /// </summary>
    public partial class WebLink
    {
        #region Properties
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(WebLink));

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }
        public static DependencyProperty UrlProperty = DependencyProperty.Register(nameof(Url), typeof(string), typeof(WebLink));
        #endregion

        public WebLink()
        {
            InitializeComponent();
            MainControl.DataContext = this;
        }

        private void MainControlOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Web.OpenUrlInDefaultBrowser(Url);
        }
    }
}
