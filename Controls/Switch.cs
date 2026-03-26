
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Switch : UserControl
    {
        AntdUI.BaseForm form;
        public Switch(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}