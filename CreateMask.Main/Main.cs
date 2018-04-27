using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using CreateMask.Containers;
using CreateMask.Contracts.Helpers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Utilities;

namespace CreateMask.Main
{
    public class Main
    {
        public event EventHandler<string> Output;

        private readonly IGenericLoader<Measurement> _measurementsLoader;
        private readonly IMaskIntensityResistanceInterpolator _maskIntensityInterpolator;
        private readonly IGenericGridLoader<int> _measurementGridLoader;
        private readonly IMeasurementGridProcessor _measurementGridProcessor;
        private readonly IExposureTimeCalculator _exposureTimeCalculator;

        public IEnumerable<string> SupportedFileTypes => ImageFileTypeHelper.ImageFileTypes;

        public Main(IGenericLoader<Measurement> measurementsLoader, 
                    IMaskIntensityResistanceInterpolator maskIntensityInterpolator,
                    IGenericGridLoader<int> measurementGridLoader,
                    IMeasurementGridProcessor measurementGridProcessor,
                    IExposureTimeCalculator exposureTimeCalculator)
        {
            _measurementsLoader = measurementsLoader;
            _maskIntensityInterpolator = maskIntensityInterpolator;
            _measurementGridLoader = measurementGridLoader;
            _measurementGridProcessor = measurementGridProcessor;
            _exposureTimeCalculator = exposureTimeCalculator;
        }

        public void CreateMask(ApplicationArguments arguments)
        {
            var imageFormat = ImageFormat.Png;
            if (!string.IsNullOrEmpty(arguments.FileType))
            {
                imageFormat = ImageFileTypeHelper.FromString(arguments.FileType).ToImageFormat();
            }

            OnOutput($"Loading {arguments.LdrCalibrationFilePath}");
            var ldrCalibrationMeasurements = _measurementsLoader.GetFromCsvFile(arguments.LdrCalibrationFilePath);
            
            OnOutput("Constructing LDR polynomial curve fit.");
            _maskIntensityInterpolator.LoadMeasurements(ldrCalibrationMeasurements);

            OnOutput($"Loading { arguments.LcdMeasurementsFilePathHigh}");
            var measurementsHigh = _measurementGridLoader.GetFromCsvFile(
                arguments.LcdMeasurementsFilePathHigh,
                arguments.MeasurementsNrOfRows,
                arguments.MeasurementsNrOfColumns).GetData();

            OnOutput($"Loading { arguments.LcdMeasurementsFilePathLow}");
            var measurementsLow = _measurementGridLoader.GetFromCsvFile(
                arguments.LcdMeasurementsFilePathLow,
                arguments.MeasurementsNrOfRows,
                arguments.MeasurementsNrOfColumns).GetData();

            OnOutput("Constructing grid of low/high measurements.");
            var minMaxResistanceGrid = _measurementGridProcessor.CreateMinMaxMeasurementGrid(arguments.Low, arguments.High, measurementsLow, measurementsHigh);
            OnOutput("Creating grid of local mask intensities.");
            var localMaskIntensityGrid = _measurementGridProcessor.CreateLocalMaskIntensityGrid(_maskIntensityInterpolator, minMaxResistanceGrid, arguments.DesiredResistance);
            OnOutput("Converting grid of local mask intensities to bitmap.");
            using (var bitmap = _measurementGridProcessor.CreateBitMap(localMaskIntensityGrid))
            {
                OnOutput($"Resizing bitmap of {bitmap.Width}x{bitmap.Height}px to final mask of {arguments.LcdWidth}x{arguments.LcdHeight}px using bilinear interpolation.");
                using (var mask = Image.Resize(bitmap, arguments.LcdWidth, arguments.LcdHeight))
                {
                    mask.Save(arguments.MaskFilePath, imageFormat);
                }
            }
            OnOutput($"Mask saved to {arguments.MaskFilePath}.");
        }

        private void OnOutput(string e)
        {
            Output?.Invoke(this, e);
        }
    }
}
