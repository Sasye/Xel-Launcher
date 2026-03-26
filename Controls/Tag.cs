
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Tag : UserControl
    {
        AntdUI.BaseForm form;
        public Tag(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}