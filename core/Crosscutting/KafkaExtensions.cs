﻿using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Streamiz.Kafka.Net.Crosscutting
{
    internal static class KafkaExtensions
    {
        internal static List<WatermarkOffsetsByTopicPartition> GetWatermarkOffsets<K, V>(this IConsumer<K, V> consumer)
        {
            List<WatermarkOffsetsByTopicPartition> l = new List<WatermarkOffsetsByTopicPartition>();
            foreach (var p in consumer.Assignment)
            {
                var w = consumer.GetWatermarkOffsets(p);
                l.Add(new WatermarkOffsetsByTopicPartition(p.Topic, p.Partition, w.Low, w.High));
            }
            return l;
        }

        internal static IEnumerable<PartitionMetadata> PartitionsForTopic(this Metadata clusterMetadata, string topic)
        {
            if (clusterMetadata.Topics.Any(t => t.Topic.Equals(topic)))
            {
                return clusterMetadata.Topics.Find(t => t.Topic.Equals(topic)).Partitions;
            }
            else
                return new List<PartitionMetadata>();
        }

        internal static IEnumerable<ConsumeResult<K, V>> ConsumeRecords<K, V>(this IConsumer<K, V> consumer, TimeSpan timeout)
        {
            List<ConsumeResult<K, V>> records = new List<ConsumeResult<K, V>>();
            DateTime dt = DateTime.Now;
            while (dt.Add(timeout) > DateTime.Now)
            {
                var r = consumer.Consume(TimeSpan.Zero);
                if (r != null)
                    records.Add(r);
                else
                    return records;
            }
            return records;
        }
    }
}
