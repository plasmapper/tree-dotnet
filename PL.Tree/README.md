# PL.Tree
Thread-safe tree data structure library with event propagation.

## Features
1. All properties and methods are thread-safe.
2. Values are stored in ValueNode\<T\> and ValueListNode\<T\>.
3. Value property of ValueNode\<T\> is the main way of changing node values.
4. ValueChanged event propagates to parent nodes. In case of changing the value of a container node its ValueChanged event is raised only once.
5. ValueValidating event can be used to validate node value before it is set and ValueChanged event is raised.
6. ClassNode and ClassListNode\<T\> provide container functionality.

## Example
### Code
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

### Output
    XY.X changed to 5
    XY changed to {5, 0}
    XY.X changed to 1
    XY.Y changed to 2
    XY changed to {1, 2}
