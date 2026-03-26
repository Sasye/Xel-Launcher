
using System;
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Signal : UserControl
    {
        AntdUI.BaseForm form;
        public Signal(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) => SetSignal(1, Controls);

        private void button2_Click(object sender, EventArgs e) => SetSignal(-1, Controls);

        void SetSignal(int value, ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is AntdUI.Signal signal) signal.Value += value;
                if (control.Controls.Count > 0) SetSignal(value, control.Controls);
            }
        }
    }
}