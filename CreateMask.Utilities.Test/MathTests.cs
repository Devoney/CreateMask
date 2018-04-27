using System;
using FluentAssertions;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Utilities.Test
{
    [TestFixture]
    public class MathTests
    {
        [Test, Category(Categories.Unit)]
        [TestCase(2)]
        [TestCase(32)]
        public void NumbersAreEvaluatedAsEven(int number)
        {
            //when
            var isEven = number.IsEven();

            //Then
            isEven.Should().BeTrue();
        }

        [Test, Category(Categories.Unit)]
        [TestCase(21)]
        [TestCase(1289)]
        public void NumbersAreEvaluatedAsOdd(int number)
        {
            //when
            var isEven = number.IsEven();

            //Then
            isEven.Should().BeFalse();
        }

        [Test, Category(Categories.Unit)]
        public void TryingToReturnCenterItemsThrowsExceptionOnEmptyArray()
        {
            //Given
            const string expectedExceptionMessage = "No items where present in the given 2D array.";
            var array = new int[0,4];

            //When
            var action = new Action(() =>
            {
                array.GetCenterItems();
            });
            
            //Then
            AssertExt.ThrowsException<ArgumentException>(action, expectedExceptionMessage);
        }

        [Test, Category(Categories.Unit)]
        public void TryingToReturnCenterItemsThrowsExceptionOnNull()
        {
            //Given
            const string expectedExceptionMessage = "An initialized 2D array should have been provided, but null was instead.";
            int[,] array = null;

            //When
            var action = new Action(() =>
            {
                array.GetCenterItems();
            });

            //Then
            AssertExt.ThrowsException<ArgumentNullException>(action, expectedExceptionMessage);
        }

        [Test, Category(Categories.Unit)]
        public void CorrectCenterItemsAreReturnedForEvenRowsAndEvenColumns()
        {
            //Given
            var array = new [,]
            {
                {1, 2, 3, 4 },
                {5, 6, 7, 8 },
                {9, 10, 11, 12 },
                {13, 14, 15, 16 },
                {17, 18, 19, 20 },
                {21, 22, 23, 24 },
            };
            var expectedCenterItems = new[]
            {
                10, 11, 14, 15
            };

            //When
            var actualCenterItems = array.GetCenterItems();

            //Then
            actualCenterItems.Should().BeEquivalentTo(expectedCenterItems);
        }

        [Test, Category(Categories.Unit)]
        public void CorrectCenterItemsAreReturnedForEvenRowsAndOddColumns()
        {
            //Given
            var array = new[,]
            {
                {1, 2, 3, 4, 25},
                {5, 6, 7, 8, 26 },
                {9, 10, 11, 12, 27 },
                {13, 14, 15, 16, 28 },
                {17, 18, 19, 20, 29 },
                {21, 22, 23, 24, 30 }
            };
            var expectedCenterItems = new[]
            {
                11, 15
            };

            //When
            var actualCenterItems = array.GetCenterItems();

            //Then
            actualCenterItems.Should().BeEquivalentTo(expectedCenterItems);
        }

        [Test, Category(Categories.Unit)]
        public void CorrectCenterItemsAreReturnedForOddRowsAndOddColumns()
        {
            //Given
            var array = new[,]
            {
                {1, 2, 3, 4, 25},
                {5, 6, 7, 8, 26 },
                {9, 10, 11, 12, 27 },
                {13, 14, 15, 16, 28 },
                {17, 18, 19, 20, 29 }
            };
            var expectedCenterItems = new[]
            {
                11
            };

            //When
            var actualCenterItems = array.GetCenterItems();

            //Then
            actualCenterItems.Should().BeEquivalentTo(expectedCenterItems);
        }

        [Test, Category(Categories.Unit)]
        public void CorrectCenterItemsAreReturnedForOddRowsAndEvenColumns()
        {
            //Given
            var array = new[,]
            {
                {1, 2, 3, 4},
                {5, 6, 7, 8 },
                {9, 10, 11, 12 },
                {13, 14, 15, 16 },
                {17, 18, 19, 20 }
            };
            var expectedCenterItems = new[]
            {
                10, 11
            };

            //When
            var actualCenterItems = array.GetCenterItems();

            //Then
            actualCenterItems.Should().BeEquivalentTo(expectedCenterItems);
        }
    }
}
