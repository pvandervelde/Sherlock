//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NUnit.Framework;

namespace Nuclei.Core.Testing
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public abstract class EqualityContractVerifier<T> : HashcodeContractVerifier<T>
    {
        public abstract bool HasOperatorOverloads
        {
            get;
        }

        public abstract T FirstInstance
        {
            get;
        }

        public abstract T SecondInstance
        {
            get;
        }

        public abstract T Copy(T original);

        [Test]
        public void ObjectEquals()
        {
            object left = FirstInstance;
            object right = Copy(FirstInstance);

            Assert.IsTrue(left.Equals(right));
        }

        [Test]
        public void ObjectEqualsWithNullObject()
        {
            object left = FirstInstance;
            Assert.IsFalse(left.Equals(null));
        }

        [Test]
        public void ObjectEqualsWithNonEqualObjectOfSameType()
        {
            object left = FirstInstance;
            object right = SecondInstance;

            Assert.IsFalse(left.Equals(right));
        }

        [Test]
        public void ObjectEqualsWithNonEqualObjectOfDifferentType()
        {
            object left = FirstInstance;
            Assert.IsFalse(left.Equals(new object()));
        }

        [Test]
        public void EquatableEquals()
        {
            if (!typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
            {
                Assert.Ignore("IEquatable<T> not implemented.");
                return;
            }

            var left = (IEquatable<T>)FirstInstance;
            var right = Copy(FirstInstance);

            Assert.IsTrue(left.Equals(right));
        }

        [Test]
        public void EquatableEqualsWithNonEqualObject()
        {
            if (!typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
            {
                Assert.Ignore("IEquatable<T> not implemented.");
                return;
            }

            var left = (IEquatable<T>)FirstInstance;
            var right = SecondInstance;

            Assert.IsFalse(left.Equals(right));
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;
            var right = Copy(FirstInstance);

            var method = typeof(T).GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        right
                    });

            Assert.IsTrue((bool)result);
        }

        [Test]
        public void EqualsOperatorWithNonEqualObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;
            var right = SecondInstance;

            var method = typeof(T).GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        right
                    });

            Assert.IsFalse((bool)result);
        }

        [Test]
        public void EqualsOperatorWithLeftNullObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var instance = FirstInstance;

            var method = typeof(T).GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        null,
                        instance
                    });

            Assert.IsFalse((bool)result);
        }

        [Test]
        public void EqualsOperatorWithRightNullObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var instance = SecondInstance;

            var method = typeof(T).GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        instance,
                        null
                    });

            Assert.IsFalse((bool)result);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;
            var right = Copy(FirstInstance);

            var method = typeof(T).GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        right
                    });

            Assert.IsFalse((bool)result);
        }

        [Test]
        public void NotEqualsOperatorWithNonEqualObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;
            var right = SecondInstance;

            var method = typeof(T).GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        right
                    });

            Assert.IsTrue((bool)result);
        }

        [Test]
        public void NotEqualsOperatorWithLeftNullObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;

            var method = typeof(T).GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        null
                    });

            Assert.IsTrue((bool)result);
        }

        [Test]
        public void NotEqualsOperatorWithRightNullObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var right = SecondInstance;

            var method = typeof(T).GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        null,
                        right
                    });

            Assert.IsTrue((bool)result);
        }

        [Test]
        public void HashcodeComparisonForEqualObjects()
        {
            object left = FirstInstance;
            object right = Copy(FirstInstance);

            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        }
    }
}
