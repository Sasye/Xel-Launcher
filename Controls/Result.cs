
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Result : UserControl
    {
        AntdUI.BaseForm form;
        public Result(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}