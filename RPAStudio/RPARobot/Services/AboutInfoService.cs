using RPARobot.Librarys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Services
{
    /// <summary>
    /// 关于服务，提供关于产品信息的相关内容
    /// </summary>
    public class AboutInfoService
    {
        public AboutInfoService()
        {

        }

        /// <summary>
        /// 获取机器名
        /// </summary>
        /// <returns>机器名</returns>
        public string GetMachineName()
        {
            return Environment.MachineName;
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        public string GetIp()
        {
            try
            {
                IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress item in IpEntry.AddressList)
                {
                    if (item.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return item.ToString();
                    }
                }
                return "";
            }
            catch { return ""; }
        }

        /// <summary>
        /// 获取程序版本
        /// </summary>
        /// <returns>程序版本</returns>
        public string GetVersion()
        {
            return Common.GetProgramVersion();
        }
    }
}
