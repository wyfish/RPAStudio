using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace RPA.Core.Activities.EnvironmentActivity
{
    [Designer(typeof(WinBeepDesigner))]
    public sealed class WinBeep : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Beep";
            }
        }

        public enum BeepTypes
        {
            蜂鸣,
            系统提示
        }

        [Category("选项")]
        [DisplayName("提示音类型")]
        [Browsable(true)]
        public BeepTypes BeepType
        {
            get;
            set;
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Core.Activities;Component/Resources/Environment/beep.png"; } }

        [DllImport("kernel32.dll")]
        public static extern bool Beep(int frequency, int duration);

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                if(BeepType == BeepTypes.蜂鸣)
                    Beep(2000, 500);
                else if(BeepType == BeepTypes.系统提示)
                    System.Media.SystemSounds.Asterisk.Play();//噔楞噔 警示音
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL执行过程出错", e.Message);
                throw e;
            }
        }
    }
}
