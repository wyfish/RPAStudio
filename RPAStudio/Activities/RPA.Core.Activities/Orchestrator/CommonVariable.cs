using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace RPA.Core.Activities.OrchestratorActivity
{
    /// <summary>         
    /// 凭据类型         
    /// </summary> 
    public enum CRED_TYPE : uint
    {
        //普通凭据
        GENERIC = 1,
        //域密码 
        DOMAIN_PASSWORD = 2,
        //域证书 
        DOMAIN_CERTIFICATE = 3,
        //域可见密码 
        DOMAIN_VISIBLE_PASSWORD = 4,
        //一般证书 
        GENERIC_CERTIFICATE = 5,
        //域扩展 
        DOMAIN_EXTENDED = 6,
        //最大 
        MAXIMUM = 7,
        // Maximum supported cred type 
        MAXIMUM_EX = (MAXIMUM + 1000),  // Allow new applications to run on old OSes 
    }
    //永久性 
    public enum CRED_PERSIST : uint
    {
        All = 0,
        SESSION = 1,             //本地计算机 
        LOCAL_MACHINE = 2,             //企业 
        ENTERPRISE = 3,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct Credential
    {
        public UInt32 Flags;
        public CRED_TYPE Type;
        public string TargetName;
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public UInt32 CredentialBlobSize;
        public string CredentialBlob;
        public CRED_PERSIST Persist;
        public UInt32 AttributeCount;
        public IntPtr Attributes;
        public string TargetAlias;
        public string UserName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NativeCredential
    {
        public UInt32 Flags;
        public CRED_TYPE Type;
        public IntPtr TargetName;
        public IntPtr Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public UInt32 CredentialBlobSize;
        public IntPtr CredentialBlob;
        public UInt32 Persist;
        public UInt32 AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        public IntPtr UserName;


        internal static NativeCredential GetNativeCredential(Credential cred)
        {

            NativeCredential ncred = new NativeCredential();
            ncred.AttributeCount = 0;
            ncred.AttributeCount = 0;
            ncred.Attributes = IntPtr.Zero;
            ncred.Comment = IntPtr.Zero;
            ncred.TargetAlias = IntPtr.Zero;
            ncred.Type = cred.Type;
            ncred.Persist = (UInt32)cred.Persist;
            ncred.CredentialBlobSize = (UInt32)cred.CredentialBlobSize;
            ncred.TargetName = Marshal.StringToCoTaskMemUni(cred.TargetName);
            ncred.CredentialBlob = Marshal.StringToCoTaskMemUni(cred.CredentialBlob);
            ncred.UserName = Marshal.StringToCoTaskMemUni(cred.UserName);
            //                    var ncred = new NativeCredential
            //                    {
            //                        AttributeCount = 0,
            //                        Attributes = IntPtr.Zero,
            //                        Comment = IntPtr.Zero,
            //                        TargetAlias = IntPtr.Zero,
            //                        //Type = CRED_TYPE.DOMAIN_PASSWORD,                                         
            //                        Type = cred.Type,
            //                        Persist = (UInt32)cred.Persist,
            //                        CredentialBlobSize = (UInt32)cred.CredentialBlobSize,
            //                        TargetName = Marshal.StringToCoTaskMemUni(cred.TargetName),
            //                        CredentialBlob = Marshal.StringToCoTaskMemUni(cred.CredentialBlob),
            //                        UserName = Marshal.StringToCoTaskMemUni(cred.UserName)
            //                    };
            return ncred;
        }
    }

    public class CommonVariable
    {
        public static Object cellContent;       //存储单元格获取值
        public static bool isNewFile = true;    //创建新文件标志

        public static string getPropertyValue(string propertyName, ModelItem currItem)
        {
            List<ModelProperty> PropertyList = currItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
            return _property.Value.ToString();
        }
    }
}
