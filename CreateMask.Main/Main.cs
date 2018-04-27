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

            OnOutput(OutputStrings.LoadingFile, arguments.LdrCalibrationFilePath);
            var ldrCalibrationMeasurements = _measurementsLoader.GetFromCsvFile(arguments.LdrCalibrationFilePath);
            
            OnOutput(OutputStrings.ConstructionLdPolynomialCurveFit);
            _maskIntensityInterpolator.LoadMeasurements(ldrCalibrationMeasurements);

            OnOutput(OutputStrings.LoadingFile, arguments.LcdMeasurementsFilePathHigh);
            var measurementsHigh = _measurementGridLoader.GetFromCsvFile(
                arguments.LcdMeasurementsFilePathHigh,
                arguments.MeasurementsNrOfRows,
                arguments.MeasurementsNrOfColumns).GetData();

            OnOutput(OutputStrings.LoadingFile, arguments.LcdMeasurementsFilePathLow);
            var measurementsLow = _measurementGridLoader.GetFromCsvFile(
                arguments.LcdMeasurementsFilePathLow,
                arguments.MeasurementsNrOfRows,
                arguments.MeasurementsNrOfColumns).GetData();

            OnOutput(OutputStrings.ConstructingGridOfLowHighMeasurements);
            var minMaxResistanceGrid = _measurementGridProcessor.CreateMinMaxMeasurementGrid(arguments.Low, arguments.High, measurementsLow, measurementsHigh);
            OnOutput(OutputStrings.CreatingGridOfLocalMaskIntensities);
            var localMaskIntensityGrid = _measurementGridProcessor.CreateLocalMaskIntensityGrid(_maskIntensityInterpolator, minMaxResistanceGrid, arguments.DesiredResistance);
            OnOutput(OutputStrings.ConvertingLocalMaskIntensitiesToBitmap);
            using (var bitmap = _measurementGridProcessor.CreateBitMap(localMaskIntensityGrid))
            {
                OnOutput(OutputStrings.ResizingBitmap, bitmap.Width, bitmap.Height, arguments.LcdWidth, arguments.LcdHeight);
                using (var mask = Image.Resize(bitmap, arguments.LcdWidth, arguments.LcdHeight))
                {
                    mask.Save(arguments.MaskFilePath, imageFormat);
                }
            }
            OnOutput(OutputStrings.MaskSavedTo, arguments.MaskFilePath);

            if (arguments.OriginalExposureTime > 0)
            {
                var exposureTime = _exposureTimeCalculator.CalculateExposure(arguments.High, localMaskIntensityGrid,
                    arguments.OriginalExposureTime);
                OnOutput(OutputStrings.NewAdvisedExposureTime, exposureTime);
            }
        }

        private void OnOutput(string format, params object[] parameters)
        {
            OnOutput(string.Format(format, parameters));
        }

        private void OnOutput(string e)
        {
            Output?.Invoke(this, e);
        }
    }
}
