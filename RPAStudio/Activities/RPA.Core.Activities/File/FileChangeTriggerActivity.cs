using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace RPA.Core.Activities.FileActivity
{
    [Designer(typeof(FileChangeTriggerActivityDesigner))]
    public sealed class FileChangeTriggerActivity : NativeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "File Change Trigger";
            }
        }

        [Browsable(false)]
        private BookmarkResumptionHelper BookmarkResumptionHelper;
        [Browsable(false)]
        private Bookmark RuntimeBookmark;

        [Category("File")]
        [Browsable(true)]
        [DisplayNameAttribute("FileName")]
        [Description("要监视更改的文件的名称")]  //The name of the file to be watched for changes
        public InArgument<string> FileName { get; set; }

        [Category("File")]
        [Browsable(true)]
        [DisplayNameAttribute("Path")]
        [Description("要监视的文件的路径，以查找更改。")]  //The path of the file to be watched for changes.
        public InArgument<string> Path { get; set; }

        [Category("Event")]
        [Browsable(true)]
        public string ChangeType { get; set; }

        [Category("Event")]
        [Browsable(true)]
        [DisplayNameAttribute("NotifyFilters")]
        public string _NotifyFilters { get; set; }

        [Category("Event")]
        [Browsable(true)]
        [DisplayNameAttribute("包含子目录")]
        public bool IncludeSubdirectories  { get; set; }

        [Browsable(false)]
        private FileSystemWatcher FileSystemWatcher;

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/File/appline.png";
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.RequireExtension<BookmarkResumptionHelper>();
            metadata.AddDefaultExtensionProvider<BookmarkResumptionHelper>(() => new BookmarkResumptionHelper());
            metadata.AddArgument(new RuntimeArgument("Path", typeof(string), ArgumentDirection.In, true));
            if (this.ChangeType == "")
            {
                metadata.AddValidationError("Type 为空!");
            }
            if (this._NotifyFilters == "")
            {
                metadata.AddValidationError("Type 为空!");
            }
            base.CacheMetadata(metadata);
        }

        public FileChangeTriggerActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(FileChangeTriggerActivity), "ChangeType", new EditorAttribute(typeof(ChangeTypeEditor), typeof(PropertyValueEditor)));
            builder.AddCustomAttributes(typeof(FileChangeTriggerActivity), "_NotifyFilters", new EditorAttribute(typeof(NotifyFiltersEditor), typeof(PropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        protected override void Execute(NativeActivityContext context)
        {
            this.RuntimeBookmark = (context.Properties.Find("MonitorBookmark") as Bookmark);
            if (this.RuntimeBookmark == null)
            {
                return;
            }
            this.BookmarkResumptionHelper = context.GetExtension<BookmarkResumptionHelper>();
            this.StartMonitor(context);
            context.CreateBookmark();
        }

        private NotifyFilters getCboInfo()
        {
            NotifyFilters exp = new NotifyFilters();
            if(_NotifyFilters.Contains("LastAccess") && _NotifyFilters.Contains("LastWrite"))
                exp = (NotifyFilters)((NotifyFilters.FileName) | (NotifyFilters.DirectoryName) | (NotifyFilters.LastAccess) | (NotifyFilters.LastWrite));
            if (_NotifyFilters.Contains("LastWrite") &&!_NotifyFilters.Contains("LastAccess"))
                exp = (NotifyFilters)((NotifyFilters.FileName) | (NotifyFilters.DirectoryName) | (NotifyFilters.LastWrite));
            if (_NotifyFilters.Contains("LastAccess") && !_NotifyFilters.Contains("LastWrite"))
                exp = (NotifyFilters)((NotifyFilters.FileName) | (NotifyFilters.DirectoryName) | (NotifyFilters.LastAccess));
            if (!_NotifyFilters.Contains("LastAccess") && !_NotifyFilters.Contains("LastWrite"))
                exp = (NotifyFilters)((NotifyFilters.FileName) | (NotifyFilters.DirectoryName));
            return exp;
        }

        void StartMonitor(NativeActivityContext context)
        {
            string path = this.Path.Get(context);
            string text = this.FileName.Get(context);
            if ((File.GetAttributes(path) & FileAttributes.Directory) != FileAttributes.Directory && string.IsNullOrEmpty(text))
            {
                text = System.IO.Path.GetFileName(path);
                path = System.IO.Path.GetDirectoryName(path);
            }
            this.FileSystemWatcher = new FileSystemWatcher(path);
            this.FileSystemWatcher.Filter = text;
            this.FileSystemWatcher.IncludeSubdirectories = this.IncludeSubdirectories;
                FileSystemWatcher.NotifyFilter = getCboInfo();

            this.FileSystemWatcher.NotifyFilter = NotifyFilters.FileName|NotifyFilters.Size;
            if (this.ChangeType.Contains("All") || this.ChangeType.Contains("Changed"))
            {
                this.FileSystemWatcher.Changed += new FileSystemEventHandler(this.Event_Trigger);
            }
            if (this.ChangeType.Contains("All") || this.ChangeType.Contains("Created"))
            {
                this.FileSystemWatcher.Created += new FileSystemEventHandler(this.Event_Trigger);
            }
            if (this.ChangeType.Contains("All") || this.ChangeType.Contains("Renamed"))
            {
                this.FileSystemWatcher.Renamed += new RenamedEventHandler(this.Event_Trigger);
            }
            if (this.ChangeType.Contains("All") || this.ChangeType.Contains("Deleted"))
            {
                this.FileSystemWatcher.Deleted += new FileSystemEventHandler(this.Event_Trigger);
            }
            this.FileSystemWatcher.EnableRaisingEvents = true;
        }
        void StopMonitor(ActivityContext context)
        {
            try
            {
                if (this.FileSystemWatcher != null)
                {
                    this.FileSystemWatcher.EnableRaisingEvents = false;
                    this.FileSystemWatcher.Dispose();
                    this.FileSystemWatcher = null;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }
        protected override void Cancel(NativeActivityContext context)
        {
            this.StopMonitor(context);
            base.Cancel(context);
        }
        protected override void Abort(NativeActivityAbortContext context)
        {
            this.StopMonitor(context);
            base.Abort(context);
        }
        void Event_Trigger(object sender, object args)
        {
            if (this.RuntimeBookmark != null)
            {
                this.BookmarkResumptionHelper.BeginResumeBookmark(RuntimeBookmark, args);
            }
        }
    }
}
