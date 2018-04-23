using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using CreateMask.Containers;
using CreateMask.Contracts.Enums;
using CreateMask.Contracts.Helpers;
using CreateMask.Gui.Controls;
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

        public IEnumerable<string> SupportedFileTypes => _main.SupportedFileTypes;

        public string FileFilter
        {
            get
            {
                var fileFilter = "";
                foreach (var supportedFileType in SupportedFileTypes)
                {
                    fileFilter += $"{supportedFileType}|*.{supportedFileType}|";
                }
                fileFilter = fileFilter.Substring(0, fileFilter.Length - 1);
                return fileFilter;
            }
        }

        public string Version => "v" +Assembly.GetExecutingAssembly().GetName().Version;

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
                DesiredResistance = 8820,
                FileType = ImageFileType.Png.ToString()
            };
            InitializeComponent();
        }

        private void CmbFileTypeOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (sfOutputPath == null || string.IsNullOrWhiteSpace(sfOutputPath.SelectedFilePath)) return;
            sfOutputPath.SelectedFilePath = Path.ChangeExtension(sfOutputPath.SelectedFilePath, (string) cmbFileType.SelectedItem);
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

        private void SelectFile_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectFile.SelectedFilePath)) return; 

            var selectFile = (SelectFile) sender;
            var extension = Path.GetExtension(selectFile.SelectedFilePath);
            extension = ImageFileTypeHelper.FromString(extension).ToString();
            cmbFileType.SelectedItem = extension;
        }
    }
}
