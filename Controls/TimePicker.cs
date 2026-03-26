
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class TimePicker : UserControl
    {
        AntdUI.BaseForm form;
        public TimePicker(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}