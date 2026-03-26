
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class ColorPicker : UserControl
    {
        AntdUI.BaseForm form;
        public ColorPicker(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}