using PL.Tree;

namespace WinForms
{
    internal enum Enum : int { Stop = 0, Rate1 = 1, Rate10 = 10, Rate100 = 100 };

    internal class Graph : ClassNode
    {
        public ValueNode<double> X0 { get; } = new(0);
        public ValueNode<double> DX { get; } = new(1);
        public ValueListNode<double> Y { get; } = new();
    }

    internal class ComplexData : ClassNode
    {
        public ValueNode<string> String { get; } = new("");
        public ValueNode<float> Float { get; } = new();
        public ValueNode<bool> Bool { get; } = new();
        public ValueNode<Enum> Enum { get; } = new();
    }

    // Program parameter tree
    internal static class Parameters
    {
        public static ValueNode<string> String { get; } = new ("");
        public static ValueNode<float> Float { get; } = new();
        public static ValueNode<int> Integer { get; } = new();
        public static ValueNode<int> IntegerPlusOne { get; } = new(1);
        public static ValueNode<Enum> Enum { get; } = new();
        public static ValueNode<bool> Hex { get; } = new();
        public static ValueNode<DateTime> DateTime { get; } = new(System.DateTime.Now);
        public static ValueNode<int> TrackBar { get; } = new();
        public static Graph Graph { get; } = new();
        public static ValueNode<bool> NewGraphY { get; } = new();
        public static ClassListNode<ComplexData> ComplexDataList { get; } = new();
    }

    internal static class Program
    {
        public const int numberOfGraphPoints = 10;

        // Background thread that increments Interer value based on Enum value
        static void Increment()
        {
            while (true)
            {
                Parameters.Integer.Value += (int)Parameters.Enum.Value;
                Thread.Sleep(100);
            }       
        }

        [STAThread]
        static void Main()
        {
            // Specify Float range using ValueValidating event
            Parameters.Float.ValueValidating += (s, e) => e.Value = Math.Min(1e6f, Math.Max(e.Value, -1e6f));

            // Force IntegerPlusOne to be Integer + 1 using ValueChanged event
            Parameters.Integer.ValueChanged += (s, e) => Parameters.IntegerPlusOne.Value = Parameters.Integer.Value + 1;

            // Show message box when DateTime is changed
            Parameters.DateTime.ValueChanged += (s, e) => MessageBox.Show(Parameters.DateTime.Value.ToString());

            // Generate new Y data when NewGraphY value is changed (button clicked)
            Parameters.NewGraphY.ValueChanged += (s, e) =>
            {
                if (Parameters.NewGraphY.Value)
                {
                    List<double> y = new();
                    Random random = new();
                    for (int i = 0; i < numberOfGraphPoints; i++)
                        y.Add (random.NextDouble());
                    Parameters.Graph.Y.SetValue(y);
                    Parameters.NewGraphY.Value = false;
                }
            };

            Thread IncrementThread = new(Increment);
            IncrementThread.IsBackground = true;
            IncrementThread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form());
        }
    }
}
