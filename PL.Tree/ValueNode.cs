using System;

namespace PL.Tree
{
    /// <summary>
    /// Represents node with typed value. 
    /// </summary>
    /// <typeparam name="T">Node value type.</typeparam>
    public class ValueNode<T> : Node
    {
        private T _value;

        /// <summary>
        /// Initializes a new instance of the ValueNode class.
        /// </summary>
        /// <param name="value">Node value.</param>
        public ValueNode(T value = default) => _value = value;

        /// <summary>
        /// Get: locks node and gets value.
        /// <para>Set: locks node, checks if new value is different, raises ValueValidating event, sets value and raises ValueChange event.</para>
        /// </summary>
        public T Value
        {
            get
            {
                lock (this)
                    return _value;
            }
            set
            {
                lock (this)
                {
                    if (Equals(value, _value))
                        return;
                }
                value = Validate(value);
                lock (this)
                    _value = value;
                OnValueChanged();
            }
        }

        /// <summary>
        /// Occurs when node value is validating.
        /// </summary>
        public event EventHandler<ValueValidatingEventArgs<T>> ValueValidating;

        public override object GetValue() => Value;

        public override void SetValue(object value) => Value = (T)value;

        private T Validate(T value)
        {
            ValueValidatingEventArgs<T> eventArgs = new ValueValidatingEventArgs<T>(value);
            if (ValueValidating != null)
            {
                foreach (EventHandler<ValueValidatingEventArgs<T>> handler in ValueValidating.GetInvocationList())
                    handler.Invoke(this, eventArgs);
            }
            return eventArgs.Value;
        }
    }

    /// <summary>
    /// Provides data for the ValueValidating event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueValidatingEventArgs<T> : EventArgs
    {
        public ValueValidatingEventArgs(T value) => Value = value;

        /// <summary>
        /// Get: gets value that should be validated.
        /// <para>Set: sets validated value.</para>
        /// </summary>
        public T Value { get; set; }
    }
}
