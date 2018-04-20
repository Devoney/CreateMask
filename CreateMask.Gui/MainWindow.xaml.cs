using System.Windows;
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
                DesiredResistance = 200
            };
            InitializeComponent();
        }

        private void BtnCreateMask_OnClick(object sender, RoutedEventArgs e)
        {
            var main = new Main.Main(Arguments);
            main.CreateMask();
        }
    }
}
