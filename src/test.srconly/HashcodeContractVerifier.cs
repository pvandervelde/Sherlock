//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace Nuclei.Core.Testing
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public abstract class HashcodeContractVerifier<T>
    {
        public static class Gamma
        {
            // Fields
            private const double Accuracy = 3E-07;
            private const double Epsilon = 1E-30;
            private const int MaxIterations = 0x3e8;

            // Methods
            private static double GammaLogarithm(double a)
            {
                double[] numArray = new double[]
                    {
                        76.180091729471457, 
                        -86.505320329416776, 
                        24.014098240830911, 
                        -1.231739572450155, 
                        0.001208650973866179, 
                        -5.395239384953E-06
                    };
                double num = a;
                double num2 = a;
                double d = num + 5.5;
                d -= (num + 0.5) * Math.Log(d);
                double num4 = 1.0000000001900149;
                for (int i = 0; i < numArray.Length; i++)
                {
                    num4 += numArray[i] / ++num2;
                }

                return -d + Math.Log((2.5066282746310007 * num4) / num);
            }

            public static double IncompleteGamma(double a, double x)
            {
                if (x < 0.0)
                {
                    throw new ArgumentOutOfRangeException("x");
                }

                if (a <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("a");
                }

                if (x < (a + 1.0))
                {
                    return 1.0 - IncompleteGammaSeries(a, x);
                }

                return IncompleteGammaContinuedFraction(a, x);
            }

            private static double IncompleteGammaContinuedFraction(double a, double x)
            {
                double num = (x + 1.0) - a;
                double num2 = 9.9999999999999988E+29;
                double num3 = 1.0 / num;
                double num4 = num3;
                for (int i = 1; i < 0x3e8; i++)
                {
                    double num6 = -i * (i - a);
                    num += 2.0;
                    num3 = 1.0 / Math.Max((double)1E-30, (double)((num6 * num3) + num));
                    num2 = Math.Max((double)1E-30, (double)(num + (num6 / num2)));
                    double num7 = num3 * num2;
                    num4 *= num7;
                    if (Math.Abs((double)(num7 - 1.0)) < 3E-07)
                    {
                        return Math.Exp((-x + (a * Math.Log(x))) - GammaLogarithm(a)) * num4;
                    }
                }

                throw new ArgumentOutOfRangeException("a", "Value too large.");
            }

            private static double IncompleteGammaSeries(double a, double x)
            {
                if (x < 0.0)
                {
                    throw new ArgumentOutOfRangeException("x");
                }

                double num = a;
                double num2 = 1.0 / a;
                double num3 = num2;
                for (int i = 1; i < 0x3e8; i++)
                {
                    num++;
                    num2 *= x / num;
                    num3 += num2;
                    if (Math.Abs(num2) < (Math.Abs(num3) * 3E-07))
                    {
                        return num3 * Math.Exp((-x + (a * Math.Log(x))) - GammaLogarithm(a));
                    }
                }

                throw new ArgumentOutOfRangeException("a", "Value too large.");
            }
        }

        public static class CollisionProbability
        {
            // Fields
            public static readonly double High = 0.3;
            public static readonly double Low = 0.05;
            public static readonly double Medium = 0.1;
            public static readonly double Perfect = 0.0;
            public static readonly double VeryLow = 0.01;
        }

        public static class UniformDistributionQuality
        {
            // Fields
            public static readonly double Excellent = 0.01;
            public static readonly double Fair = 0.1;
            public static readonly double Good = 0.05;
            public static readonly double Mediocre = 0.3;
        }

        public sealed class HashStoreResult
        {
            private readonly double m_CollisionProbability;
            private readonly int m_StatisticalPopulation;
            private readonly double m_UniformDistributionDeviationProbability;

            public HashStoreResult(int statisticalPopulation, double collisionProbability, double uniformDistributionDeviationProbability)
            {
                this.m_StatisticalPopulation = statisticalPopulation;
                this.m_CollisionProbability = collisionProbability;
                this.m_UniformDistributionDeviationProbability = uniformDistributionDeviationProbability;
            }

            public double CollisionProbability
            {
                get
                {
                    return this.m_CollisionProbability;
                }
            }

            public int StatisticalPopulation
            {
                get
                {
                    return this.m_StatisticalPopulation;
                }
            }

            public double UniformDistributionDeviationProbability
            {
                get
                {
                    return this.m_UniformDistributionDeviationProbability;
                }
            }
        }

        public class NotEnoughHashesException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
            /// </summary>
            public NotEnoughHashesException()
                : this("Not enough hashes.")
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            public NotEnoughHashesException(string message)
                : base(message)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
            /// </summary>
            /// <param name="expected">The expected number of hashes.</param>
            /// <param name="actual">The actual number of hashes.</param>
            public NotEnoughHashesException(int expected, int actual)
                : this(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Expected at least {0} hashes. Got {1} hashes.",
                        expected,
                        actual))
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="innerException">The inner exception.</param>
            public NotEnoughHashesException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
            /// </summary>
            /// <param name="info">
            ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized 
            ///     object data about the exception being thrown.
            /// </param>
            /// <param name="context">
            ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual
            ///     information about the source or destination.
            /// </param>
            /// <exception cref="T:System.ArgumentNullException">
            /// The <paramref name="info"/> parameter is null.
            /// </exception>
            /// <exception cref="T:System.Runtime.Serialization.SerializationException">
            /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
            /// </exception>
            private NotEnoughHashesException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }

        public class HashStore
        {
            // Fields
            private const int MinimumHashCount = 2;
            private readonly IDictionary<int, int> m_More = new Dictionary<int, int>();
            private readonly HashSet<int> m_One = new HashSet<int>();
            private readonly HashStoreResult m_Result;
            private readonly HashSet<int> m_Two = new HashSet<int>();

            // Methods
            public HashStore(IEnumerable<int> hashes)
            {
                int actual = 0;
                foreach (int num2 in hashes)
                {
                    this.Add(num2);
                    actual++;
                }

                if (actual < 2)
                {
                    throw new NotEnoughHashesException(2, actual);
                }

                this.m_Result = this.CalculateResults(actual);
            }

            private void Add(int hash)
            {
                if (this.m_One.Contains(hash))
                {
                    this.m_One.Remove(hash);
                    this.m_Two.Add(hash);
                }
                else if (this.m_Two.Contains(hash))
                {
                    this.m_Two.Remove(hash);
                    this.m_More.Add(hash, 3);
                }
                else
                {
                    int num;
                    if (this.m_More.TryGetValue(hash, out num))
                    {
                        this.m_More[hash] = 1 + num;
                    }
                    else
                    {
                        this.m_One.Add(hash);
                    }
                }
            }

            private HashStoreResult CalculateResults(int count)
            {
                int bucketSize = this.GetBucketSize();
                double[] actual = new double[bucketSize];
                double collisionProbability = 0.0;
                int num3 = 0;
                for (int i = 0; i < this.m_One.Count; i++)
                {
                    actual[num3++ % bucketSize]++;
                }

                for (int j = 0; j < this.m_Two.Count; j++)
                {
                    actual[num3++ % bucketSize] += 2.0;
                    collisionProbability += 2.0 / ((double)(count * (count - 1)));
                }

                foreach (KeyValuePair<int, int> pair in this.m_More)
                {
                    actual[num3++ % bucketSize] += (double)pair.Value;
                    collisionProbability += ((((double)pair.Value) / ((double)count)) * ((double)(pair.Value - 1))) / ((double)(count - 1));
                }

                ChiSquareTest test = new ChiSquareTest(((double)count) / ((double)bucketSize), actual, 1);
                return new HashStoreResult(count, collisionProbability, 1.0 - test.TwoTailedpValue);
            }

            private int GetBucketSize()
            {
                int[] numArray = new int[] { 0x39dd, 0xe1d, 0xdf, 0x11 };
                int num = (this.m_One.Count + this.m_Two.Count) + this.m_More.Count;
                foreach (int num2 in numArray)
                {
                    if (num >= (num2 * 10))
                    {
                        return num2;
                    }
                }

                return num;
            }

            // Properties
            internal int this[int hash]
            {
                get
                {
                    int num;
                    if (this.m_One.Contains(hash))
                    {
                        return 1;
                    }

                    if (this.m_Two.Contains(hash))
                    {
                        return 2;
                    }

                    if (!this.m_More.TryGetValue(hash, out num))
                    {
                        return 0;
                    }

                    return num;
                }
            }

            public HashStoreResult Result
            {
                get
                {
                    return this.m_Result;
                }
            }
        }

        public class ChiSquareTest
        {
            // Fields
            private readonly double m_ChiSquareValue;
            private readonly int m_DegreesOfFreedom;
            private readonly double m_TwoTailedpValue;

            // Methods
            public ChiSquareTest(double expected, ICollection<double> actual, int numberOfConstraints)
            {
                if (expected <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("The expected value is negative.", "expected");
                }

                if (actual == null)
                {
                    throw new ArgumentNullException("actual");
                }

                this.m_DegreesOfFreedom = actual.Count - numberOfConstraints;
                foreach (double num in actual)
                {
                    double num2 = num - expected;
                    this.m_ChiSquareValue += (num2 * num2) / expected;
                }

                this.m_TwoTailedpValue = Gamma.IncompleteGamma(((double)this.m_DegreesOfFreedom) / 2.0, this.m_ChiSquareValue / 2.0);
            }

            // Properties
            public double ChiSquareValue
            {
                get
                {
                    return this.m_ChiSquareValue;
                }
            }

            public int DegreesOfFreedom
            {
                get
                {
                    return this.m_DegreesOfFreedom;
                }
            }

            public double TwoTailedpValue
            {
                get
                {
                    return this.m_TwoTailedpValue;
                }
            }
        }

        public abstract double CollisionProbabilityLimit
        {
            get;
        }

        public abstract double UniformDistributionQualityLimit
        {
            get;
        }

        public abstract IEnumerable<T> DistinctInstances
        {
            get;
        }

        [Test]
        public void VerifyCollisionProbability()
        {
            HashStoreResult result = null;
            try
            {
                result = GetResult();
            }
            catch (NotEnoughHashesException)
            {
                Assert.Fail("Not enough hash code samples were provided to the hash code acceptance contract.");
            }

            double collisionProbability = result.CollisionProbability;
            Assert.LessOrEqual(collisionProbability, CollisionProbabilityLimit);
        }

        private HashStoreResult GetResult()
        {
            var store = new HashStore(DistinctInstances.Select(o => o.GetHashCode()));
            return store.Result;
        }

        [Test]
        public void VerifyUniformDistribution()
        {
            HashStoreResult result = null;
            try
            {
                result = GetResult();
            }
            catch (NotEnoughHashesException)
            {
                Assert.Fail("Not enough hash code samples were provided to the hash code acceptance contract.");
            }

            double uniformDistributionDeviationProbability = result.UniformDistributionDeviationProbability;
            Assert.LessOrEqual(uniformDistributionDeviationProbability, UniformDistributionQualityLimit);
        }
    }
}
