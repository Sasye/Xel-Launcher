
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Slider : UserControl
    {
        AntdUI.BaseForm form;
        public Slider(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }

        private string slider7_ValueFormatChanged(object sender, AntdUI.IntEventArgs e)
        {
            return e.Value + "℃";
        }
    }
}