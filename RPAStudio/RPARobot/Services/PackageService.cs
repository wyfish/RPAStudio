using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPARobot.ViewModel;
using RPARobot.Librarys;

namespace RPARobot.Services
{
    /// <summary>
    /// 包服务
    /// </summary>
    public class PackageService
    {
        private MainViewModel mainViewModel;

        public PackageService(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        /// <summary>
        /// 运行指定名称和版本的包（停止其它正在运行的包）
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="version">版本</param>
        public void Run(string name,string version)
        {
            Common.RunInUI(() =>
            {
                string runningName, runningVersion;
                bool existRun = GetRunningPackage(out runningName, out runningVersion);

                if(existRun)
                {
                    if (runningName == name && runningVersion == version)
                    {
                        return;
                    }
                    else
                    {
                        StopCurrentRun();
                    }
                }
                

                RealRun(name, version);
            });
            
        }

        /// <summary>
        /// 实际的运行流程
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="version">版本</param>
        private void RealRun(string name, string version)
        {
            foreach(var item in mainViewModel.PackageItems)
            {
                if(item.Name == name && item.Version == version)
                {
                    if(item.IsNeedUpdate)
                    {
                        item.UpdateCommand.Execute(null);
                    }

                    item.StartCommand.Execute(null);

                    Common.RunInUI(() =>
                    {
                        mainViewModel.RefreshAllPackages();
                    });

                    break;
                }
            }
        }

        /// <summary>
        /// 获取当前正在运行的包
        /// </summary>
        /// <param name="name">当前正在运行的包名</param>
        /// <param name="version">当前正在运行的包版本</param>
        /// <returns>是否有正在运行的包</returns>
        private bool GetRunningPackage(out string name, out string version)
        {
            foreach (var item in mainViewModel.PackageItems)
            {
                if (item.IsRunning)
                {
                    name = item.Name;
                    version = item.Version;

                    return true;
                }
            }

            name = "";
            version = "";
            return false;
        }

        /// <summary>
        /// 停止当前正在运行的包
        /// </summary>
        private void StopCurrentRun()
        {
            mainViewModel.StopCommand.Execute(null);
        }


    }
}
