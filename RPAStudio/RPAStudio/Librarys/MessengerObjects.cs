using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPAStudio.ViewModel;

namespace RPAStudio.Librarys
{
    /// <summary>
    /// 一些自定义类型的对象，以便Messenger收发消息使用
    /// </summary>
    class MessengerObjects
    {
        //截图程序传消息用
        public class CopyData
        {
            public string data { get; private set; }

            public CopyData(string data)
            {
                this.data = data;
            }
        }

        public class ProjectOpen
        {
            public object Sender { get; set; }

            //project.json文件全路径
            public string ProjectJsonFile { get; set; }

            //默认自动打开的XAML文件
            public string DefaultOpenXamlFile { get; set; }
        }

        public class ProjectClose
        {

        }

        //项目名改变消息，主要给主程序标题栏用
        public class ProjectStateChanged
        {
            public bool IsOpen { get; set; }
            public string ProjectPath { get; set; }
            public string ProjectName { get; set; }

            public ProjectStateChanged()
            {
                IsOpen = true;
            }
        }

        public class RecentProjectsModify
        {

        }

    }
}
