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