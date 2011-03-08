using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.SpecHelpers
{
    public static class BddExtensions
    {
        public static void ShouldBeEqualTo<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldBeTrue(this bool value)
        {
            ShouldBeEqualTo(value, true);
        }

        public static void ShouldBeFalse(this bool value)
        {
            ShouldBeEqualTo(value, false);
        }

        public static void ShouldNotBeNull(this object item)
        {
            Assert.IsNotNull(item);
        }
        
        public static void ShouldBeAnInstanceOf<Type>(this object Item )
        {
            Assert.IsInstanceOfType(Item, typeof(Type));
        }

        public static ExceptionType ShouldThowAn<ExceptionType>(this Action workForAction)
            where ExceptionType : Exception
        {
            Exception resultingException = GetExceptionForPerformingTheAction(workForAction);
            resultingException.ShouldNotBeNull();
            resultingException.ShouldBeAnInstanceOf<ExceptionType>();
            return (ExceptionType) resultingException;
        }

        private static Exception GetExceptionForPerformingTheAction(Action workForAction)
        {
            try
            {
                workForAction();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}