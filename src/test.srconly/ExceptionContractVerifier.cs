//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace Nuclei.Core.Testing
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public abstract class ExceptionContractVerifier<TException> where TException : Exception
    {
        [Test]
        public void HasSerializationAttribute()
        {
            Assert.True(typeof(TException).IsDefined(typeof(SerializableAttribute), false));
        }

        [Test]
        public void HasDefaultConstructor()
        {
            var constructor = typeof(TException).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new Type[0],
                null);
            Assert.NotNull(constructor);
        }

        [Test]
        public void HasMessageConstructor()
        {
            var constructor = typeof(TException).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[]
                    {
                        typeof(string)
                    },
                null);
            Assert.NotNull(constructor);

            var text = "a";
            var instance = (TException)constructor.Invoke(new object[] { text });
            Assert.AreEqual(text, instance.Message);
        }

        [Test]
        public void HasMessageAndExceptionConstructor()
        {
            var constructor = typeof(TException).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[]
                    {
                        typeof(string),
                        typeof(Exception)
                    },
                null);
            Assert.NotNull(constructor);

            var text = "a";
            var inner = new Exception();
            var instance = (TException)constructor.Invoke(
                new object[] 
                { 
                    text,
                    inner
                });
            Assert.AreEqual(text, instance.Message);
            Assert.AreSame(inner, instance.InnerException);
        }

        [Test]
        public void HasSerializationConstructor()
        {
            var constructor = typeof(TException).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[]
                    {
                      typeof(SerializationInfo),
                      typeof(StreamingContext)
                    },
                null);
            Assert.NotNull(constructor);
        }

        [Test]
        public void RoundTripSerializeAndDeserialize()
        {
            var constructor = typeof(TException).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[]
                    {
                        typeof(string),
                        typeof(Exception)
                    },
                null);
            Assert.NotNull(constructor);

            var text = "a";
            var inner = new Exception();
            var instance = (TException)constructor.Invoke(
                new object[] 
                { 
                    text,
                    inner
                });
            var copy = AssertExtensions.RoundTripSerialize(instance);

            Assert.AreEqual(instance.Message, copy.Message);
            Assert.AreEqual(instance.InnerException.GetType(), copy.InnerException.GetType());
        }
    }
}
