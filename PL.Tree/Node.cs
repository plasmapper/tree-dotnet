using System;
using System.Collections.Generic;

namespace PL.Tree
{
    /// <summary>
    /// Base class for all nodes.
    /// </summary>
    public abstract class Node
    {
        private Node _parent = null;

        public Node() { }

        internal Node Parent
        {
            get
            {
                lock (this)
                    return _parent;
            }
            set
            {
                lock (this)
                    _parent = value;
            }
        }

        /// <summary>
        /// Occurs when node value has been changed.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Raises the ValueChanged event of this node and parent nodes. Catches all exceptions and throws as AggregateException.
        /// </summary>
        /// <exception cref="AggregateException"></exception>
        public virtual void OnValueChanged()
        {
            List<Exception> exceptions = new List<Exception>();
            if (ValueChanged != null)
            {
                foreach (EventHandler handler in ValueChanged.GetInvocationList())
                {
                    try
                    {
                        handler.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception exception)
                    {
                        exceptions.Add(exception);
                    }
                }
            }
            try
            {
                Parent?.OnValueChanged();
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        /// <summary>
        /// Gets node value.
        /// </summary>
        /// <returns>Node value.</returns>
        public abstract object GetValue();

        /// <summary>
        /// Sets node value.
        /// </summary>
        /// <param name="value">New node value.</param>
        public abstract void SetValue(object value);
    }
}
