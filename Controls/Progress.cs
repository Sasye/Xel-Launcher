
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Progress : UserControl
    {
        AntdUI.BaseForm form;
        public Progress(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}