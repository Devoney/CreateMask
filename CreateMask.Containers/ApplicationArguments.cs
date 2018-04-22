using System;

namespace CreateMask.Containers
{
    [Serializable]
    public class ApplicationArguments
    {
        public int DesiredResistance { get; set; }
        public int LcdWidth { get; set; }
        public int LcdHeight { get; set; }
        public string LdrCalibrationFilePath { get; set; }
        public string LcdMeasurementsFilePathHigh { get; set; }
        public string LcdMeasurementsFilePathLow { get; set; }
        public int MeasurementsNrOfRows { get; set; }
        public int MeasurementsNrOfColumns { get; set; }
        public string MaskFilePath { get; set; }
        public string FileType { get; set; }
        public byte High { get; set; }
        public byte Low { get; set; }
    }
}
