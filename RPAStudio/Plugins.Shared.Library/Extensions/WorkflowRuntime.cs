using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Shared.Library.Extensions
{
    public class WorkflowRuntime
    {
        public Activity RootActivity { get; set; }

        public Activity GetRootActivity()
        {
            return RootActivity;
        }
    }
}
