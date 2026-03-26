
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Timeline : UserControl
    {
        AntdUI.BaseForm form;
        public Timeline(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}