
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Tooltip : UserControl
    {
        AntdUI.BaseForm form;
        public Tooltip(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}