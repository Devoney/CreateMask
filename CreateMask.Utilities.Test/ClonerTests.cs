using System;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using NUnit.Framework;

namespace CreateMask.Utilities.Test
{
    [TestFixture]
    public class ClonerTests
    {
        [Test]
        public void ObjectReferencesAreNotEqualAfterCloning()
        {
            //Given
            var cloner = GetCloner();
            var source = new object();

            //When
            var clone = cloner.DeepClone(source);

            //Then
            ReferenceEquals(source, clone).Should().BeFalse();
        }

        [Test]
        public void ObjectIsClonedCorrectly()
        {
            //Given
            var cloner = GetCloner();
            var source = new Dummy
            {
                Name = "Test name",
                Age = 15,
                LaunchDate = DateTime.Now
            };

            //When
            var clone = cloner.DeepClone(source);

            //Then
            clone.Should().BeEquivalentTo(source);
        }

        #region Helpers
        private ICloner GetCloner()
        {
            return new Cloner();
        }

        [Serializable]
        public class Dummy
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime LaunchDate { get; set; }
        }
        #endregion
    }
}
