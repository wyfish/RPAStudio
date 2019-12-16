using System;
using System.Drawing;
using System.Windows.Forms;

namespace Plugins.Shared.Library.Window
{
    public partial class ShowImageWindow : Form
    {
        public ShowImageWindow()
        {
            InitializeComponent();
        }

        public void ShowImage(string src)
        {
            var filename = SharedObject.Instance.ProjectPath + @"\.screenshots\"+ src;
            Image img = Image.FromFile(filename);
            this.BackgroundImage = img;
            this.Width = img.Width;
            this.Height = img.Height;
            this.Show();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Pen pen1 = new Pen(Color.LightGray, 2);
            pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            e.Graphics.DrawRectangle(pen1, 0, 0, this.Width - 2, this.Height - 2);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (bShow)
                this.Close();
            else
                bShow = !bShow;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.Close();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        protected override void OnShown(EventArgs e)
        {
            SetForegroundWindow(this.Handle);
        }

        bool bShow = false;
    }
}
