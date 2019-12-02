using System.Activities;
using System.ComponentModel;
using System;
using Plugins.Shared.Library;
using System.Security;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace RPA.Core.Activities.OrchestratorActivity
{
    [Designer(typeof(SetCredentialDesigner))]
    public sealed class SetCredential : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Set Credential";
            }
        }

        public SetCredential()
        {
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Core.Activities;Component/Resources/Orchestrator/Credential.png"; } }

        public InArgument<Int32> _TimeoutMS = 30000;
        [Category("选项")]
        [DisplayName("超时时间(毫秒)")]
        [Browsable(true)]
        [Description(" 指定在引发错误之前等待活动运行的时间量（以毫秒为单位）,默认值为30000毫秒（30秒）")]
        public InArgument<Int32> TimeoutMS
        {
            get
            {
                return _TimeoutMS;
            }
            set
            {
                _TimeoutMS = value;
            }
        }

        [Category("选项")]
        [DisplayName("错误继续执行")]
        [Browsable(true)]
        [Description(" 指定即使活动引发错误, 自动化是否仍应继续")]
        public InArgument<bool> errorContinue
        {
            get;
            set;
        }

        [Category("凭证选项")]
        [RequiredArgument]
        [DisplayName("用户名")]
        [Browsable(true)]
        [Description(" 设置凭证的用户名")]
        public InArgument<string> UserName
        {
            get;
            set;
        }

        [Category("凭证选项")]
        [RequiredArgument]
        [DisplayName("密码")]
        [Browsable(true)]
        [Description(" 设置凭证的密码")]
        public InArgument<SecureString> PassWord
        {
            get;
            set;
        }

        [Category("凭证选项")]
        [RequiredArgument]
        [DisplayName("凭证名称")]
        [Browsable(true)]
        [Description(" 要更新的凭据名称")]
        public InArgument<string> CredentialName
        {
            get;
            set;
        }

        [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        //增加凭据 
        static extern bool CredWrite([In] ref NativeCredential userCredential, [In] UInt32 flags);

        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                Int32 _timeout = TimeoutMS.Get(context);
                Thread.Sleep(_timeout);
                latch = new CountdownEvent(1);
                Thread td = new Thread(() =>
                {
                    string credName = CredentialName.Get(context);
                    string userName = UserName.Get(context);
                    SecureString credentialName = PassWord.Get(context);
                    IntPtr inP = Marshal.SecureStringToBSTR(credentialName);
                    string passWord = Marshal.PtrToStringBSTR(inP);
                    System.Diagnostics.Debug.WriteLine(" secureStr.ToString() : " + passWord);
                    WriteCred(credName, userName, passWord, CRED_TYPE.GENERIC, CRED_PERSIST.LOCAL_MACHINE);

                    refreshData(latch);
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();
            }
            catch (Exception e)
            {
                bool isRunOnError = errorContinue.Get(context);
                if (isRunOnError)
                {
                    //错误继续运行代码段
                }
                else
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "设置凭证执行过程出错", e.Message);
                }
            }
        }

        public static int WriteCred(string key, string userName, string secret, CRED_TYPE type, CRED_PERSIST credPersist)
        {
            var byteArray = Encoding.Unicode.GetBytes(secret);
            if (byteArray.Length > 512)
                throw new ArgumentOutOfRangeException("The secret message has exceeded 512 bytes.");
            var cred = new Credential
            {
                TargetName = key,
                CredentialBlob = secret,
                CredentialBlobSize = (UInt32)Encoding.Unicode.GetBytes(secret).Length,
                AttributeCount = 0,
                Attributes = IntPtr.Zero,
                UserName = userName,
                Comment = null,
                TargetAlias = null,
                Type = type,
                Persist = credPersist
            };
            var ncred = NativeCredential.GetNativeCredential(cred);
            var written = CredWrite(ref ncred, 0);
            var lastError = Marshal.GetLastWin32Error();
            if (written)
            {
                return 0;
            }

            var message = "";
            if (lastError == 1312)
            {
                message = (string.Format("Failed to save " + key + " with error code {0}.", lastError)
                + "  This error typically occurrs on home editions of Windows XP and Vista.  Verify the version of Windows is Pro/Business or higher.");
            }
            else
            {
                message = string.Format("Failed to save " + key + " with error code {0}.", lastError);
            }
            System.Diagnostics.Debug.WriteLine("Error:" + message);
            return 1;
        }
    }
}
