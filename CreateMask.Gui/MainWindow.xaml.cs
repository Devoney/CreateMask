using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using CreateMask.Containers;
using CreateMask.Contracts.Enums;
using CreateMask.Contracts.Helpers;
using CreateMask.Contracts.Interfaces;
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
        private readonly IKernel _kernel;

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
            _kernel = KernelConstructor.GetKernel();
            _main = _kernel.Get<Main.Main>();
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
                FileType = ImageFileType.Png.ToString(),
                OriginalExposureTime = 8000
            };

            InitializeComponent();

            Loaded += (sender, args) => CheckForUpdate();
        }

        private async void CheckForUpdate()
        {
            var releaseManager = _kernel.Get<IReleaseManager>();
            var args = new CheckForReleaseArgs
            {
                CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version,
                OnNewReleaseCallBack = AskToVisiteTheDownloadPage,
                Owner = "Devoney", // TODO: Should be defined somewhere else
                Repository = "CreateMask" // TODO: Should be defined somewhere else
            };
            await releaseManager.CheckForNewReleaseAsync(args);
        }

        private void AskToVisiteTheDownloadPage(ReleaseInfo releaseInfo)
        {
            Dispatcher.Invoke(() =>
            {
                var messageBoxText = $"A newer version is available ({releaseInfo.Version}).\r\nDo you wish to visit the download page?";
                var answer = MessageBox.Show(this, messageBoxText, "New version available",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (answer != MessageBoxResult.Yes) return;

                Process.Start(new ProcessStartInfo(releaseInfo.Uri.AbsoluteUri));
            });
        }

        private void CmbFileTypeOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (string.IsNullOrWhiteSpace(sfOutputPath?.SelectedFilePath)) return;
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
