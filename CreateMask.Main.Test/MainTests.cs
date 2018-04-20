using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Main.Test.Helpers;
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
        //OPTIMIZE: This may actually be no test by itself, the usages of the services should be tested.
        //How that is done is not relevant.
        [Test, Category(Categories.Unit)]
        public void KernelHasAllRequiredBindingsThatCanBeResolved()
        {
            //Given
            var expectedResolvableTypes = new List<Type>
            {
                typeof (IMaskIntensityResistanceInterpolator),
                typeof(IGenericLoader<Measurement>),
                typeof(IGenericGridLoader<int>),
                typeof(IMeasurementGridProcessor)
            };
            var applicationArguments = new ApplicationArguments();

            //When
            var main = new MainTester(applicationArguments);

            //Then
            var error = false;
            foreach (var type in expectedResolvableTypes)
            {
                try
                {
                    main.InternalKernel.Get(type);
                }
                catch (ActivationException ex)
                {
                    error = true;
                    Console.WriteLine($"Could not resolve an instance for type '{type.FullName}' due to an exception:\r\n" +
                                      $"{ex.Message}");
                }
            }
            error.Should().BeFalse();
        }

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
            var main = new MainTester(applicationArguments);

            //When
            main.CreateMask();

            //Then
            using (var actualBitmap = new Bitmap(Image.FromFile(applicationArguments.MaskFilePath)))
            using (var expectedBitmap = new Bitmap(Image.FromFile(FileManager.GetFullFilePath("MaskIsCreatedAsExpected.png"))))
            {
                AssertExt.Equals(expectedBitmap, actualBitmap);
            }
        }
    }
}
