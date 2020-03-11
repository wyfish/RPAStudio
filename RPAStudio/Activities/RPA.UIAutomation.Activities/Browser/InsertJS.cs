using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Threading;
using System.Windows;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections;
using Plugins.Shared.Library.UiAutomation;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Activities.Presentation.PropertyEditing;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Plugins.Shared.Library.Editors;

[assembly: InternalsVisibleTo("BrowserActivity.Microsoft.CSharp")]

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(InsertJSDesigner))]
    public sealed class InsertJS : AsyncCodeActivity
    {
        //构造函数自定义属性框
        public InsertJS()
        {
            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(InsertJS), nameof(InsertJS.Parameters), new EditorAttribute(typeof(ArgumentCollectionEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }


        public string _DisplayName { get { return "InsertJS"; } }


        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName17")] //延迟时间(结束) //Delay time (end) //遅延時間（終了）
        [Localize.LocalizedDescription("Description19")] //执行活动后的延迟时间(以毫秒为单位),默认时间为3000毫秒 //The delay (in milliseconds) after the activity is executed. The default time is 3000 milliseconds. //アクティビティが実行された後の遅延（ミリ秒）デフォルトの時間は3000ミリ秒です。
        public InArgument<Int32> DelayAfter { get; set; }

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName18")] //延迟时间(开始) //Delay time (start) //遅延時間（開始）
        [Localize.LocalizedDescription("Description20")] //活动开始执行任何操作之前的延迟时间(以毫秒为单位),默认的时间量是3000毫秒。 //The delay (in milliseconds) before the activity begins any operation, the default amount of time is 3000 milliseconds. //アクティビティが操作を開始するまでの遅延（ミリ秒）。デフォルトの時間は3000ミリ秒です。
        public InArgument<Int32> DelayBefore { get; set; }

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [DisplayName("循环执行")]
        [Description("JS运行失败是否重复执行直到执行成功或超时")]
        public bool isCircle { get; set; }


        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [DisplayName("错误超时时间")]
        [Description("JS脚本循环执行直到执行成功或者超过超时时间.默认为10000毫秒")]
        public InArgument<Int32> TimeoutMS { get; set; }


        //[Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        //[Browsable(true)]
        //[Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //セレクター
        //[Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        //public InArgument<string> Selector { get; set; }


        //[Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        //[Browsable(true)]
        //[Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        //[Localize.LocalizedDescription("Description3")] //输入UIElement //Enter UIElement //UIElementを入力
        //public InArgument<UiElement> Element { get; set; }


        InArgument<string> _JSCode;
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName19")] //JavaScript代码 //JavaScript code //JavaScriptコード
        [Localize.LocalizedDescription("Description21")] //要运行的JavaScript代码。您可以在此处将其作为字符串写入，或添加包含要执行的代码的.js文件的完整路径 //The JavaScript code to run.  You can write it here as a string or add the full path to the .js file containing the code to execute. //実行するJavaScriptコード。 ここに文字列として書き込むか、実行するコードを含む.jsファイルへのフルパスを追加できます。
        [RequiredArgument]
        [Browsable(true)]
        public InArgument<string> JSCode
        {
            get
            {
                return _JSCode;
            }
            set
            {
                _JSCode = value;
            }
        }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName20")] //JavaScript代码数据 //JavaScript code data //JavaScriptコードデータ
        [Localize.LocalizedDescription("Description22")] //输入JavaScript代码的数据，作为字符串或字符串变量 //Enter the data of the JavaScript code as a string or string variable //JavaScriptコードのデータを文字列または文字列変数として入力します
        [Browsable(true)]
        public List<InArgument> Parameters
        {
            get;
            set;
        } = new List<InArgument>();


        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName21")] //JavaScript输出 //JavaScript output //JavaScript出力
        [Localize.LocalizedDescription("Description23")] //从JavaScript代码返回的字符串结果 //String result returned from JavaScript code //JavaScriptコードから返された文字列結果
        [Browsable(true)]
        public OutArgument<object> JSOut
        {
            get; set;
        }


        [Browsable(false)]
        public string SourceImgPath { get; set; }

        private System.Windows.Visibility visi = System.Windows.Visibility.Hidden;
        [Browsable(false)]
        public System.Windows.Visibility visibility
        {
            get
            {
                return visi;
            }
            set
            {
                visi = value;
            }
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/InsertJS.png";
            }
        }

        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
        }

        static object InvokeScript(object callee, string method, params object[] args)
        {
            return callee.GetType().InvokeMember(method, BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, callee, args);
        }

        public static T GetValueOrDefault<T>(ActivityContext context, InArgument<T> source, T defaultValue)
        {
            T result = defaultValue;
            if (source != null && source.Expression != null)
            {
                result = source.Get(context);
            }
            return result;
        }

        //创建并验证活动的参数、变量、子活动和活动委托的说明
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            int num = 0;
            foreach (InArgument para in Parameters)
            {
                RuntimeArgument argument = new RuntimeArgument("attachmentArg" + ++num, para.ArgumentType, ArgumentDirection.In);
                metadata.Bind(para, argument);
                metadata.AddArgument(argument);
            }
        }

        //错误标志
        bool isErrorFlag = false;

        static void CallWithTimeout(Action action, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action();
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Int32 delayAfter = GetValueOrDefault(context, this.DelayAfter, 300);
            Int32 delayBefore = GetValueOrDefault(context, this.DelayBefore, 300);
            Int32 timeoutMS = GetValueOrDefault(context, this.TimeoutMS, 10000);
            object[] args;
            Thread.Sleep(delayBefore);

            if (Parameters.Count == 0)
            {
                args = new object[0];
            }
            else
            {
                args = new object[Parameters.Count];
                int i = 0;
                foreach (InArgument para in Parameters)
                {
                    args[i++] = context.GetValue(para);
                }
            }


            //var selStr = Selector.Get(context);
            string jsCode = JSCode.Get(context);
            if (File.Exists(jsCode))
            {
                string jsContent = File.ReadAllText(jsCode);
                jsCode = jsContent;
            }

            m_Delegate = new runDelegate(Run);
            //流程运行 只允许一个输入参数
            //chrome运行JS与IE不同，首先命名函数，而后执行。IE命名后可直接执行
            //返回值必须调用return func(),参数传递必须使用func(arguments[i])
            try
            {
                /******流程运行******/
                //根据OpenBrowser标志获取其抽象化属性
                PropertyDescriptor property = context.DataContext.GetProperties()[OpenBrowser.OpenBrowsersPropertyTag];
                if (property == null)
                    //根据AttachBrowser标志获取其抽象化属性
                    property = context.DataContext.GetProperties()[AttachBrowser.OpenBrowsersPropertyTag];
                if (property == null)
                {
                    return m_Delegate.BeginInvoke(callback, state);
                }
                //抽象化属性转换为可用Browser属性
                Browser getBrowser = property.GetValue(context.DataContext) as Browser;

                ChromeDriver chromeDriver = getBrowser.getICFBrowser() as ChromeDriver;
                InternetExplorerDriver ieDriver = getBrowser.getICFBrowser() as InternetExplorerDriver;
                FirefoxDriver fxDriver = getBrowser.getICFBrowser() as FirefoxDriver;

                //流程运行
                object returnVaue = null;
                Int32 circleTimes = timeoutMS / 1000;

                CallWithTimeout(new Action(() => {
                    if (isCircle)
                    {
                        for (int i = 0; i <= circleTimes; i++)
                        {
                            try
                            {
                                if (chromeDriver != null)
                                {
                                    returnVaue = chromeDriver.ExecuteScript(jsCode, args);
                                }
                                else if (fxDriver != null)
                                {
                                    returnVaue = fxDriver.ExecuteScript(jsCode, args);
                                }
                                else if (ieDriver != null)
                                {
                                    returnVaue = ieDriver.ExecuteScript(jsCode, args);
                                }
                                break;
                            }
                            catch
                            {
                                Thread.Sleep(500);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (chromeDriver != null)
                        {
                            returnVaue = chromeDriver.ExecuteScript(jsCode, args);
                        }
                        else if (fxDriver != null)
                        {
                            returnVaue = fxDriver.ExecuteScript(jsCode, args);
                        }
                        else if (ieDriver != null)
                        {
                            returnVaue = ieDriver.ExecuteScript(jsCode, args);

                        }
                    }
                }), timeoutMS);

                if (returnVaue != null && JSOut != null)
                    JSOut.Set(context, returnVaue);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "运行JS出错", e + "    JS: " + jsCode);
                if (ContinueOnError.Get(context))
                {
                    return m_Delegate.BeginInvoke(callback, state);
                }
                else
                {
                    throw new NotImplementedException("执行JS过程出错！");
                }
            }

            if (isErrorFlag==true && !ContinueOnError.Get(context))
            {
                throw new NotImplementedException("执行JS过程出错！");
            }
            else
            {
                Thread.Sleep(delayAfter);
                return m_Delegate.BeginInvoke(callback, state);
            }
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }


        //protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        //{
        //    Int32 delayAfter = GetValueOrDefault(context, this.DelayAfter, 300);
        //    Int32 delayBefore = GetValueOrDefault(context, this.DelayBefore, 300);
        //    Int32 timeoutMS = GetValueOrDefault(context, this.TimeoutMS, 10000);
        //    bool isRunSucceed = false;

        //    var selStr = Selector.Get(context);
        //    string jsCode = JSCode.Get(context);
        //    string jsPara = JSPara.Get(context);
        //    if (File.Exists(jsCode))
        //    {
        //        string jsContent = File.ReadAllText(jsCode);
        //        jsCode = jsContent;
        //    }

        //    Thread.Sleep(delayBefore);
        //    m_Delegate = new runDelegate(Run);
        //    //流程运行 只允许一个输入参数
        //    //chrome运行JS与IE不同，首先命名函数，而后执行。IE命名后可直接执行
        //    //返回值必须调用return func(),参数传递必须使用func(arguments[i])
        //    try
        //    {
        //        if (selStr == null || selStr == "")
        //        {
        //            PropertyDescriptor property = context.DataContext.GetProperties()[OpenBrowser.OpenBrowsersPropertyTag];
        //            if (property == null)
        //                property = context.DataContext.GetProperties()[AttachBrowser.OpenBrowsersPropertyTag];
        //            if (property == null)
        //            {
        //                m_Delegate = new runDelegate(Run);
        //                return m_Delegate.BeginInvoke(callback, state);
        //            }
        //            Browser getBrowser = property.GetValue(context.DataContext) as Browser;
        //            //流程运行Chrome和火狐浏览器
        //            if (getBrowser.getICFBrowser() != null)
        //            {
        //                ChromeDriver chromeDriver = getBrowser.getICFBrowser() as ChromeDriver;

        //                object returnVaue = null;
        //                //暂定一个参数
        //                //function sss(eee){document.getElementById('kw').value=eee; return 1223;};return sss(arguments[0]); 
        //                object[] args = new object[1];
        //                args[0] = jsPara;


        //                Int32 circleTimes = timeoutMS / 1000;

        //                CallWithTimeout(new Action(() => {
        //                    if (isCircle)
        //                    {
        //                        for (int i = 0; i <= circleTimes; i++)
        //                        {
        //                            try
        //                            {
        //                                if (jsPara != null && jsPara != "")
        //                                    returnVaue = chromeDriver.ExecuteScript(jsCode, jsPara);
        //                                else
        //                                    returnVaue = chromeDriver.ExecuteScript(jsCode);
        //                                isRunSucceed = true;
        //                            }
        //                            catch
        //                            {
        //                                Thread.Sleep(500);
        //                                continue;
        //                            }
        //                            if (isRunSucceed)
        //                                break;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (jsPara != null && jsPara != "")
        //                            returnVaue = chromeDriver.ExecuteScript(jsCode, jsPara);
        //                        else
        //                            returnVaue = chromeDriver.ExecuteScript(jsCode);
        //                    }

        //                }), timeoutMS);


        //                if (returnVaue != null && JSOut != null)
        //                    JSOut.Set(context, returnVaue);

        //                ReadOnlyCollection<string> chromeHandles = getBrowser.getICFBrowser().WindowHandles;
        //                string chromeHandle = chromeHandles[chromeHandles.Count - 1];
        //                chromeDriver.SwitchTo().Window(chromeHandle);
        //            }
        //            else if (getBrowser.getIEBrowser() != null)
        //            {
        //                SHDocVw.InternetExplorer ieBrowser = getBrowser.getIEBrowser() as SHDocVw.InternetExplorer;
        //                mshtml.IHTMLDocument2 currDoc = ieBrowser.Document as mshtml.IHTMLDocument2;
        //                mshtml.IHTMLElement currEle = null;

        //                OpenQA.Selenium.IE.InternetExplorerDriver sss = new OpenQA.Selenium.IE.InternetExplorerDriver();

        //                //latch = new CountdownEvent(1);
        //                //Thread td = new Thread(() =>
        //                //{
        //                //    runIEJS(ieBrowser, currDoc, currEle, jsCode, jsPara, context, 0);
        //                //    refreshData(latch);
        //                //});
        //                //td.TrySetApartmentState(ApartmentState.STA);
        //                //td.IsBackground = true;
        //                //td.Start();
        //                //latch.Wait();
        //            }
        //        }
        //        //桌面选取运行 允许三个参数
        //        else
        //        {
        //            latch = new CountdownEvent(1);
        //            Thread td = new Thread(() =>
        //            {

        //                UiElement element = GetValueOrDefault(context, this.Element, null);
        //                if (element == null && selStr != null)
        //                {
        //                    element = UiElement.FromSelector(selStr);
        //                }
        //                if (element != null)
        //                {
        //                    int windowHandle = (int)element.WindowHandle;
        //                    if ((int)element.WindowHandle == 0)
        //                    {
        //                        windowHandle = (int)element.Parent.WindowHandle;
        //                    }
        //                    mshtml.IHTMLDocument2 currDoc = null;
        //                    SHDocVw.InternetExplorer ieBrowser = GetIEFromHWndClass.GetIEFromHWnd(windowHandle, out currDoc);
        //                    mshtml.IHTMLElement currEle = null;
        //                    if (currDoc != null)
        //                        currEle = GetIEFromHWndClass.GetEleFromDoc(element.GetClickablePoint(), windowHandle, currDoc);
        //                    runIEJS(ieBrowser, currDoc, currEle, jsCode, jsPara, context, 1);
        //                }
        //                else
        //                {
        //                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "查找不到元素");
        //                    if (ContinueOnError.Get(context))
        //                    {
        //                        return;
        //                    }
        //                    else
        //                    {
        //                        throw new NotImplementedException("查找不到元素");
        //                    }
        //                }
        //                refreshData(latch);
        //            });
        //            td.TrySetApartmentState(ApartmentState.STA);
        //            td.IsBackground = true;
        //            td.Start();
        //            latch.Wait();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "子线程运行JS出错", e + "    JS: " + jsCode);
        //        if (ContinueOnError.Get(context))
        //        {
        //            return m_Delegate.BeginInvoke(callback, state);
        //        }
        //        else
        //        {
        //            throw new NotImplementedException("执行JS过程出错！");
        //        }
        //    }

        //    if (isErrorFlag == true && !ContinueOnError.Get(context))
        //    {
        //        throw new NotImplementedException("执行JS过程出错！");
        //    }
        //    else
        //    {
        //        Thread.Sleep(delayAfter);
        //        return m_Delegate.BeginInvoke(callback, state);
        //    }
        //}


        //private void runIEJS(SHDocVw.InternetExplorer ieBrowser,
        //    mshtml.IHTMLDocument2 currDoc,
        //    mshtml.IHTMLElement currEle,
        //    string jsCode,
        //    string jsPara,
        //    AsyncCodeActivityContext context,
        //    int jsFlag
        //)
        //{
        //    try
        //    {
        //        /*WEB页面中添加JS*/
        //        mshtml.IHTMLElement JSele = ieBrowser.Document.createElement("script");
        //        JSele.setAttribute("type", "text/javascript");
        //        JSele.setAttribute("text", jsCode);
        //        ieBrowser.Document.Body.AppendChild(JSele);

                //设置属性
                //currDoc.parentWindow.execScript("document.body.setAttribute('PSResult','PSResult')", "JavaScript");
                //移除属性
                //currDoc.parentWindow.execScript("document.body.removeAttribute('PSResult')", "JavaScript");

        //        //若为Function类型的JS函数体
        //        string FuncName = "";
        //        string ArgStr = "";
        //        bool flag = false;
        //        jsCode = jsCode.Trim();
        //        if (jsCode.Contains("function") || jsCode.Contains("Function"))
        //        {
        //            foreach (char myChar in jsCode)
        //            {
        //                if (myChar == ' ')
        //                {
        //                    flag = true;
        //                }
        //                if (flag)
        //                {
        //                    if (myChar == '(')
        //                        break;
        //                    FuncName += myChar;
        //                }
        //            }

        //            flag = false;
        //            foreach (char myChar in jsCode)
        //            {
        //                if (myChar == '(')
        //                {
        //                    flag = true;
        //                    continue;
        //                }
        //                if (flag)
        //                {
        //                    if (myChar == ')')
        //                        break;
        //                    ArgStr += myChar;
        //                }
        //            }
        //        }
        //        //execScript方式无法获取返回值
        //        //object c = currDoc.parentWindow.execScript("function aaa(){return \"aaa\"};aaa();");
        //        FuncName = FuncName.Trim();
        //        string[] ArgArray = ArgStr.Split(',');
        //        object htmlWindowObject = currDoc.parentWindow;

        //        object returnValue = null;
        //        if (ArgArray.Length > 2 && jsFlag == 1)
        //        {
        //            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "JS函数参数个数超出范围");
        //            isErrorFlag = true;
        //        }
        //        else if (ArgArray.Length > 1 && jsFlag == 0)
        //        {
        //            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "JS函数参数个数超出范围");
        //            isErrorFlag = true;
        //        }
        //        else
        //        {
        //            //两个参数 第一个为element 第二个为输入参数值
        //            if (ArgArray.Length == 2 && jsFlag == 1)
        //                returnValue = InvokeScript(htmlWindowObject, FuncName, new object[] { currEle, jsPara });
        //            //一个参数 默认为element
        //            if (ArgArray.Length == 1 && jsFlag == 1)
        //                returnValue = InvokeScript(htmlWindowObject, FuncName, new object[] { currEle });
        //            //无参
        //            if (ArgArray.Length == 0 && jsFlag == 1)
        //                returnValue = InvokeScript(htmlWindowObject, FuncName, new object[] { });
        //            //流程化IE 无参
        //            if (ArgArray.Length == 0 && jsFlag == 0)
        //                returnValue = InvokeScript(htmlWindowObject, FuncName, new object[] { });
        //            //流程化IE 一个参数
        //            if (ArgArray.Length == 1 && jsFlag == 0)
        //                returnValue = InvokeScript(htmlWindowObject, FuncName, new object[] { jsPara });
        //        }
        //        if (returnValue != null)
        //        {
        //            JSOut.Set(context, returnValue);
        //        }

        //        ArrayList list = new ArrayList();
        //        var allShellWindows = new SHDocVw.ShellWindows();
        //        foreach (SHDocVw.InternetExplorer browser in allShellWindows)
        //        {
        //            list.Add(browser);
        //        }
        //        CommonVariable.BrowsersList = list;

        //        //确保页面是否已加载完成
        //        while (ieBrowser.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
        //        {
        //            Thread.Sleep(500);
        //        }
        //        /*使用此种方式获取返回值会返回Microsoft.CSharp.RuntimeBinder.RuntimeBinderException*/
        //        //int a = ieBrowser.Document.Script.value;
        //        //int a = currDoc.Script.value;
        //    }
        //    catch (Exception e)
        //    {
        //        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "JS子线程运行失败");
        //        isErrorFlag = true;
        //    }
        //}


        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }
        [Browsable(false)]
        public string ClassName { get { return "InsertJS"; } }
    }
}
