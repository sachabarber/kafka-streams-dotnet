﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Streamiz.Kafka.Net.SerDes
{
    /// <summary>
    /// The interface (NOT TYPED) for wrapping a serializer and deserializer for object.
    /// A class that implements this interface is expected to have a constructor with no parameter.
    /// <para>
    /// If you know the type of your data, please use <see cref="ISerDes{T}"/>.
    /// </para>
    /// </summary>
    public interface ISerDes
    {
        /// <summary>
        /// Deserialize a record value from a byte array into an object.
        /// </summary>
        /// <param name="data">serialized bytes; may be null; implementations are recommended to handle null by returning a value or null rather than throwing an exception.</param>
        /// <returns>deserialized object data; may be null</returns>
        object DeserializeObject(byte[] data);

        /// <summary>
        /// Convert <code>data</code> into a byte array.
        /// </summary>
        /// <param name="data">object data</param>
        /// <returns>serialized bytes</returns>
        byte[] SerializeObject(object data);
    }


    /// <summary>
    /// The interface for wrapping a serializer and deserializer for the given data type.
    /// A class that implements this interface is expected to have a constructor with no parameter.
    /// </summary>
    /// <typeparam name="T">Type to be serialized from and deserialized into.</typeparam>
    public interface ISerDes<T> : ISerDes
    {
        /// <summary>
        /// Deserialize a record value from a byte array into a value or object.
        /// </summary>
        /// <param name="data">serialized bytes; may be null; implementations are recommended to handle null by returning a value or null rather than throwing an exception.</param>
        /// <returns>deserialized typed data; may be null</returns>
        T Deserialize(byte[] data);

        /// <summary>
        /// Convert <code>data</code> into a byte array.
        /// </summary>
        /// <param name="data">typed data</param>
        /// <returns>serialized bytes</returns>
        byte[] Serialize(T data);
    }
}