using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RPA.UIAutomation.Activities.Image
{
    public class FlaxCV
    {
        private int _retryTimes = 600;
        private int _matchingInterval = 2000;
        private int _matchingThreshold = 91;
        private bool _waitExit = true;
        // you can get Flax.CV.exe here.
        // https://github.com/teonsen/Flax.CV/releases
        internal static readonly string _FlaxCV_exe = Environment.CurrentDirectory + @"\Flax.CV.exe";

        internal enum CvActionType : int
        {
            DoNothing,
            LClick,
            DoubleClick,
            RightClick,
            Move,
            Matching,
            CompareHist
        }

        public int RetryTimes {
            get { return _retryTimes; }
            set { _retryTimes = value; }
        }

        public int MatchingInterval {
            set {
                if (value < 300)
                {
                    _matchingInterval = 300;
                }
                else
                {
                    _matchingInterval = value;
                }
            }
            get {
                return _matchingInterval;
            }
        }

        public int MatchingThreshold {
            set {
                if (value < 80)
                {
                    _matchingThreshold = 80;
                }
                else if (value > 100)
                {
                    _matchingThreshold = 100;
                }
                else
                {
                    _matchingThreshold = value;
                }
            }
            get {
                return _matchingThreshold;
            }
        }

        public bool WaitExit {
            get { return _waitExit; }
            set { _waitExit = value; }
        }

        private Rectangle Rect0 {
            get { return new Rectangle(0, 0, 0, 0); }
        }

        public void Click(string templateImagePath, int retryTimes = 600)
        {
            ActionOnImage(CvActionType.LClick, templateImagePath, retryTimes, Rect0);
        }

        public void DoubleClick(string templateImagePath, int retryTimes = 600)
        {
            ActionOnImage(CvActionType.DoubleClick, templateImagePath, retryTimes, Rect0);
        }

        public void RightClick(string templateImagePath, int retryTimes = 600)
        {
            ActionOnImage(CvActionType.RightClick, templateImagePath, retryTimes, Rect0);
        }

        public void Hover(string templateImagePath, int retryTimes = 600)
        {
            ActionOnImage(CvActionType.Move, templateImagePath, retryTimes, Rect0);
        }

        public void Wait(string templateImagePath, int retryTimes = 600)
        {
            ActionOnImage(CvActionType.DoNothing, templateImagePath, retryTimes, Rect0);
        }

        internal CvResult ActionOnImage(CvActionType actionType, string templateImagePath, int retryTimes, Rectangle rect, string sourceImagePath = "")
        {
            retryTimes = retryTimes < 0 ? _retryTimes : retryTimes;
            return DoCVAction(actionType, templateImagePath, _matchingThreshold, _matchingInterval, retryTimes, rect, sourceImagePath);
        }

        internal CvResult DoCVAction(CvActionType actionType,
                                        string templateImagePath,
                                        int matchingThreshold,
                                        int matchingInterval,
                                        int retry,
                                        Rectangle rect,
                                        string sourceImagePath = "")
        {
            if (File.Exists(templateImagePath))
            {
                var psi = new ProcessStartInfo();
                psi.FileName = _FlaxCV_exe;
                psi.WindowStyle = ProcessWindowStyle.Normal;
                psi.UseShellExecute = false;
                int minimizeCucumber = 0;
                int width = rect.Width;
                int height = rect.Height;
                var psb = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                if (width <= 0 || height <= 0)
                {
                    // Get full screen capture if capture width <= 0.
                    width = psb.Width;
                    height = psb.Height;
                }
                if (rect.X >= psb.Width - 300 && rect.Y >= psb.Height - 250)
                {
                    // Minimize Flax.CV.exe if search area might overlaps on it. 
                    minimizeCucumber = -1;
                }
                int a = (int)actionType;
                string sArgs = string.Format(@"{0} ""{1}"" {2} {3} {4} {5} {6} {7} {8} {9} ""{10}""",
                                            (int)actionType,       // Command
                                            templateImagePath,     // Template image path
                                            matchingThreshold,     // Matching threshold
                                            matchingInterval,      // Matching interval
                                            retry,                 // Number of retry
                                            minimizeCucumber,
                                            rect.X,                // Capture start point x
                                            rect.Y,                // Capture start point y
                                            width,                 // Capture width
                                            height,                // Capture height
                                            sourceImagePath);      // Compared image path
                psi.Arguments = sArgs;
                var p = Process.Start(psi);

                int matchedLevel = 0;
                int posX = -1, posY = -1;
                if (_waitExit)
                {
                    p.WaitForExit();
                    int exitCode = p.ExitCode;
                    if (exitCode >= 0)
                    {
                        // Get 25 to 31bit value
                        matchedLevel = exitCode >> 24;
                        // Get 13 to 24bit value + offset x = detected x cordinate
                        posX = ((exitCode >> 12) & 0xFFF) + rect.X;
                        // Get 1 to 12bit value + offset y = detected y cordinate
                        posY = (exitCode & 0xFFF) + rect.Y;
                    }
                }
                p.Close();
                p.Dispose();
                bool isMatched = matchedLevel >= _matchingThreshold;
                return new CvResult(matchedLevel, posX, posY, isMatched);
            }
            return new CvResult();
        }

        public class CvResult
        {
            public bool IsMatched;
            public int MatchedLevel;
            public Point Pt;

            public CvResult(int matchedLev = 0, int posX = -1, int posY = -1, bool isMatched = false)
            {
                MatchedLevel = matchedLev;
                Pt = new Point(posX, posY);
                IsMatched = isMatched;
            }

            // http://stackoverflow.com/questions/21485644/implicit-bool-and-operator-override-handle-if-statements-correctly
            public static implicit operator bool(CvResult cr)
            {
                return !ReferenceEquals(cr, null) && cr.IsMatched;
            }
        }

    }
}
