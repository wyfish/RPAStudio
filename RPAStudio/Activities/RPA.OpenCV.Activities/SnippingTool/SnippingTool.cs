using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RPA.OpenCV.Activities.Snip
{
    // http://stackoverflow.com/questions/3123776/net-equivalent-of-snipping-tool
    // https://github.com/thepirat000/Snipping-Ocr/tree/master/Snipping%20OCR/SnippingTool
    public partial class SnippingTool : Form
    {
        internal static Rectangle _Selection = new Rectangle();
        private static Rectangle _margineRect = new Rectangle();

        #region Public members
        //public static event EventHandler Cancel;
        //public static event EventHandler AreaSelected;
        #endregion

        #region Private members
        private Image Image { get; set; }
        private static bool _SelectionDone = false, _SelectionCanceled = false;
        private static SnippingTool[] _forms;
        private Point _pointStart;
        private int _Margine = 0;
        #endregion

        public SnippingTool(Image screenShot, Rectangle limitedRect, int margine)
        {
            InitializeComponent();
            BackgroundImage = screenShot;
            BackgroundImageLayout = ImageLayout.Stretch;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            SetBounds(limitedRect.X, limitedRect.Y, limitedRect.Width, limitedRect.Height);
            if (limitedRect.Width == 0) {
                this.WindowState = FormWindowState.Maximized;
            }
            else {
                this.Location = new Point(limitedRect.X, limitedRect.Y);
                this.Size = new Size(limitedRect.Width, limitedRect.Height);
            }
            DoubleBuffered = true;
            Cursor = Cursors.Cross;
            TopMost = true;
            _Margine = margine;
            _SelectionDone = false;
            _SelectionCanceled = false;
        }

        #region Private methods
        private void OnCancel(EventArgs e)
        {
            _SelectionCanceled = true;
            //Cancel?.Invoke(this, e);
        }

        private void OnAreaSelected(EventArgs e)
        {
            _SelectionDone = true;
            //AreaSelected?.Invoke(this, e);
        }

        //private void CloseForms()
        //{
        //    for (int i = 0; i < _forms.Length; i++) {
        //        _forms[i].Dispose();
        //    }
        //}
        #endregion

        public static Image Snip()
        {
            var rc = Screen.PrimaryScreen.Bounds;
            using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                }
                using (var snipper = new SnippingTool(bmp, new Rectangle(0, 0, 0, 0), 0))
                {
                    if (snipper.ShowDialog() == DialogResult.OK)
                    {
                        return snipper.Image;
                    }
                }
                return null;
            }
        }

        public static Image Snip2()
        {
            _SelectionCanceled = false;
            _SelectionDone = false;
            var screens = ScreenHelper.GetMonitorsInfo();
            _forms = new SnippingTool[screens.Count];
            for (int i = 0; i < screens.Count; i++) {
                int hRes = screens[i].HorizontalResolution;
                int vRes = screens[i].VerticalResolution;
                int top = screens[i].MonitorArea.Top;
                int left = screens[i].MonitorArea.Left;
                var bmp = new Bitmap(hRes, vRes, PixelFormat.Format32bppPArgb);
                using (var g = Graphics.FromImage(bmp)) {
                    g.CopyFromScreen(left, top, 0, 0, bmp.Size);
                }
                //_forms[i] = new SnippingTool(bmp, left, top, hRes, vRes);
                _forms[i] = new SnippingTool(bmp, new Rectangle(left, top, hRes, vRes), 0);
                _forms[i].Show();
            }
            // Wait to be done
            for (; ;) {
                Application.DoEvents();
                if (_SelectionCanceled) {
                    for (int i = 0; i < _forms.Length; i++) {
                        _forms[i].Dispose();
                    }
                    return null;
                }
                if (_SelectionDone) {
                    Image img = null;
                    for (int i = 0; i < screens.Count; i++) {
                        //if (_forms[i].Image != null) return _forms[i].Image;
                        if (_forms[i].Image != null) img = _forms[i].Image;
                        _forms[i].Dispose();
                    }
                    return img;
                }
            }
        }

        public static Image SnipRect(Rectangle limitedRect, int margine, ref Rectangle margineRect, ref Rectangle snipedRect)
        {
            using (Bitmap bmp = new Bitmap(limitedRect.Width, limitedRect.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(limitedRect.X, limitedRect.Y, 0, 0, bmp.Size);
                }
                using (var snipper = new SnippingTool(bmp, limitedRect, margine))
                {
                    if (snipper.ShowDialog() == DialogResult.OK)
                    {
                        snipedRect = _Selection;
                        margineRect = _margineRect;
                        return snipper.Image;
                    }
                }
                return null;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Start the snip on mouse down
            if (e.Button != MouseButtons.Left) {
                return;
            }
            _pointStart = e.Location;
            _Selection = new Rectangle(e.Location, new Size(0, 0));
            SetMargineRect();
            this.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Modify the selection on mouse move
            if (e.Button != MouseButtons.Left) {
                return;
            }
            int x1 = Math.Min(e.X, _pointStart.X);
            int y1 = Math.Min(e.Y, _pointStart.Y);
            int x2 = Math.Max(e.X, _pointStart.X);
            int y2 = Math.Max(e.Y, _pointStart.Y);
            _Selection = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            SetMargineRect();
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Complete the snip on mouse-up
            if (_Selection.Width <= 0 || _Selection.Height <= 0) {
                //CloseForms();
                Image = null;
                OnCancel(new EventArgs());
                return;
            }
            //Image = new Bitmap(_Selection.Width, _Selection.Height);
            Image = new Bitmap(_Selection.Width, _Selection.Height, PixelFormat.Format24bppRgb);
            var hScale = BackgroundImage.Width / (double)Width;
            var vScale = BackgroundImage.Height / (double)Height;
            using (Graphics gr = Graphics.FromImage(Image)) {
                gr.DrawImage(BackgroundImage,
                    new Rectangle(0, 0, Image.Width, Image.Height),
                    new Rectangle((int)(_Selection.X * hScale), (int)(_Selection.Y * vScale), (int)(_Selection.Width * hScale), (int)(_Selection.Height * vScale)),
                    GraphicsUnit.Pixel);
            }
            DialogResult = DialogResult.OK;
            //CloseForms();
            OnAreaSelected(new EventArgs());
        }

        private void SetMargineRect()
        {
            _margineRect = new Rectangle(_Selection.Left - _Margine, _Selection.Top - _Margine, _Selection.Width + _Margine * 2, _Selection.Height + _Margine * 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try {
                // Draw the current selection
                using (Brush br = new SolidBrush(Color.FromArgb(120, Color.White))) {
                    int x1 = _Selection.X; int x2 = _Selection.X + _Selection.Width;
                    int y1 = _Selection.Y; int y2 = _Selection.Y + _Selection.Height;
                    e.Graphics.FillRectangle(br, new Rectangle(0, 0, x1, this.Height));
                    e.Graphics.FillRectangle(br, new Rectangle(x2, 0, this.Width - x2, this.Height));
                    e.Graphics.FillRectangle(br, new Rectangle(x1, 0, x2 - x1, y1));
                    e.Graphics.FillRectangle(br, new Rectangle(x1, y2, x2 - x1, this.Height - y2));
                }
                using (Pen pen = new Pen(Color.Red, 2)) {
                    e.Graphics.DrawRectangle(pen, _Selection);
                }
                // Draw margine area
                if (_Selection != _margineRect) {
                    using (Pen pen = new Pen(Color.Orange, 2)) {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        e.Graphics.DrawRectangle(pen, _margineRect);
                    }
                }
            }
            catch (Exception) { }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Allow canceling the snip with the Escape key
            if (keyData == Keys.Escape) {
                DialogResult = DialogResult.Cancel;
                Image = null;
                //CloseForms();
                OnCancel(new EventArgs());
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SnippingTool
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SnippingTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);

        }
    }
}
