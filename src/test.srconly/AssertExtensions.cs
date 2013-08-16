//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nuclei.Core.Testing
{
    /// <summary>
    /// Defines additional methods useful during testing.
    /// </summary>
    public static class AssertExtensions
    {
        /// <summary>
        /// Serializes and then deserializes the given instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>A copy of the instance.</returns>
        public static T RoundTripSerialize<T>(T instance)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, instance);
                memoryStream.Position = 0L;
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}
