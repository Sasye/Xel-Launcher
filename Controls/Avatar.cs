
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Avatar : UserControl
    {
        AntdUI.BaseForm form;
        public Avatar(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}