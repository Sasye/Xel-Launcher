
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Badge : UserControl
    {
        AntdUI.BaseForm form;
        public Badge(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }

        private void button5_Click(object sender, System.EventArgs e)
        {
            button4.Enabled = false;
            AntdUI.ITask.Run(() =>
            {
                System.Threading.Thread.Sleep(2000);
                button4.Enabled = true;
            });
        }
    }
}