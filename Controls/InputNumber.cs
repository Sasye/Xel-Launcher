
using System.Globalization;
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class InputNumber : UserControl
    {
        AntdUI.BaseForm form;
        public InputNumber(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
            inputNumberWithValueFormatter.ValueFormatter += (sender, e) =>
            {
                if (e.Value % 1 == 0) return ((int)e.Value).ToString("N0");
                else return e.Value.ToString($"N{inputNumberWithValueFormatter.DecimalPlaces}", CultureInfo.CurrentCulture);
            };
        }
    }
}