
using System.Windows.Forms;

namespace XelLauncher
{
    public partial class Main : AntdUI.Window
    {
        public Main()
        {
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            DraggableMouseDown();
            base.OnMouseDown(e);
        }
    }
}
