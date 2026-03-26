
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Shield : UserControl
    {
        AntdUI.BaseForm form;
        public Shield(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
            shield1.Text = "v" + shield1.ProductVersion;
        }
    }
}