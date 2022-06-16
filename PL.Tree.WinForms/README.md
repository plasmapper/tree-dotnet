# PL.Tree.WinForms
PL.Tree binding library for Windows Forms.

## Features
1. Bidirectional binding between nodes and controls.
2. Binding provides handlers for node and control events.
3. Node ValueChanged event handler is executed on the thread that owns the control's underlying window handle.
4. Node ValueChanged can be ignored while control is focused.
5. Default bidirectional converter between node and control values can be changed.
6. Bindings class acts as a collection for bindings to simplify actions targeted on all of those bindings and provide generic methods to add specific bindings.

## Bindings.Add methods
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

## Example
Examples/WinForms application demonstrates Windows Forms binding functionality.
