using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RPA.Core.Activities.EnvironmentActivity
{
    [Designer(typeof(GetEnvPathDesigner))]
    public sealed class GetEnvPath : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Get Environment Folder";
            }
        }

        Environment.SpecialFolder _selectedMyEnumType = Environment.SpecialFolder.AdminTools;
        [Category("选项")]
        [DisplayName("系统文件夹")]
        [Browsable(true)]
        public Environment.SpecialFolder FolderType
        {
            get { return _selectedMyEnumType; }
            set { _selectedMyEnumType = value; }
        }

        [Browsable(false)]
        public IEnumerable<Environment.SpecialFolder> FolderTypePro
        {
            get
            {
                //return Enum.GetValues(typeof(Environment.SpecialFolder)) as IEnumerable<Environment.SpecialFolder>;
                return Enum.GetValues(typeof(Environment.SpecialFolder)).Cast<Environment.SpecialFolder>();
            }
        }

        [Category("输出")]
        [RequiredArgument]
        [DisplayName("文件夹路径")]
        [Browsable(true)]
        public OutArgument<string> FolderPath
        {
            get;
            set;
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Core.Activities;Component/Resources/Environment/envfolder.png"; } }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                String Path = Environment.GetFolderPath(FolderType);
                FolderPath.Set(context, Path);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取系统文件夹执行过程出错", e.Message);
                throw e;
            }
        }
    }
}
