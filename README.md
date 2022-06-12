# Tree Data Structure Library with Events and Binding for .NET

## Core
### Features
1. All properties and methods are thread-safe.
2. Values are stored in ValueNode\<T\> and ValueListNode\<T\>.
3. Value property of ValueNode\<T\> is the main way of changing node values.
4. ValueChanged event propagates to parent nodes. In case of changing the value of a container node its ValueChanged event is raised only once.
5. ValueValidating event can be used to validate node value before it is set and ValueChanged event is raised.
6. ClassNode and ClassListNode\<T\> provide container functionality.

### Example
#### Code
    using PL.Tree;
    
    XY XY = new();
    XY.X.ValueChanged += (s, e) => Console.WriteLine($"XY.X changed to {XY.X.Value}");
    XY.Y.ValueChanged += (s, e) => Console.WriteLine($"XY.Y changed to {XY.Y.Value}");
    XY.ValueChanged += (s, e) => Console.WriteLine($"XY changed to {{{XY.X.Value}, {XY.Y.Value}}}");

    XY.X.Value = 5;
    XY newXY = new();
    newXY.X.Value = 1;
    newXY.Y.Value = 2;
    XY.SetValue(newXY);

    internal class XY : ClassNode
    {
        public ValueNode<int> X { get; } = new();
        public ValueNode<int> Y { get; } = new();
    }

#### Output
    XY.X changed to 5
    XY changed to {5, 0}
    XY.X changed to 1
    XY.Y changed to 2
    XY changed to {1, 2}
   
## Windows Forms Binding
### Features
1. Bidirectional binding between nodes and controls.
2. Binding provides handlers for node and control events.
3. Node ValueChanged event handler is executed on the thread that owns the control's underlying window handle.
4. Node ValueChanged can be ignored while control is focused.
5. Default bidirectional converter between node and control values can be changed.
6. Bindings class acts as a collection for bindings to simplify actions targeted on all of those bindings and provide generic methods to add specific bindings.

### Bindings.Add methods
1. Node ⟶ Control (custom ValueChanged event handler).
2. ValueNode\<T\>.Value ⟶ Control.Text
3. ValueNode\<T\>.Value ⟷ TextBoxBase.Text
4. ValueNode\<string\>.Value ⟷ ComboBox.Text
5. ValueNode\<T\>.Value ⟷ ListControl.SelectedIndex
6. ValueNode\<Enum\>.Value ⟷ ComboBox.SelectedIndex
7. ValueNode\<Enum\>.Value ⟷ TabControl.SelectedIndex
8. ValueNode\<Enum\>.Value ⟷ GroupBox (RadioButtons)
9. ValueNode\<Enum\>.Value ⟷ Panel (RadioButtons)
10. ValueNode\<bool\>.Value = !Value ⟵ ButtonBase.Click
11. ValueNode\<bool\>.Value ⟷ CheckBox
11. ValueNode\<DateTime\>.Value ⟷ DateTimePicker
12. ValueNode\<T\>.Value ⟷ TrackBar
13. ValueNode\<T\>.Value ⟶ ProgressBar
14. ValueListNode<T> ⟷ DataGridView (with Converter)
15. ClassListNode<T> ⟷ DataGridView (with List<Converter>)

### Example
`Example/WinForms` application demonstrates Windows Forms binding functionality.
