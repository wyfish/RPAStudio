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
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName49")] //超时时间(毫秒) //Timeout (ms) //タイムアウト（ミリ秒）
        [Browsable(true)]
        [Localize.LocalizedDescription("Description107")] // 指定在引发错误之前等待活动运行的时间量（以毫秒为单位）,默认值为30000毫秒（30秒） //Specifies the amount of time, in milliseconds, to wait for an activity to run before an error is raised. The default value is 30000 milliseconds (30 seconds) //エラーが発生する前にアクティビティの実行を待機する時間をミリ秒単位で指定しますデフォルト値は30000ミリ秒（30秒）です
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

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName51")] //错误继续执行 //Error continues to execute //エラーは引き続き実行されます
        [Browsable(true)]
        [Localize.LocalizedDescription("Description108")] // 指定即使活动引发错误, 自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> errorContinue
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category10")] //凭证选项 //Credential option //資格情報オプション
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName50")] //用户名 //username //ユーザー名
        [Browsable(true)]
        [Localize.LocalizedDescription("Description109")] // 设置凭证的用户名 //Set the username of the credential //資格情報のユーザー名を設定します
        public InArgument<string> UserName
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category10")] //凭证选项 //Credential option //資格情報オプション
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName45")] //密码 //password //パスワード
        [Browsable(true)]
        [Localize.LocalizedDescription("Description110")] // 设置凭证的密码 //Set the password for the credential //資格情報のパスワードを設定します
        public InArgument<SecureString> PassWord
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category10")] //凭证选项 //Credential option //資格情報オプション
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName52")] //凭证名称 //Credential name //資格情報名
        [Browsable(true)]
        [Localize.LocalizedDescription("Description111")] // 要更新的凭据名称 //The name of the credential to update //更新する資格情報の名前
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
