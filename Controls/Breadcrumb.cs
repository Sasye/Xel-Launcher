
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Breadcrumb : UserControl
    {
        AntdUI.BaseForm form;
        public Breadcrumb(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();
        }

        private void breadcrumb1_ItemClick(object sender, AntdUI.BreadcrumbItemEventArgs e)
        {
            AntdUI.Message.info(form, AntdUI.Localization.Get("Click:", "点击了：") + e.Item.Text);
        }
    }
}