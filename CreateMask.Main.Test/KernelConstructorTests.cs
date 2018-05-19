using System;
using System.Collections.Generic;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using Ninject;
using NUnit.Framework;
using Octokit;
using TestHelpers;

namespace CreateMask.Main.Test
{
    [TestFixture]
    public class KernelConstructorTests
    {
        [Test, Category(Categories.Unit)]
        public void KernelHasAllRequiredBindingsThatCanBeResolved()
        {
            //Given
            var expectedResolvableTypes = new List<Type>
            {
                typeof(IArgumentsParser),
                typeof(IBitmapProcessor),
                typeof(IDateTimeWorker),
                typeof(IErrorReportCreator),
                typeof(IExposureTimeCalculator),
                typeof(IGenericGridLoader<int>),
                typeof(IGenericLoader<Measurement>),
                typeof(IMaskIntensityResistanceInterpolatorFactory),
                typeof(IMeasurementGridProcessor),
                typeof(IOutputWriter),
                typeof(IReleaseManager),
                typeof(IReleasesClient)
            };

            //When
            var kernel = KernelConstructor.GetKernel();

            //Then
            var error = false;
            foreach (var type in expectedResolvableTypes)
            {
                try
                {
                    kernel.Get(type);
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
    }
}
