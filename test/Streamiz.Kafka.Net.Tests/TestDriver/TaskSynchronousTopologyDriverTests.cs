﻿using NUnit.Framework;
using Streamiz.Kafka.Net.Errors;
using Streamiz.Kafka.Net.Mock;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.State;
using Streamiz.Kafka.Net.Table;
using System.Linq;

namespace Streamiz.Kafka.Net.Tests.TestDriver
{
    public class TaskSynchronousTopologyDriverTests
    {
        [Test]
        public void GetTaskWithUnknownTopic()
        {
            var config = new StreamConfig<StringSerDes, StringSerDes>();
            config.ApplicationId = "test-config";

            var builder = new StreamBuilder();
            builder.Stream<string, string>("test").To("test2");
            
            var driver = new TaskSynchronousTopologyDriver("client", builder.Build().Builder, config, config, default);
            var task = driver.GetTask("test");
            // Create the task
            Assert.IsNotNull(task);
            // Topic doesn't not exist in topology
            Assert.Throws<TopologyException>(() => driver.GetTask("topicnotexists"));
            // Same task, so task2 == task
            var task2 = driver.GetTask("test");
            Assert.IsTrue(task == task2);
            driver.Dispose();
        }

        [Test]
        public void GetStateStoreNotExist()
        {
            var config = new StreamConfig<StringSerDes, StringSerDes>();
            config.ApplicationId = "test-config";

            var builder = new StreamBuilder();
            builder.Stream<string, string>("test").To("test2");

            var driver = new TaskSynchronousTopologyDriver("client", builder.Build().Builder, config, config, default);
            driver.CreateInputTopic("test", new StringSerDes(), new StringSerDes());
            var store = driver.GetStateStore<string, string>("store");
            Assert.IsNull(store);
            driver.Dispose();
        }

        [Test]
        public void GetStateStoreExist()
        {
            var config = new StreamConfig<StringSerDes, StringSerDes>();
            config.ApplicationId = "test-config";

            var builder = new StreamBuilder();
            builder.Table<string, string>("test", InMemory<string, string>.As("store"));

            var driver = new TaskSynchronousTopologyDriver("client", builder.Build().Builder, config, config, default);
            var input = driver.CreateInputTopic("test", new StringSerDes(), new StringSerDes());
            var store = driver.GetStateStore<string, string>("store");
            Assert.IsNotNull(store);
            Assert.IsInstanceOf<ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>>(store);
            input.PipeInput("coucou", "1");

            Assert.AreEqual(1, ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).All().Count());
            Assert.AreEqual("1", ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).Get("coucou").Value);
            driver.Dispose();
        }

        [Test]
        public void StateStoreUpdateKey()
        {
            var config = new StreamConfig<StringSerDes, StringSerDes>();
            config.ApplicationId = "test-config";

            var builder = new StreamBuilder();
            builder.Table<string, string>("test", InMemory<string, string>.As("store"));

            var driver = new TaskSynchronousTopologyDriver("client", builder.Build().Builder, config, config, default);
            var input = driver.CreateInputTopic("test", new StringSerDes(), new StringSerDes());
            var store = driver.GetStateStore<string, string>("store");
            Assert.IsNotNull(store);
            Assert.IsInstanceOf<ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>>(store);
            input.PipeInput("coucou", "1");

            Assert.AreEqual(1, ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).All().Count());
            Assert.AreEqual("1", ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).Get("coucou").Value);

            input.PipeInput("coucou", "2");

            Assert.AreEqual(1, ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).All().Count());
            Assert.AreEqual("2", ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).Get("coucou").Value);
            driver.Dispose();
        }

        [Test]
        public void StateStoreDeleteKey()
        {
            var config = new StreamConfig<StringSerDes, StringSerDes>();
            config.ApplicationId = "test-config";

            var builder = new StreamBuilder();
            builder.Table<string, string>("test", InMemory<string, string>.As("store"));

            var driver = new TaskSynchronousTopologyDriver("client", builder.Build().Builder, config, config, default);
            var input = driver.CreateInputTopic("test", new StringSerDes(), new StringSerDes());
            var store = driver.GetStateStore<string, string>("store");
            Assert.IsNotNull(store);
            Assert.IsInstanceOf<ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>>(store);
            input.PipeInput("coucou", "1");

            Assert.AreEqual(1, ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).All().Count());
            Assert.AreEqual("1", ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).Get("coucou").Value);

            input.PipeInput("coucou", null);

            Assert.AreEqual(0, ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).All().Count());
            Assert.AreEqual(null, ((ReadOnlyKeyValueStore<string, ValueAndTimestamp<string>>)store).Get("coucou"));
            driver.Dispose();
        }
    }
}
