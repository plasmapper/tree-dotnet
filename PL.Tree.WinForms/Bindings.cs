namespace PL.Tree.WinForms
{
    /// <summary>
    /// Represens a collection of bindings
    /// </summary>
    public class Bindings : List<Binding>, IDisposable
    {
        private readonly Action _loseFocus;

        /// <summary>
        /// Initializes a new instance of the Converter class.
        /// </summary>
        /// <param name="loseFocus">Action that removes focus from any bound control.</param>
        public Bindings(Action loseFocus) : base() => _loseFocus = loseFocus;

        /// <summary>
        /// Occurs when exception has been caught while processing any bound control value changed event. 
        /// </summary>
        public event EventHandler<ControlValueChangedExceptionEventArgs> ControlValueChangedException
        {
            add
            {
                foreach (var binding in this)
                    binding.ControlValueChangedException += value;
            }
            remove
            {
                foreach (var binding in this)
                    binding.ControlValueChangedException -= value;
            }
        }

        /// <summary>
        /// Adds binding.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <param name="nodeValueChangedEventHandler">Node value changed event handler.</param>
        /// <returns>Binding.</returns>
        public Binding Add(Node node, Control control, EventHandler nodeValueChangedEventHandler)
        {
            Binding binding = new(node, control);
            binding.NodeValueChanged += nodeValueChangedEventHandler;
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding that converts node value to control text.
        /// </summary>
        /// <typeparam name="T">Node value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <param name="format">String format.</param>
        /// <returns>Binding.</returns>
        public Binding<T, string> Add<T>(ValueNode<T> node, Control control, string? format = null)
        {
            Binding<T, string> binding = new(node, control, new StringConverter<T>(format));
            binding.NodeValueChanged += (s, e) => control.Text = binding.Converter.ConvertSourceToTarget(node.Value);
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional (unidirectional if control is read-only) converter between node value and control text.
        /// </summary>
        /// <typeparam name="T">Node value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <param name="format">String format.</param>
        /// <returns>Binding.</returns>
        public Binding<T, string> Add<T>(ValueNode<T> node, TextBoxBase control, string? format = null)
        {
            Binding<T, string> binding = new(node, control, new StringConverter<T>(format));
            binding.NodeValueChanged += (s, e) => control.Text = binding.Converter.ConvertSourceToTarget(node.Value);
            if (!control.ReadOnly)
            {
                binding.ControlValueChanged += (s, e) =>
                {
                    try { node.Value = binding.Converter.ConvertTargetToSource(control.Text); }
                    catch { }
                };
                binding.AddControlValueChangedEvent("LostFocus");
                if (!control.Multiline)
                    AddTextControlKeyDownEvent(binding, control);
            }
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between node string value and ComboBox text.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <returns>Binding.</returns>
        public Binding<string, string> Add(ValueNode<string> node, ComboBox control)
        {
            Binding<string, string> binding = new(node, control);
            binding.NodeValueChanged += (s, e) => control.Text = binding.Converter.ConvertSourceToTarget(node.Value);
            binding.ControlValueChanged += (s, e) => node.Value = binding.Converter.ConvertTargetToSource(control.Text);
            AddTextControlKeyDownEvent(binding, control);
            binding.AddControlValueChangedEvent("LostFocus");
            control.SelectedIndexChanged += (s, e) => _loseFocus();
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between node value and ListControl index.
        /// </summary>
        /// <typeparam name="T">Node value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <returns>Binding.</returns>
        public Binding<T, int> Add<T>(ValueNode<T> node, ListControl control)
        {
            Binding<T, int> binding = new(node, control);
            binding.NodeValueChanged += (s, e) => control.SelectedIndex = binding.Converter.ConvertSourceToTarget(node.Value);
            binding.ControlValueChanged += (s, e) => node.Value = binding.Converter.ConvertTargetToSource(control.SelectedIndex);
            binding.AddControlValueChangedEvent("SelectedIndexChanged");
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between node enum value and ComboBox index.
        /// </summary>
        /// <typeparam name="T">Node enum value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">ComboBox with items that correspond to Enum.GetValues(typeof(T)).</param>
        /// <returns>Binding.</returns>
        public Binding<T, int> Add<T>(ValueNode<T> node, ComboBox control) where T : Enum
        {
            Binding<T, int> binding = new(node, control, new EnumIndexConverter<T>());
            binding.NodeValueChanged += (s, e) => control.SelectedIndex = binding.Converter.ConvertSourceToTarget(node.Value);
            binding.ControlValueChanged += (s, e) => node.Value = binding.Converter.ConvertTargetToSource(control.SelectedIndex);
            binding.AddControlValueChangedEvent("SelectedIndexChanged");
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between node enum value and TabControl index.
        /// </summary>
        /// <typeparam name="T">Node enum value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">TabControl with TabPages that correspond to Enum.GetValues(typeof(T)).</param>
        /// <returns>Binding.</returns>
        public Binding<T, int> Add<T>(ValueNode<T> node, TabControl control) where T : Enum
        {
            Binding<T, int> binding = new(node, control, new EnumIndexConverter<T>());
            binding.NodeValueChanged += (s, e) => control.SelectedIndex = binding.Converter.ConvertSourceToTarget(node.Value);
            binding.ControlValueChanged += (s, e) => node.Value = binding.Converter.ConvertTargetToSource(control.SelectedIndex);
            binding.AddControlValueChangedEvent("SelectedIndexChanged");
            Add(binding);
            return binding;
        }

        private Binding<T, int> AddRadioButtonsBinding<T>(ValueNode<T> node, Control control) where T : Enum
        {
            Binding<T, int> binding = new(node, control, new EnumIndexConverter<T>());
            binding.NodeValueChanged += (s, e) => ((RadioButton)control.Controls[binding.Converter.ConvertSourceToTarget(node.Value)]).Checked = true;
            binding.ControlValueChanged += (s, e) =>
            {
                for (int i = 0; i < control.Controls.Count; i++)
                {
                    if (((RadioButton)control.Controls[i]).Checked)
                        node.Value = binding.Converter.ConvertTargetToSource(i);
                }
            };
            foreach (var c in control.Controls)
            {
                ((RadioButton)c).CheckedChanged += (s, e) =>
                {
                    control.Focus();
                    _loseFocus();
                };
            }
            binding.AddControlValueChangedEvent("LostFocus");
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between node enum value and RadioButtons inside a GroupBox.
        /// </summary>
        /// <typeparam name="T">Node enum value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">GroupBox with RadioButtons that correspond to Enum.GetValues(typeof(T))..</param>
        /// <returns>Binding.</returns>
        public Binding<T, int> Add<T>(ValueNode<T> node, GroupBox control) where T : Enum =>
            AddRadioButtonsBinding(node, control);

        /// <summary>
        /// Adds binding with bidirectional converter between node enum value and RadioButtons inside a Panel.
        /// </summary>
        /// <typeparam name="T">Node enum value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">Panel with RadioButtons that correspond to Enum.GetValues(typeof(T))..</param>
        /// <returns>Binding.</returns>
        public Binding<T, int> Add<T>(ValueNode<T> node, Panel control) where T : Enum =>
            AddRadioButtonsBinding(node, control);

        /// <summary>
        /// Adds binding that changes node boolean value when ButtonBase control is clicked.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <returns>Binding.</returns>
        public Binding<bool, bool> Add(ValueNode<bool> node, ButtonBase control)
        {
            Binding<bool, bool> binding = new(node, control);
            binding.ControlValueChanged += (s, e) => node.Value = !node.Value;
            binding.AddControlValueChangedEvent("Click");
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between node boolean value and CheckBox checked state.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <returns>Binding.</returns>
        public Binding<bool, bool> Add(ValueNode<bool> node, CheckBox control)
        {
            Binding<bool, bool> binding = new(node, control);
            binding.NodeValueChanged += (s, e) => control.Checked = binding.Converter.ConvertSourceToTarget(node.Value);
            binding.ControlValueChanged += (s, e) => node.Value = !node.Value;
            binding.AddControlValueChangedEvent("CheckedChanged");
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between node DateTime value and DateTimePicker value.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <returns>Binding.</returns>
        public Binding<DateTime, DateTime> Add(ValueNode<DateTime> node, DateTimePicker control)
        {
            Binding<DateTime, DateTime> binding = new(node, control);
            binding.NodeValueChanged += (s, e) =>
            {
                DateTime dateTime = binding.Converter.ConvertSourceToTarget(node.Value);
                control.Value = (dateTime < control.MinDate) ? control.MinDate : (dateTime > control.MaxDate ? control.MaxDate : dateTime);
            };
            binding.ControlValueChanged += (s, e) => node.Value = binding.Converter.ConvertTargetToSource(control.Value);
            binding.AddControlValueChangedEvent("LostFocus");
            AddTextControlKeyDownEvent(binding, control);
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between node value and TrackBar value.
        /// </summary>
        /// <typeparam name="T">Node enum value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <returns>Binding.</returns>
        public Binding<T, int> Add<T>(ValueNode<T> node, TrackBar control)
        {
            Binding<T, int> binding = new(node, control);
            binding.NodeValueChanged += (s, e) => control.Value = binding.Converter.ConvertSourceToTarget(node.Value);
            binding.ControlValueChanged += (s, e) => node.Value = binding.Converter.ConvertTargetToSource(control.Value);
            binding.AddControlValueChangedEvent("ValueChanged");
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding that converts node value to ProgressBar value.
        /// </summary>
        /// <typeparam name="T">Node value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">Control.</param>
        /// <returns>Binding.</returns>
        public Binding<T, int> Add<T>(ValueNode<T> node, ProgressBar control)
        {
            Binding<T, int> binding = new(node, control);
            binding.NodeValueChanged += (s, e) =>
            {
                int value = binding.Converter.ConvertSourceToTarget(node.Value);
                if (value < control.Maximum)
                    control.Value = value + 1;
                else
                {
                    control.Maximum = value + 1;
                    control.Value = value + 1;
                    control.Maximum = value;
                }
                control.Value = value;
            };
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between ValueListNode and DataGridView.
        /// </summary>
        /// <typeparam name="T">Node value type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">DataGridView with single column.</param>
        /// <param name="converters">Converter.</param>
        /// <returns>Binding.</returns>
        public Binding Add<T>(ValueListNode<T> node, DataGridView control, Converter converter)
        {
            Binding binding = new (node, control);
            binding.NodeValueChanged += (s, e) =>
            {
                control.Rows.Clear();
                foreach (var item in node)
                    control.Rows.Add(new object[] { converter.ConvertSourceToTarget(item) });
            };
            binding.ControlValueChanged += (s, e) =>
            {
                List<T> items = new();
                int rowCount = control.RowCount - (control.AllowUserToAddRows ? 1 : 0);

                try
                {
                    for (int r = 0; r < rowCount; r++)
                        items.Add((T)converter.ConvertTargetToSource(control.Rows[r].Cells[0].Value));
                    node.SetValue(items);
                }
                catch { }                
            };
            control.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (control.CurrentCell.EditType == null || !control.CurrentCell.EditType.IsSubclassOf(typeof(TextBoxBase)))
                    _loseFocus();
            };

            binding.AddControlValueChangedEvent("UserDeletedRow");
            binding.AddControlValueChangedEvent("Sorted");
            binding.AddControlValueChangedEvent("CellValueChanged");
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds binding with bidirectional converter between ClassNodeList and DataGridView.
        /// </summary>
        /// <typeparam name="T">ClassNode-based type.</typeparam>
        /// <param name="node">Node.</param>
        /// <param name="control">DataGridView with columns that correspond to node children.</param>
        /// <param name="converters">List of Converters that correspond to node children.</param>
        /// <returns>Binding.</returns>
        public Binding Add<T>(ClassListNode<T> node, DataGridView control, List<Converter> converters) where T : ClassNode, new()
        {
            Binding binding = new(node, control);
            binding.NodeValueChanged += (s, e) =>
            {
                control.Rows.Clear();
                foreach (var item in node)
                {
                    object[] values = new object[converters.Count];
                    var children = item.GetChildren().GetEnumerator();
                    for (int i = 0; i < converters.Count; i++)
                    {
                        children.MoveNext();
                        values[i] = converters[i].ConvertSourceToTarget(children.Current.GetValue());
                    }
                    control.Rows.Add(values);
                }
            };
            binding.ControlValueChanged += (s, e) =>
            {
                List<T> items = new();
                int rowCount = control.RowCount - (control.AllowUserToAddRows ? 1 : 0);

                for (int r = 0; r < rowCount; r++)
                {
                    T item = new();
                    var children = item.GetChildren().GetEnumerator();
                    for (int c = 0; c < converters.Count; c++)
                    {
                        try
                        {
                            children.MoveNext();
                            children.Current.SetValue(converters[c].ConvertTargetToSource(control.Rows[r].Cells[c].Value));
                        }
                        catch { }
                    }
                        
                    items.Add(item);
                }
                node.SetValue(items);
            };
            control.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (control.CurrentCell.EditType == null || !control.CurrentCell.EditType.IsSubclassOf(typeof(TextBoxBase)))
                    _loseFocus();
            };

            binding.AddControlValueChangedEvent("UserDeletedRow");
            binding.AddControlValueChangedEvent("Sorted");
            binding.AddControlValueChangedEvent("CellValueChanged");
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Raises NodeValueChanged event for all bound controls.
        /// </summary>
        public void OnNodeValueChange()
        {
            foreach (var binding in this)
                binding.OnNodeValueChanged();
        }

        private void AddTextControlKeyDownEvent(Binding binding, Control control)
        {
            control.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Return)
                    _loseFocus();
                if (e.KeyCode == Keys.Escape)
                {
                    binding.EventsEnabled = false;
                    _loseFocus();
                    binding.EventsEnabled = true;
                    OnNodeValueChange();
                }
            };
        }

        public void Dispose()
        {
            foreach (var binding in this)
                binding.Dispose();
            Clear();
            GC.SuppressFinalize(this);
        }
    }
}
