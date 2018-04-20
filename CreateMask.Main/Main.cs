using System;
using System.Drawing.Imaging;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Ninject;
using Image = CreateMask.Utilities.Image;

namespace CreateMask.Main
{
    public class Main
    {
        public event EventHandler<string> Output;

        private readonly ApplicationArguments _arguments;
        protected IKernel Kernel;

        public Main(ApplicationArguments arguments)
        {
            _arguments = arguments;
            CreateKernel();
        }

        public void CreateMask()
        {
            var measurementsLoader = Kernel.Get<IGenericLoader<Measurement>>();
            OnOutput($"Loading {_arguments.LdrCalibrationFilePath}");
            var ldrCalibrationMeasurements = measurementsLoader.GetFromCsvFile(_arguments.LdrCalibrationFilePath);

            var maskIntensityInterpolator = Kernel.Get<IMaskIntensityResistanceInterpolator>();
            OnOutput("Constructing LDR polynomial curve fit.");
            maskIntensityInterpolator.LoadMeasurements(ldrCalibrationMeasurements);

            OnOutput($"Loading { _arguments.LcdMeasurementsFilePathHigh}");
            var gridMeasurementsLoader = Kernel.Get<IGenericGridLoader<int>>();
            var measurementsHigh = gridMeasurementsLoader.GetFromCsvFile(
                _arguments.LcdMeasurementsFilePathHigh,
                _arguments.MeasurementsNrOfRows,
                _arguments.MeasurementsNrOfColumns).GetData();

            OnOutput($"Loading { _arguments.LcdMeasurementsFilePathLow}");
            var measurementsLow = gridMeasurementsLoader.GetFromCsvFile(
                _arguments.LcdMeasurementsFilePathLow,
                _arguments.MeasurementsNrOfRows,
                _arguments.MeasurementsNrOfColumns).GetData();

            var measurementGridProcessor = Kernel.Get<IMeasurementGridProcessor>();
            OnOutput("Constructing grid of low/high measurements.");
            var minMaxResistanceGrid = measurementGridProcessor.CreateMinMaxMeasurementGrid(_arguments.Low, _arguments.High, measurementsLow, measurementsHigh);
            OnOutput("Creating grid of local mask intensities.");
            var localMaskIntensityGrid = measurementGridProcessor.CreateLocalMaskIntensityGrid(maskIntensityInterpolator, minMaxResistanceGrid, _arguments.DesiredResistance);
            OnOutput("Converting grid of local mask intensities to bitmap.");
            using (var bitmap = measurementGridProcessor.CreateBitMap(localMaskIntensityGrid))
            {
                OnOutput($"Resizing bitmap of {bitmap.Width}x{bitmap.Height}px to final mask of {_arguments.LcdWidth}x{_arguments.LcdHeight}px using bilinear interpolation.");
                using (var mask = Image.Resize(bitmap, _arguments.LcdWidth, _arguments.LcdHeight))
                {
                    mask.Save(_arguments.MaskFilePath, ImageFormat.Png);
                }
            }
            OnOutput($"Mask saved to {_arguments.MaskFilePath}.");
        }

        private void CreateKernel()
        {
            Kernel = new StandardKernel();
            KernelRegistrator.Register(Kernel);
        }

        protected virtual void OnOutput(string e)
        {
            Output?.Invoke(this, e);
        }
    }
}
