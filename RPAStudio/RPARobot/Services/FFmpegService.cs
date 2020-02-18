using log4net;
using RPARobot.Librarys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Services
{
    /// <summary>
    /// ffmpeg录像服务类
    /// </summary>
    public class FFmpegService
    {
        /// <summary>
        /// 日志实例
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 录像保存位置
        /// </summary>
        private string _screenCaptureSavePath;

        /// <summary>
        /// 控制台管理类
        /// </summary>
        private CLIManager _cliManager;

        //fps值
        private string _fps;

        //crf值
        private string _crf;

        /// <summary>
        /// ffmpeg.exe所在路径，默认为当前目录
        /// </summary>
        private string FFmpegPath
        {
            get
            {
                return System.Environment.CurrentDirectory + @"\ffmpeg.exe";
            }
        }

        /// <summary>
        /// 录像参数
        /// </summary>
        private string CaptureScreenOption
        {
            get
            {
                return $" -y -rtbufsize 150M -f gdigrab -framerate {_fps}  -draw_mouse 1 -i desktop -c:v libx264 -r {_fps} -preset ultrafast -tune zerolatency -crf {_crf} -pix_fmt yuv420p -movflags +faststart ";
            }
        }



        public FFmpegService(string screenCaptureSavePath,string fps,string quality)
        {
            _screenCaptureSavePath = screenCaptureSavePath;
            _fps = fps;
            _crf = ((100-Convert.ToInt32(quality))/2).ToString();
        }

        /// <summary>
        /// 开始屏幕录像
        /// </summary>
        public void StartCaptureScreen()
        {
            if (_cliManager != null)
            {
                _cliManager.Dispose();
                _cliManager = null;
            }

            _cliManager = new CLIManager();

            var args = $"{CaptureScreenOption} \"{_screenCaptureSavePath}\"";
            Logger.Debug("ffmpeg录像开始，参数=" + args, logger);
            _cliManager.Open(FFmpegPath, args);
        }

        /// <summary>
        /// 屏幕录像是否正在运行
        /// </summary>
        /// <returns>是否运行</returns>
        public bool IsRunning()
        {
            if(_cliManager == null)
            {
                return false;
            }

            return _cliManager.IsProcessRunning();
        }

        /// <summary>
        /// 停止屏幕录像
        /// </summary>
        public void StopCaptureScreen()
        {
            if (_cliManager != null)
            {
                _cliManager.WaitForClose("q");
                _cliManager.Dispose();
                _cliManager = null;

                Logger.Debug("ffmpeg录像结束", logger);
            }
        }

    }
}
