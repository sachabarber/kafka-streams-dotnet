﻿using kafka_stream_core.SerDes;
using System;
using System.Collections.Generic;

namespace kafka_stream_core.Processors
{
    internal interface IProcessor : ICloneable
    {
        string Name { get; }
        IList<string> StateStores {get;}
        ISerDes Key { get; }
        ISerDes Value { get; }
        void Init(ProcessorContext context);
        // TODO : Maybe not needed !!!!!
        IList<IProcessor> Previous { get; }
        IList<IProcessor> Next { get; }
        void SetPreviousProcessor(IProcessor prev);
        void SetNextProcessor(IProcessor next);
        void Close();
        void Process(object key, object value);
    }

    internal interface IProcessor<K,V> : IProcessor
    {
        void SetProcessorName(string name);
        ISerDes<K> KeySerDes { get; }
        ISerDes<V> ValueSerDes { get; }
        void Process(K key, V value);
        void Forward(K key, V value);
        void Forward(K key, V value, string name);
        void Forward<K1, V1>(K1 key, V1 value);
        void Forward<K1, V1>(K1 key, V1 value, string name);
    }
}
