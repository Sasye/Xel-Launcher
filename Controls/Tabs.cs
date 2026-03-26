
using System.Drawing;
using System.Windows.Forms;

namespace XelLauncher.Controls
{
    public partial class Tabs : UserControl
    {
        AntdUI.BaseForm form;
        public Tabs(AntdUI.BaseForm _form)
        {
            form = _form;
            InitializeComponent();

            for (int i = 0; i < 30; i++)
            {
                var it = new AntdUI.TabPage
                {
                    ReadOnly = i == 0,
                    IconSvg = i == 0 ? "AppleFilled" : null,
                    Text = "Tab" + (i + 1).ToString(),
                };
                it.Controls.Add(new AntdUI.Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.TopLeft,
                    Text = "Content of Tab Pane " + (i + 1)
                });
                tabs_close.Pages.Add(it);
            }
        }
    }
}