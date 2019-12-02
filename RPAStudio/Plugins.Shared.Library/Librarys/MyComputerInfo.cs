using System;
using System.Management;
using Microsoft.VisualBasic.Devices;
using System.Collections.Generic;

namespace Plugins.Shared.Library.Librarys
{
    public class MyComputerInfo
    {
        public string CpuID;
        public string MacAddress;
        public List<string> MacAddressList;
        public string DiskID;
        public string IpAddress;
        public string LoginUserName;
        public string ComputerName;
        public string SystemType;
        public string TotalPhysicalMemory; //单位：字节

        public string OSVersion;
        public string OSFullName;

        private static MyComputerInfo _instance;
        public static MyComputerInfo Instance()
        {
            if (_instance == null)
                _instance = new MyComputerInfo();
            return _instance;
        }
        ///
        /// 构造函数
        ///
        private MyComputerInfo()
        {
            CpuID = GetCpuID();
            MacAddressList = GetMacAddressList();
            MacAddress = GetMacAddress();
            DiskID = GetDiskID();
            IpAddress = GetIPAddress();
            LoginUserName = GetUserName();
            SystemType = GetSystemType();
            TotalPhysicalMemory = GetTotalPhysicalMemory();
            ComputerName = GetComputerName();

            GetOSVersionInfo();
        }

        private void GetOSVersionInfo()
        {
            try
            {
                var computer = new ComputerInfo();
                OSVersion = computer.OSVersion;//6.1.7601.65536
                OSFullName = computer.OSFullName;//Microsoft Windows 7 Ultimate
            }
            catch (Exception)
            {

            }
        }

        ///
        /// 获取cpu序列号
        ///
        ///
        string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码   
                string cpuInfo = "";//cpu序列号   
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }


        public bool IsExistMacAddress(string mac)
        {
            if(MacAddressList.Count == 0)
            {
                //找不到任何网卡地址，说明取网址列表发生错误，不再用网卡进行判断，直接返回真
                return true;
            }

            foreach(var item in MacAddressList)
            {
                if(item  == mac)
                {
                    return true;
                }
            }

            return false;
        }

        List<string> GetMacAddressList()
        {
            var macList = new List<string>();

            try
            {
                //获取网卡硬件地址列表
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        var mac = mo["MacAddress"].ToString();
                        macList.Add(mac);
                    }   
                }
                moc = null;
                mc = null;

                return macList;
            }
            catch
            {
                
            }

            return macList;
        }


        ///
        /// 获取网卡硬件地址  
        ///
        ///
        string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址   
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        ///
        /// 获取IP地址
        ///
        ///
        string GetIPAddress()
        {
            try
            {
                //获取IP地址   
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        //st=mo["IpAddress"].ToString();   
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        ///
        /// 获取硬盘ID 
        ///
        ///
        string GetDiskID()
        {
            try
            {
                //获取硬盘ID   
                String HDid = "";
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    HDid = (string)mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        ///    
        /// 操作系统的登录用户名   
        ///    
        ///    
        string GetUserName()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["UserName"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }


        ///    
        /// PC类型   
        ///    
        ///    
        string GetSystemType()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["SystemType"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        ///    
        /// 物理内存   
        ///    
        ///    
        string GetTotalPhysicalMemory()
        {
            try
            {

                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["TotalPhysicalMemory"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }
        ///    
        ///    电脑名称
        ///    
        ///    
        string GetComputerName()
        {
            try
            {
                return System.Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

    }
}
