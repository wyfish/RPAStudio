using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPAStudio.ViewModel;
using System.Activities.Presentation;
using System.Windows;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Activities.Validation;
using GalaSoft.MvvmLight.Messaging;
using RPAStudio.Librarys;
using RPAStudio.Localization;
using Plugins.Shared.Library.Extensions;
using Plugins.Shared.Library;

namespace RPAStudio.Executor
{
    public class RunManager
    {
        private WorkflowApplication m_app;
        public WorkflowDesigner m_workflowDesigner { get; set; }
        private string m_xamlPath { get; set; }

        public Window m_mainView;

        public MainViewModel m_mainViewModel;

        public RunManager(MainViewModel mainViewModel, DocumentViewModel documentViewModel)
        {
            m_mainViewModel = mainViewModel;
            m_mainView = mainViewModel.m_view;
            m_workflowDesigner = documentViewModel.WorkflowDesignerInstance;
            m_xamlPath = documentViewModel.XamlPath;
        }



        /// <summary>
        /// 开始执行运行流程
        /// </summary>
        public void Run()
        {
            //授权检测
            ViewModelLocator.Instance.SplashScreen.DoAuthorizationCheck();

            Activity workflow = ActivityXamlServices.Load(m_xamlPath);

            var result = ActivityValidationServices.Validate(workflow);
            if (result.Errors.Count == 0)
            {
                Messenger.Default.Send(this, "BeginRun");

                if(m_app != null)
                {
                    m_app.Terminate("");
                }

                m_app = new WorkflowApplication(workflow);
                m_app.Extensions.Add(new LogToOutputWindowTextWriter());

                if (workflow is DynamicActivity)
                {
                    var wr = new WorkflowRuntime();
                    wr.RootActivity = workflow;
                    m_app.Extensions.Add(wr);
                }

                m_app.OnUnhandledException = WorkflowApplicationOnUnhandledException;
                m_app.Completed = WorkflowApplicationExecutionCompleted;
                m_app.Run();
            }
            else
            {
                MessageBox.Show(App.Current.MainWindow, ResxIF.GetString("WorkflowCheckError"), ResxIF.GetString("ErrorText"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private UnhandledExceptionAction WorkflowApplicationOnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            var name = e.ExceptionSource.DisplayName;
            SharedObject.Instance.Output(SharedObject.enOutputType.Error, string.Format("{0} 执行时出现异常", name), e.UnhandledException.ToString());

            return UnhandledExceptionAction.Terminate;
        }

        private void WorkflowApplicationExecutionCompleted(WorkflowApplicationCompletedEventArgs obj)
        {
            if(obj.TerminationException != null)
            {
                if(!string.IsNullOrEmpty(obj.TerminationException.Message))
                {
                    Common.RunInUI(()=> {
                        // 运行时执行错误
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, ResxIF.GetString("RuntimeError"), obj.TerminationException.ToString());
                        MessageBox.Show(App.Current.MainWindow, obj.TerminationException.Message, ResxIF.GetString("RuntimeError"), MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            }
            Messenger.Default.Send(this, "EndRun");
        }

        public void Stop()
        {
            if (m_app != null)
            {
                try
                {
                    m_app.Terminate("执行已取消", new TimeSpan(0, 0, 0, 30));
                }
                catch (Exception )
                {
                    MessageBox.Show(App.Current.MainWindow, "停止调试发生异常！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
