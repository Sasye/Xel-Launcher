
using System.Windows.Forms;
using XelLauncher.Forms;

namespace XelLauncher.Controls
{
    public partial class Panel : UserControl
    {
        Overview form;
        public Panel(Overview _form)
        {
            form = _form;
            InitializeComponent();
            button4.Click += button_Click;
        }

        private void button_Click(object sender, System.EventArgs e)
        {
            //form.OpenPage("VirtualPanel");
        }
    }
}