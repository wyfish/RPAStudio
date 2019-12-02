using System;
using System.Activities;
using System.Activities.Hosting;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Plugins.Shared.Library
{
    public class BookmarkResumptionHelper : IWorkflowInstanceExtension
    {
        private WorkflowInstanceProxy _instance;
        internal void ResumeBookmark(Bookmark bookmark, object value)
        {
            this._instance.EndResumeBookmark(this._instance.BeginResumeBookmark(bookmark, value, null, null));
        }
        public void BeginResumeBookmark(Bookmark bookmark, object value)
        {
            this._instance.BeginResumeBookmark(bookmark, value, null, null);
        }
        
        public IEnumerable<object> GetAdditionalExtensions()
        {
            return null;
        }

        public void SetInstance(WorkflowInstanceProxy instance)
        {
            this._instance = instance;
        }

        public static BookmarkResumptionHelper Create()
        {
            return new BookmarkResumptionHelper();
        }
    }
}
