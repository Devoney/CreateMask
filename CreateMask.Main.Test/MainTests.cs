using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using Ninject;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Main.Test
{
    /// <summary>
    /// Summary description for MainTests
    /// </summary>
    [TestFixture]
    public class MainTests
    {
        [Test, Category(Categories.Integration)]
        public void MaskIsCreatedAsExpected()
        {
            //Given
            var applicationArguments = new ApplicationArguments();
            applicationArguments.DesiredResistance = 8820;
            applicationArguments.High = 255;
            applicationArguments.LcdHeight = 1440;
            applicationArguments.LcdMeasurementsFilePathHigh = FileManager.GetFullFilePath("high.csv");
            applicationArguments.LcdMeasurementsFilePathLow = FileManager.GetFullFilePath("low.csv");
            applicationArguments.LcdWidth = 2560;
            applicationArguments.LdrCalibrationFilePath = FileManager.GetFullFilePath("ldrcurve.csv");
            applicationArguments.Low = 175;
            applicationArguments.MaskFilePath = Path.GetFullPath("./mask.png");
            applicationArguments.MeasurementsNrOfColumns = 12;
            applicationArguments.MeasurementsNrOfRows = 7;
            applicationArguments.FileType = ".png";
            var main = KernelConstructor.GetKernel().Get<Main>();

            //When
            main.CreateMask(applicationArguments);

            //Then
            using (var actualBitmap = new Bitmap(Image.FromFile(applicationArguments.MaskFilePath)))
            using (var expectedBitmap = new Bitmap(Image.FromFile(FileManager.GetFullFilePath("MaskIsCreatedAsExpected.png"))))
            {
                AssertExt.Equals(expectedBitmap, actualBitmap);
            }
        }
    }
}
