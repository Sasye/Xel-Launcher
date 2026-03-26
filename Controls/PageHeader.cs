
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class PageHeader : UserControl
    {
        AntdUI.BaseForm form;
        public PageHeader(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            button1.Toggle = pageHeader1.ShowBack = pageHeader2.ShowBack = pageHeader4.ShowBack = !button1.Toggle;
        }
    }
}