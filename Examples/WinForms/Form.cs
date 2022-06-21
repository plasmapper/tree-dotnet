using System.ComponentModel;
using System.Windows.Forms.DataVisualization.Charting;

using PL.Tree;
using PL.Tree.WinForms;

namespace WinForms
{
    public partial class Form : System.Windows.Forms.Form
    {
        private readonly Bindings _bindings;
        private readonly Chart chart = new();

        public Form()
        {
            _bindings = new Bindings(LoseFocus);
            InitializeComponent();

            ((ISupportInitialize)chart).BeginInit();
            panelGraph.Controls.Add(chart);
            chart.Dock = DockStyle.Fill;
            chart.ChartAreas.Add(new ChartArea());
            chart.Series.Add(new Series());
            ((ISupportInitialize)chart).EndInit();
        }

        private void Form_Load(object sender, EventArgs eventArgs)
        {
            LoseFocus();
            Click += LoseFocus;

            _bindings.Add(Parameters.String, textBoxString);
            _bindings.Add(Parameters.String, comboBoxString);

            _bindings.Add(Parameters.Float, textBoxFloat, "F");
            _bindings.Add(Parameters.Float, textBoxFloatCopy, "E");

            var integerBinding = _bindings.Add(Parameters.Integer, textBoxInteger);
            integerBinding.NodeEventsEnabledWhenControlIsFocused = false;
            integerBinding.Converter = new DecimalOrHexConverter();
            var integerPlusOneBinding = _bindings.Add(Parameters.IntegerPlusOne, textBoxIntegerPlusOne);
            integerPlusOneBinding.Converter = new DecimalOrHexConverter();

            _bindings.Add(Parameters.Enum, comboBoxEnum);
            _bindings.Add(Parameters.Enum, tabControlEnum);
            _bindings.Add(Parameters.Enum, groupBoxEnum);

            _bindings.Add(Parameters.Hex, checkBoxHex);
            _bindings[^1].NodeValueChanged += (s, e) =>
            {
                checkBoxHex.Text = Parameters.Hex.Value ? "HEX" : "DEC";
                Parameters.Integer.OnValueChanged();
                Parameters.IntegerPlusOne.OnValueChanged();
            };
            
            _bindings.Add(Parameters.DateTime, dateTimePicker);

            _bindings.Add(Parameters.TrackBar, trackBar);
            _bindings.Add(Parameters.TrackBar, progressBar);

            _bindings.Add(Parameters.Graph.X0, textBoxGraphX0);
            _bindings.Add(Parameters.Graph.DX, textBoxGraphDX);
            _bindings.Add(Parameters.Graph.Y, dataGridViewGraphY, new StringConverter<double>("G3"));
            _bindings.Add(Parameters.NewGraphY, buttonNewGraphY);

            _bindings.Add(new(Parameters.Graph, chart));
            _bindings[^1].NodeValueChanged += (s, e) =>
            {
                Graph graph = new();
                graph.SetValue(Parameters.Graph);
                List<double> x = new();
                List<double> y = new(graph.Y);
                double x0 = graph.X0.Value;
                double dx = graph.DX.Value;
                for (int i = 0; i < y.Count; i++)
                    x.Add(x0 + dx * i);
                chart.Series[0].Points.DataBindXY(x, y);
                chart.ChartAreas[0].AxisX.Minimum = x0;
                chart.ChartAreas[0].AxisX.Maximum = x0 + dx * (y.Count - 1);
            };

            _bindings.Add(Parameters.ComplexDataList, dataGridView, new()
            {
                new CastConverter<string, string>(),
                new StringConverter<float>("F1"),
                new CastConverter<bool, bool>(),
                new EnumConverter<Enum, string>(new () { "Stop", "x1", "x10", "x100" })
            });

            _bindings.Add(new(Parameters.ComplexDataList, textBoxDataGrid));
            _bindings[^1].NodeValueChanged += (s, e) =>
            {
                string text = "";
                foreach (var data in Parameters.ComplexDataList)
                    text += $"{data.String.Value}, {data.Float.Value}, {data.Bool.Value}, {data.Enum.Value}\r\n";
                textBoxDataGrid.Text = text;
            };

            _bindings.ControlValueChangedException += (s, e) => MessageBox.Show(e.Exception.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _bindings.OnNodeValueChange();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) => _bindings.Dispose();

        private void LoseFocus() => dummy.Select();

        private void LoseFocus(object? sender, EventArgs e) => LoseFocus();
    }

    internal class DecimalOrHexConverter : PL.Tree.Converter<int, string>
    {
        public override string ConvertSourceToTarget(int source) => Parameters.Hex.Value ? source.ToString("X") : source.ToString();

        public override int ConvertTargetToSource(string target) => Parameters.Hex.Value ? Convert.ToInt32(target, 16) : int.Parse(target);
    }
}
