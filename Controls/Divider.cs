
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Divider : UserControl
    {
        AntdUI.BaseForm form;
        public Divider(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}