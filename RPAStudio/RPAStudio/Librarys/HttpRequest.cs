using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RPAStudio.Librarys
{
    class HttpRequest
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string Post(string url)
        {
            string result = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取内容 
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, logger);
            }
            
            return result;
        }

        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                #region 添加Post 参数 
                StringBuilder builder = new StringBuilder();
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
                byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容 
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, logger);
            }

            return result;
        }


        public static string Get(string url)
        {
            string result = "";
            Stream stream = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                stream = resp.GetResponseStream();

                //获取内容 
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, logger);
            }
            finally
            {
                if(stream != null)
                {
                    stream.Close();
                }
            }
            return result;
        }


        public static string Get(string url, Dictionary<string, string> dic)
        {
            string result = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }

            Stream stream = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder.ToString());
                //添加参数 
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                stream = resp.GetResponseStream();

                //获取内容 
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, logger);
            }
            finally
            {
                if(stream != null)
                {
                    stream.Close();
                }
            }
            return result;
        }



    }
}
