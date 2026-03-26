
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Checkbox : UserControl
    {
        AntdUI.BaseForm form;
        public Checkbox(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e) => checkbox9.Checked = !checkbox9.Checked;

        private void button2_Click(object sender, System.EventArgs e) => checkbox9.Enabled = !checkbox9.Enabled;

        private void checkbox9_CheckedChanged(object sender, AntdUI.BoolEventArgs e) => SetText();
        private void checkbox9_EnabledChanged(object sender, System.EventArgs e) => SetText();

        void SetText()
        {
            checkbox9.Text = (checkbox9.Checked ? "Checked" : "Unchecked") + "-" + (checkbox9.Enabled ? "Enabled" : "Disabled");
            button1.Text = checkbox9.Checked ? "Uncheck" : "Check";
            button2.Text = checkbox9.Enabled ? "Disable" : "Enable";
        }
    }
}