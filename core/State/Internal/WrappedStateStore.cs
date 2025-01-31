﻿using Streamiz.Kafka.Net.Processors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streamiz.Kafka.Net.State.Internal
{
    internal class WrappedStateStore<S> : IStateStore
        where S : IStateStore
    {

        protected readonly S wrapped;

        public WrappedStateStore(S wrapped)
        {
            this.wrapped = wrapped;
        }

        #region StateStore Impl

        public string Name => wrapped.Name;

        public bool Persistent => wrapped.Persistent;

        public bool IsOpen => wrapped.IsOpen;

        public void Close() => wrapped.Close();

        public void Flush() => wrapped.Flush();

        public virtual void Init(ProcessorContext context, IStateStore root) => wrapped.Init(context, root);

        #endregion
    }
}
