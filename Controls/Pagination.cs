
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Pagination : UserControl
    {
        AntdUI.BaseForm form;
        public Pagination(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }
    }
}