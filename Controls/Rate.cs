
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Rate : UserControl
    {
        AntdUI.BaseForm form;
        public Rate(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}