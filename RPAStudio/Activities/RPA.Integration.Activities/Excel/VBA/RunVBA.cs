using Plugins.Shared.Library;
using Plugins.Shared.Library.Editors;
using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using Excel = Microsoft.Office.Interop.Excel;


namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(RunVBADesigner))]
    public sealed class RunVBA : AsyncCodeActivity
    {
        public RunVBA()
        {
            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(RunVBA), nameof(RunVBA.Parameters), new EditorAttribute(typeof(DictionaryArgumentEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/vba.png"; } }

        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName43")] //VBA名称 //VBA name //VBA名
        [Browsable(true)]
        public InArgument<string> VBAName
        {
            get;set;
        }

        private Dictionary<string, Argument> parameters;
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName10")] //参数 //parameter //パラメータ
        public Dictionary<string, Argument> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new Dictionary<string, Argument>();
                }
                return this.parameters;
            }
            set
            {
                parameters = value;
            }
        }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName44")] //返回值 //return value //戻り値
        [Browsable(true)]
        public OutArgument<string> ReturnValue
        {
            get; set;
        }



        [Browsable(false)]
        public string ClassName { get { return "RunVBA"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            PropertyDescriptor property = context.DataContext.GetProperties()[ExcelCreate.GetExcelAppTag];
            Excel::Application excelApp = property.GetValue(context.DataContext) as Excel::Application;
            try
            {
                if (string.IsNullOrEmpty(VBAName.Get(context)))
                {
                    throw new System.Exception("请输入宏的名称");
                }
        
                object returnValue;
                string macroName = VBAName.Get(context);
                object[] parameters = null;
                if (Parameters != null)
                {
                    List<object> paraList = new List<object>();
                    foreach (var param in Parameters)
                    {
                        paraList.Add(param.Value.Get(context));
                    }
                    parameters = paraList.ToArray();
                }
                RunExcelMacro(excelApp, macroName, parameters, out returnValue);
                if (returnValue != null)
                    ReturnValue.Set(context, returnValue);
            }
            catch(Exception e)
            {
                new CommonVariable().realaseProcessExit(excelApp);
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL执行VBA出错", e.Message);
            }
            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            
        }

        public void RunExcelMacro(Excel::Application excelApp, string macroName,object[] parameters, out object rtnValue)
        {
            object oMissing = System.Reflection.Missing.Value;
            object[] paraObjects;
            if (parameters == null)
            {
                paraObjects = new object[] { macroName };
            }
            else
            {
                // 宏参数组长度  
                int paraLength = parameters.Length;

                paraObjects = new object[paraLength + 1];

                paraObjects[0] = macroName;
                for (int i = 0; i < paraLength; i++)
                {
                    paraObjects[i + 1] = parameters[i];
                }
            }

            rtnValue = "";
            try
            {
                rtnValue = excelApp.GetType().InvokeMember(
                    "Run",
                    System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod,
                    null,
                    excelApp,
                    paraObjects
                );
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "宏执行异常,请检查宏名称与参数是否匹配", e.Message);
            }
            Excel._Workbook oBook = excelApp.ActiveWorkbook;
            oBook.Save();
        }
    }   
}
