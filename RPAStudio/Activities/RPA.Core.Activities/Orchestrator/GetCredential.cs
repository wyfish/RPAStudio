using System.Activities;
using System.ComponentModel;
using System;
using Plugins.Shared.Library;
using System.Security;
using System.Runtime.InteropServices;
using System.Threading;

namespace RPA.Core.Activities.OrchestratorActivity
{
    [Designer(typeof(GetCredentialDesigner))]
    public sealed class GetCredential : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Get Credential";
            }
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Core.Activities;Component/Resources/Orchestrator/Credential.png"; } }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName48")] //凭据名称 //Credential name //資格情報名
        [Browsable(true)]
        [Localize.LocalizedDescription("Description103")] // 系统中获取的凭据名称 //The name of the credential obtained in the system //システムで取得した資格情報の名前
        public InArgument<string> CredentialName
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName49")] //超时时间(毫秒) //Timeout (ms) //タイムアウト（ミリ秒）
        [Browsable(true)]
        [Localize.LocalizedDescription("Description104")] // 指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒） //Specifies the amount of time, in milliseconds, to wait for an activity to run before an error is raised.  The default is 30000 milliseconds (30 seconds) //エラーが発生する前にアクティビティの実行を待機する時間をミリ秒単位で指定します。 デフォルトは30000ミリ秒（30秒）です
        public InArgument<Int32> TimeoutMS
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName45")] //密码 //password //パスワード
        [Browsable(true)]
        [Localize.LocalizedDescription("Description105")] // 检索到的凭证的安全密码 //The secure password of the retrieved credentials //取得した資格情報の安全なパスワード
        public OutArgument<SecureString> PassWord
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName50")] //用户名 //username //ユーザー名
        [Browsable(true)]
        [Localize.LocalizedDescription("Description106")] // 检索到的凭证的用户名 //User name of the retrieved voucher //取得したバウチャーのユーザー名
        public OutArgument<string> UserName
        {
            get;
            set;
        }


        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        //读取凭据信息 
        static extern bool CredRead(string target, CRED_TYPE type, int reservedFlag, out IntPtr CredentialPtr);

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
                    IntPtr credPtr = new IntPtr();
                    WReadCred(credName, CRED_TYPE.GENERIC, CRED_PERSIST.LOCAL_MACHINE, out credPtr);
                    Credential lRawCredential = (Credential)Marshal.PtrToStructure(credPtr, typeof(Credential));
                    SecureString securePassWord = new SecureString();
                    foreach (char c in lRawCredential.CredentialBlob)
                    {
                        securePassWord.AppendChar(c);
                    }
                    UserName.Set(context, lRawCredential.UserName);
                    PassWord.Set(context, securePassWord);

                    refreshData(latch);
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();
                //System.Diagnostics.Debug.WriteLine("UserName:" + lRawCredential.UserName);
                //System.Diagnostics.Debug.WriteLine("CredentialBlob:" + lRawCredential.CredentialBlob);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "读取凭证执行过程出错", e.Message);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredEnumerate(string filter, uint flag, out uint count, out IntPtr pCredentials);

        public static bool WReadCred(string targetName, CRED_TYPE credType, CRED_PERSIST reservedFlag, out IntPtr intPtr)
        {
            return CredRead(targetName, credType, (int)reservedFlag, out intPtr);
        }
    }
}
