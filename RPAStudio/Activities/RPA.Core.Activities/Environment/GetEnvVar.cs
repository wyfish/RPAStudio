using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace RPA.Core.Activities.EnvironmentActivity
{
    [Designer(typeof(GetEnvVarDesigner))]
    public sealed class GetEnvVar : CodeActivity
    {
        //系统变量例举
        public enum EnvVarEnums
        {
            TickCount,
            ExitCode,
            CommandLine,
            CurrentDirectory,
            SystemDirectory,
            MachineName,
            ProcessorCount,
            SystemPageSize,
            NewLine,
            Version,
            WorkingSet,
            OSVersion,
            StackTrace,
            Is64BitProcess,
            Is64BitOperatingSystem,
            HasShutdownStarted,
            UserName,
            UserInteractive,
            UserDomainName,
            CurrentManagedThreadId
        }

        public new string DisplayName
        {
            get
            {
                return "Get Environment Variable";
            }
        }

        [Browsable(false)]
        public IEnumerable<EnvVarEnums> EnvVarPro
        {
            get
            {
                return Enum.GetValues(typeof(EnvVarEnums)).Cast<EnvVarEnums>();
            }
        }

        [Category("选项")]
        [RequiredArgument]
        [DisplayName("变量名称")]
        [Browsable(true)]
        public InArgument<string> EnvVarName
        {
            get;
            set;
        }

        [Category("选项")]
        [RequiredArgument]
        [DisplayName("变量值")]
        [Browsable(true)]
        public OutArgument<string> EnvVarValue
        {
            get;
            set;
        }
  

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Core.Activities;Component/Resources/Environment/envvariable.png"; } }


        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string envVar = EnvVarName.Get(context);
                string envVarValue = "";
                switch(envVar)
                {
                    case "TickCount":
                        {
                            envVarValue = Environment.TickCount.ToString();
                            break;
                        }
                    case "ExitCode":
                        {
                            envVarValue = Environment.ExitCode.ToString();
                            break;
                        }
                    case "CommandLine":
                        {
                            envVarValue = Environment.CommandLine.ToString();
                            break;
                        }
                    case "CurrentDirectory":
                        {
                            envVarValue = Environment.CurrentDirectory.ToString();
                            break;
                        }
                    case "SystemDirectory":
                        {
                            envVarValue = Environment.SystemDirectory.ToString();
                            break;
                        }
                    case "MachineName":
                        {
                            envVarValue = Environment.MachineName.ToString();
                            break;
                        }
                    case "ProcessorCount":
                        {
                            envVarValue = Environment.ProcessorCount.ToString();
                            break;
                        }
                    case "SystemPageSize":
                        {
                            envVarValue = Environment.SystemPageSize.ToString();
                            break;
                        }
                    case "NewLine":
                        {
                            envVarValue = Environment.NewLine.ToString();
                            break;
                        }
                    case "Version":
                        {
                            envVarValue = Environment.Version.ToString();
                            break;
                        }
                    case "WorkingSet":
                        {
                            envVarValue = Environment.WorkingSet.ToString();
                            break;
                        }
                    case "OSVersion":
                        {
                            envVarValue = Environment.OSVersion.ToString();
                            break;
                        }
                    case "StackTrace":
                        {
                            envVarValue = Environment.StackTrace.ToString();
                            break;
                        }
                    case "Is64BitProcess":
                        {
                            envVarValue = Environment.Is64BitProcess.ToString();
                            break;
                        }
                    case "Is64BitOperatingSystem":
                        {
                            envVarValue = Environment.Is64BitOperatingSystem.ToString();
                            break;
                        }
                    case "HasShutdownStarted":
                        {
                            envVarValue = Environment.HasShutdownStarted.ToString();
                            break;
                        }
                    case "UserName":
                        {
                            envVarValue = Environment.UserName.ToString();
                            break;
                        }
                    case "UserInteractive":
                        {
                            envVarValue = Environment.UserInteractive.ToString();
                            break;
                        }
                    case "UserDomainName":
                        {
                            envVarValue = Environment.UserDomainName.ToString();
                            break;
                        }
                    case "CurrentManagedThreadId":
                        {
                            envVarValue = Environment.CurrentManagedThreadId.ToString();
                            break;
                        }
                    default:
                        {
                            envVarValue = Environment.GetEnvironmentVariable(envVar);
                            break;
                        }
                }
                EnvVarValue.Set(context, envVarValue);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取系统变量执行过程出错", e.Message);
                throw e;
            }
        }
    }
}
