
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Segmented : UserControl
    {
        AntdUI.BaseForm form;
        public Segmented(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}