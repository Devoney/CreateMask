using System;
using System.Reflection;
using System.Windows;
using CreateMask.Containers;
using CreateMask.Main;
using Ninject;

namespace CreateMask.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Main.Main _main;

        public ApplicationArguments Arguments { get; set; }

        public string Version
        {
            get
            {
                return "v" +Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public MainWindow()
        {
            var kernel = KernelConstructor.GetKernel();
            _main = kernel.Get<Main.Main>();
            _main.Output += Main_Output;

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
                _main.CreateMask(Arguments);
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
    }
}
