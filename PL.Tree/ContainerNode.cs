using System.Collections.Generic;

namespace PL.Tree
{
    /// <summary>
    /// Base class for classes that are parents to other nodes.
    /// </summary>
    public abstract class ContainerNode : Node
    {
        private bool _valueChanged = false;
        private bool _valueChangeEventEnabled = true;

        /// <summary>
        /// Gets node children.
        /// </summary>
        /// <returns>Node children.</returns>
        public abstract IEnumerable<Node> GetChildren();

        /// <summary>
        /// Sets children values from source node children values.
        /// </summary>
        /// <param name="node">Source node.</param>
        public void SetValue(ContainerNode node)
        {
            bool valueChanged = false;
            lock (this)
            {
                _valueChangeEventEnabled = false;
                _valueChanged = false;
                var sourceEnumerator = node.GetChildren().GetEnumerator();
                var targetEnumerator = GetChildren().GetEnumerator();
                while (sourceEnumerator.MoveNext() && targetEnumerator.MoveNext())
                    targetEnumerator.Current.SetValue(sourceEnumerator.Current.GetValue());
                _valueChangeEventEnabled = true;
                valueChanged = _valueChanged;
            }
            if (valueChanged)
                base.OnValueChanged();
        }

        /// <summary>
        /// Overrides OnValueChange to prevent raising multiple events when using SetValue.
        /// </summary>
        public override void OnValueChanged()
        {
            bool triggerEvent = false;
            lock (this)
            {
                _valueChanged = true;
                triggerEvent = _valueChangeEventEnabled;
            }
            if (triggerEvent)
                base.OnValueChanged();
        }

        public override void SetValue(object value) => SetValue((ContainerNode)value);
    }
}
