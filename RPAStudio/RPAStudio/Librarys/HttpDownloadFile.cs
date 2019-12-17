using GalaSoft.MvvmLight.Messaging;
using log4net;
using RPAStudio.Librarys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace RPAUpdate.Librarys
{
    class HttpDownloadFile
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void OnRunningChangedDelegate(HttpDownloadFile obj);
        public OnRunningChangedDelegate OnRunningChanged { get; set; }

        public delegate void OnDownloadFinishedDelegate(HttpDownloadFile obj);
        public OnDownloadFinishedDelegate OnDownloadFinished { get; set; }

        public delegate void OnDownloadingDelegate(HttpDownloadFile obj);
        public OnDownloadingDelegate OnDownloading { get; set; }


        public long FileTotalBytes { get; set; }//待下载的文件总大小
        public long FileDownloadedBytes { get; set; }//已下载的文件大小

        public bool IsDownloadSuccess { get; set; }

        public string Url { get; set; }
        public string SaveFilePath { get; set; }

        private bool _isStop;
        private bool _isRunning;

        public void Stop()
        {
            _isStop = true;
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }

            set
            {
                if(_isRunning == value)
                {
                    return;
                }

                _isRunning = value;

                OnRunningChanged?.Invoke(this);

            }
        }

        /// <summary>
        /// Http方式下载文件
        /// </summary>
        /// <param name="url">http地址</param>
        /// <param name="localfile">本地文件</param>
        /// <returns></returns>
        public bool Download(string url, string localfile)
        {
            Url = url;
            SaveFilePath = localfile;

            long startPosition = 0; // 上次下载的文件起始位置
            FileStream writeStream = null; // 写入本地文件流对象

            try
            {
                // 判断要下载的文件夹是否存在
                if (File.Exists(localfile))
                {
                    writeStream = File.OpenWrite(localfile);             // 存在则打开要下载的文件
                    startPosition = writeStream.Length;                  // 获取已经下载的长度
                    writeStream.Seek(startPosition, SeekOrigin.Current); // 本地文件写入位置定位
                }
                else
                {
                    writeStream = new FileStream(localfile, FileMode.Create);// 文件不保存创建一个文件
                    startPosition = 0;
                }


                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url);// 打开网络连接

                if (startPosition > 0)
                {
                    myRequest.AddRange((int)startPosition);// 设置Range值,与上面的writeStream.Seek用意相同,是为了定义远程文件读取位置
                }

                var myResponse = (HttpWebResponse)myRequest.GetResponse();
                FileTotalBytes = myResponse.ContentLength;

                FileDownloadedBytes = startPosition;

                Stream readStream = myRequest.GetResponse().GetResponseStream();// 向服务器请求,获得服务器的回应数据流

                byte[] btArray = new byte[512];// 定义一个字节数据,用来向readStream读取内容和向writeStream写入内容
                int contentSize = readStream.Read(btArray, 0, btArray.Length);// 向远程文件读第一次

                IsRunning = true;

                while (contentSize > 0)// 如果读取长度大于零则继续读
                {
                    if(_isStop)
                    {
                        break;
                    }

                    writeStream.Write(btArray, 0, contentSize);// 写入本地文件
                    FileDownloadedBytes += contentSize;
                    OnDownloading?.Invoke(this);

                    contentSize = readStream.Read(btArray, 0, btArray.Length);// 继续向远程文件读取
                }

                //关闭流
                writeStream.Close();
                readStream.Close();

                IsDownloadSuccess = true;        //返回true下载成功
            }
            catch (Exception e)
            {
                Logger.Error(e,logger);

                if(writeStream != null)
                {
                    writeStream.Close();
                }
                
                IsDownloadSuccess = false;       //返回false下载失败
            }

            IsRunning = false;

            OnDownloadFinished?.Invoke(this);

            return IsDownloadSuccess;
        }

    }
}
