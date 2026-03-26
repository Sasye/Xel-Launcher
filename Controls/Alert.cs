
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Alert : UserControl
    {
        AntdUI.BaseForm form;
        public Alert(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}