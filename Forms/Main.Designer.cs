using System.Windows.Forms;

namespace XelLauncher
{
    partial class Main
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panel_top = new AntdUI.PageHeader();
            SuspendLayout();
            //
            // panel_top
            //
            panel_top.Dock = DockStyle.Top;
            panel_top.Icon = Properties.Resources.logo;
            panel_top.Location = new System.Drawing.Point(0, 0);
            panel_top.Name = "panel_top";
            panel_top.ShowButton = true;
            panel_top.ShowIcon = true;
            panel_top.Size = new System.Drawing.Size(1300, 40);
            panel_top.TabIndex = 0;
            panel_top.Text = "AntdUI";
            //
            // Main
            //
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(1300, 720);
            Controls.Add(panel_top);
            Font = new System.Drawing.Font("Microsoft YaHei UI Light", 12F);
            ForeColor = System.Drawing.Color.Black;
            Icon = Properties.Resources.icon;
            MinimumSize = new System.Drawing.Size(1300, 720);
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AntdUI";
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.PageHeader panel_top;
    }
}
