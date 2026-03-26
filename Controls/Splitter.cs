
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Splitter : UserControl
    {
        AntdUI.BaseForm form;
        public Splitter(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}