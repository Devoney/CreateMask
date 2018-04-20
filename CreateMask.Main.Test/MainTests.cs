using System;
using System.Collections.Generic;
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
            var main = new MainTester(applicationArguments);

            //When
            main.CreateMask();

            //Then
            Assert.Fail("TODO");
        }
    }
}
