using System.Reflection;

namespace PL.Tree.WinForms
{
    /// <summary>
    /// Bidirectional node to control binding.
    /// </summary>
    public class Binding : IDisposable
    {
        private readonly Dictionary<string, ControlEventInfo> _controlEvents = new();
        private List<Converter> _converters = new ();

        /// <summary>
        /// Initializes a new instance of the Binding class.
        /// </summary>
        public Binding(Node node, Control control)
        {
            Node = node;
            Control = control;

            Node.ValueChanged += OnNodeValueChanged;
        }

        /// <summary>
        /// Gets binding node.
        /// </summary>
        public Node Node { get; }

        /// <summary>
        /// Gets binding control
        /// </summary>
        public Control Control { get; }

        /// <summary>
        /// Gets and sets bidirectional converters between node and control values.
        /// </summary>
        public List<Converter> Converters
        {
            get
            {
                lock (this)
                    return new List<Converter> (_converters);
            }
            set
            {
                lock (this)
                    _converters = new List<Converter> (value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node and control events are enabled.
        /// </summary>
        public bool EventsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether node events are enabled when control is focused.
        /// </summary>
        public bool NodeEventsEnabledWhenControlIsFocused { get; set; } = true;

        /// <summary>
        /// Occurs when node value has been changed. Event handler is executed on the thread that owns the control's underlying window handle.
        /// </summary>
        public event EventHandler? NodeValueChanged;

        /// <summary>
        /// Occurs when control value has been changed.
        /// </summary>
        public event EventHandler? ControlValueChanged;

        /// <summary>
        /// Occurs when exception has been caught while processing control value changed event.
        /// </summary>
        public event EventHandler<ControlValueChangedExceptionEventArgs>? ControlValueChangedException;

        /// <summary>
        /// Adds control event that should be treated as ValueChanged.
        /// </summary>
        /// <param name="name">Control event name.</param>
        public void AddControlValueChangedEvent(string name)
        {
            EventInfo eventInfo = Control.GetType().GetEvent(name)!;
            Delegate typedDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType!, ((EventHandler)OnControlEvent).Target, ((EventHandler)OnControlEvent).Method);
            _controlEvents.Add(name, new ControlEventInfo() { EventInfo = eventInfo, Delegate = typedDelegate });
            _controlEvents[name].EventInfo.AddEventHandler(Control, _controlEvents[name].Delegate);
        }

        /// <summary>
        /// Removes control event that should be treated as ValueChanged.
        /// </summary>
        /// <param name="name">Control event name.</param>
        public void RemoveControlValueChangedEvent(string name)
        {
            if (_controlEvents.ContainsKey(name))
            {
                _controlEvents[name].EventInfo.RemoveEventHandler(Control, _controlEvents[name].Delegate);
                _controlEvents.Remove(name);
            }
        }

        /// <summary>
        /// Raises NodeValueChanged event
        /// </summary>
        public void OnNodeValueChanged() => OnNodeValueChanged(null, EventArgs.Empty);

        private void OnNodeValueChanged(object? sender, EventArgs eventArgs)
        {
            if (!EventsEnabled)
                return;
            EventsEnabled = false;
            try
            {
                Control.Invoke((MethodInvoker)(() => {
                    if (!Control.Focused || NodeEventsEnabledWhenControlIsFocused)
                        NodeValueChanged?.Invoke(sender, eventArgs);
                }));
            }
            finally
            {
                EventsEnabled = true;
            }
        }

        private void OnControlEvent(object? sender, EventArgs eventArgs)
        {
            if (!EventsEnabled)
                return;
            EventsEnabled = false;
            try
            {
                ControlValueChanged?.Invoke(sender, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                ControlValueChangedException?.Invoke(sender, new ControlValueChangedExceptionEventArgs(exception));
            }
            finally
            {
                EventsEnabled = true;
            }

            try
            {
                OnNodeValueChanged();
            }
            catch (Exception exception)
            {
                ControlValueChangedException?.Invoke(sender, new ControlValueChangedExceptionEventArgs(exception));
            }
        }

        public virtual void Dispose()
        {
            Node.ValueChanged -= OnNodeValueChanged;
            foreach (var controlEvent in _controlEvents)
                controlEvent.Value.EventInfo.RemoveEventHandler(Control, controlEvent.Value.Delegate);
            GC.SuppressFinalize(this);
        }

        private struct ControlEventInfo
        {
            public EventInfo EventInfo;
            public Delegate Delegate;
        }
    }

    /// <summary>
    /// Bidirectional ValueNode to typed control binding.
    /// </summary>
    public class Binding<NodeType, ControlType> : Binding
    {
        /// <summary>
        /// Initializes a new instance of the Binding class.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        public Binding(ValueNode<NodeType> node, Control control) : base(node, control)
        {
            Converters = new() { new CastConverter<NodeType, ControlType>() };
        }

        /// <summary>
        /// Initializes a new instance of the Binding class.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <param name="converter">Bidirectional converter between node and control values.</param>
        public Binding(ValueNode<NodeType> node, Control control, Converter<NodeType, ControlType> converter) : base(node, control) => Converter = converter;

        /// <summary>
        /// Gets and sets bidirectional converter between node and control values. Same as Converters[0].
        /// </summary>
        public Converter<NodeType, ControlType> Converter
        {
            get => (Converter<NodeType, ControlType>)Converters[0];
            set => Converters = new() { value };
        }
    }

    /// <summary>
    /// Provides data for the ControlValueChangedException event.
    /// </summary>
    public class ControlValueChangedExceptionEventArgs : EventArgs
    {
        public ControlValueChangedExceptionEventArgs(Exception exception) => Exception = exception;

        /// <summary>
        /// The exception that occurred.
        /// </summary>
        public Exception Exception { get; }
    }
}
