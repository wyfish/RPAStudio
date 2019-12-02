using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library.Librarys;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(ClearOutlookCacheDesigner))]
    public sealed class ClearOutlookCacheActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            //%LocalAppData%\Microsoft\Outlook\RoamCache下的文件删除
            var localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            var RoamCache = localAppData + @"\Microsoft\Outlook\RoamCache";
            if (System.IO.Directory.Exists(RoamCache))
            {
                Common.DeleteDir(RoamCache);
                System.IO.Directory.CreateDirectory(RoamCache);
            }
        }
    }
}
