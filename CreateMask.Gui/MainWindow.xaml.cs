using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using CreateMask.Containers;

namespace CreateMask.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ApplicationArguments Arguments { get; set; }

        public MainWindow()
        {
            Arguments = new ApplicationArguments
            {
                MeasurementsNrOfRows = 7,
                MeasurementsNrOfColumns = 12,
                High = byte.MaxValue,
                Low = 175,
                LcdWidth = 2560,
                LcdHeight = 1440,
                DesiredResistance = 8820
            };
            InitializeComponent();
        }

        private void BtnCreateMask_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                tbxOutput.Clear();
                var main = new Main.Main(Arguments);
                main.Output += Main_Output;
                main.CreateMask();
            }
            catch (Exception exception)
            {
                Output(exception.Message);
                Output(exception.StackTrace);
            }
        }

        private void Main_Output(object sender, string e)
        {
            Output(e);
        }

        private void Output(string output)
        {
            tbxOutput.AppendText(output + Environment.NewLine);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
