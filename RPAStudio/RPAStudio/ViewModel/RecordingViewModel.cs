using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Plugins.Shared.Library.UiAutomation;
using System.ComponentModel;
using System.Windows;
using System;
using System.Activities.Presentation.Services;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using RPAStudio.Librarys;
using System.Collections.Generic;


namespace RPAStudio.ViewModel
{
    /// <summary>
    /// 录制窗体视图模型
    /// </summary>
    public class RecordingViewModel : ViewModelBase
    {
        /// <summary>
        /// 对应的视图
        /// </summary>
        private Window m_view;

        /// <summary>
        /// 活动信息结构体
        /// </summary>
        struct stuActivityInfo
        {
            public Activity activity;
            public Action preAction;
            public Action<ModelItem> postAction;
        }

        /// <summary>
        /// 录制的活动信息列表，按序排放
        /// </summary>
        private List<stuActivityInfo> m_activityRecordingList = new List<stuActivityInfo>();

        /// <summary>
        /// Initializes a new instance of the RecordingViewModel class.
        /// </summary>
        public RecordingViewModel()
        {
        }



        private RelayCommand<RoutedEventArgs> _loadedCommand;

        /// <summary>
        /// 窗体加载完成
        /// </summary>
        public RelayCommand<RoutedEventArgs> LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        m_view = (Window)p.Source;

                        UiElement.IsRecordingWindowOpened = true;
                    }));
            }
        }


        private RelayCommand<CancelEventArgs> _closingCommand;

        /// <summary>
        /// 窗体即将关闭时调用
        /// </summary>
        public RelayCommand<CancelEventArgs> ClosingCommand
        {
            get
            {
                return _closingCommand
                    ?? (_closingCommand = new RelayCommand<CancelEventArgs>(
                    p =>
                    {
                        bool bContinueClose = true;

                        if(IsRecorded)
                        {
                            //关闭主窗口前确认
                            m_view.Topmost = false;
                            var ret = MessageBox.Show(App.Current.MainWindow, "录制结果未保存，确定退出吗？", "询问", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                            if (ret == MessageBoxResult.No)
                            {
                                bContinueClose = false;
                                m_view.Topmost = true;
                            }
                        }

                        if (!bContinueClose)
                        {
                            p.Cancel = true;
                        }
                    }));
            }
        }

        private RelayCommand _unloadedCommand;

        /// <summary>
        /// 窗体卸载时调用
        /// </summary>
        public RelayCommand UnloadedCommand
        {
            get
            {
                return _unloadedCommand
                    ?? (_unloadedCommand = new RelayCommand(
                    () =>
                    {
                        UiElement.IsRecordingWindowOpened = false;
                    }));
            }
        }



        private RelayCommand _saveAndExitCommand;

        /// <summary>
        /// 保存并退出
        /// </summary>
        public RelayCommand SaveAndExitCommand
        {
            get
            {
                return _saveAndExitCommand
                    ?? (_saveAndExitCommand = new RelayCommand(
                    () =>
                    {
                        //判断是否有正在活动的设计器
                        if(ViewModelLocator.Instance.Dock.ActiveDocument != null && !ViewModelLocator.Instance.Dock.ActiveDocument.IsReadOnly)
                        {
                            var wd = ViewModelLocator.Instance.Dock.ActiveDocument.WorkflowDesignerInstance;
                            ModelService modelService = wd.Context.Services.GetService<ModelService>();
                            ModelItem rootModelItem = modelService.Root.Properties["Implementation"].Value;
                            if(rootModelItem == null)
                            {
                                modelService.Root.Content.SetValue(new Sequence());
                                rootModelItem = modelService.Root.Properties["Implementation"].Value;
                            }

                            foreach(var item in m_activityRecordingList)
                            {
                                item.preAction?.Invoke();
                                var retModelItem = rootModelItem.AddActivity(item.activity);
                                if(retModelItem!=null)
                                {
                                    //往流程图上放组件时要特殊处理下
                                    if (retModelItem.IsFlowStep())
                                    {
                                        retModelItem = retModelItem.Properties["Action"].Value;
                                    }

                                    item.postAction?.Invoke(retModelItem);
                                }
                                
                            }
                        }

                        IsRecorded = false;
                        //关闭当前窗口
                        m_view.Close();
                    }));
            }
        }


        /// <summary>
        /// The <see cref="IsRecorded" /> property's name.
        /// </summary>
        public const string IsRecordedPropertyName = "IsRecorded";

        private bool _isRecordedProperty = false;

        /// <summary>
        /// 是否有录制的活动 
        /// </summary>
        public bool IsRecorded
        {
            get
            {
                return _isRecordedProperty;
            }

            set
            {
                if (_isRecordedProperty == value)
                {
                    return;
                }

                _isRecordedProperty = value;
                RaisePropertyChanged(IsRecordedPropertyName);
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 进行实际的鼠标选择
        /// </summary>
        /// <param name="uiElement">ui元素</param>
        /// <param name="type">类型</param>
        /// <param name="action">活动</param>
        private void DoMouseSelect(UiElement uiElement,string type,Action<object> action = null)
        {
            IsRecorded = true;
            m_view.WindowState = WindowState.Normal;
            m_view.Topmost = true;

            Type _type = Type.GetType($"RPA.UIAutomation.Activities.Mouse.{type},RPA.UIAutomation.Activities");
            dynamic activity = Activator.CreateInstance(_type);
            action?.Invoke(activity);
            activity.SourceImgPath = uiElement.CaptureInformativeScreenshotToFile();
            activity.Selector = uiElement.Selector;
            activity.visibility = System.Windows.Visibility.Visible;
            activity.offsetX = uiElement.GetClickablePoint().X;
            activity.offsetY = uiElement.GetClickablePoint().Y;

            activity.Left = uiElement.BoundingRectangle.Left;
            activity.Right = uiElement.BoundingRectangle.Right;
            activity.Top = uiElement.BoundingRectangle.Top;
            activity.Bottom = uiElement.BoundingRectangle.Bottom;

            var append_displayName = " \"" + uiElement.ProcessName + " " + uiElement.Name + "\"";

            stuActivityInfo info = new stuActivityInfo();
            info.activity = activity;
            info.postAction = (modelItem) =>
            {
                modelItem.Properties["DisplayName"].SetValue(modelItem.Properties["DisplayName"].Value+ append_displayName);
            };
            m_activityRecordingList.Add(info);
        }

        private RelayCommand _mouseLeftClickCommand;

        /// <summary>
        /// 鼠标左键单击
        /// </summary>
        public RelayCommand MouseLeftClickCommand
        {
            get
            {
                return _mouseLeftClickCommand
                    ?? (_mouseLeftClickCommand = new RelayCommand(
                    () =>
                    {
                        m_view.WindowState = WindowState.Minimized;
                        
                        UiElement.OnSelected = UiElement_OnMouseLeftClickSelected;
                        UiElement.StartElementHighlight();
                    }));
            }
        }

        /// <summary>
        /// 鼠标左键单击选中后的事件
        /// </summary>
        /// <param name="uiElement">ui元素</param>
        private void UiElement_OnMouseLeftClickSelected(UiElement uiElement)
        {
            //单击要穿透到下面（模拟点击）
            uiElement.MouseClick(null);

            DoMouseSelect(uiElement, "ClickActivity",(activity) =>
            {
                dynamic _activity = activity;
                _activity.SetMouseLeftClick();
            });
        }

        private RelayCommand _mouseRightClickCommand;

        /// <summary>
        /// 鼠标右键单击
        /// </summary>
        public RelayCommand MouseRightClickCommand
        {
            get
            {
                return _mouseRightClickCommand
                    ?? (_mouseRightClickCommand = new RelayCommand(
                    () =>
                    {
                        m_view.WindowState = WindowState.Minimized;

                        UiElement.OnSelected = UiElement_OnMouseRightClickSelected;
                        UiElement.StartElementHighlight();
                    }));
            }
        }

        /// <summary>
        /// 鼠标右击选中元素后的事件
        /// </summary>
        /// <param name="uiElement">ui元素</param>
        private void UiElement_OnMouseRightClickSelected(UiElement uiElement)
        {
            //单击要穿透到下面（模拟点击）
            uiElement.MouseRightClick(null);

            DoMouseSelect(uiElement, "ClickActivity",(activity) =>
            {
                dynamic _activity = activity;
                _activity.SetMouseRightClick();
            });
        }

        private RelayCommand _mouseDoubleLeftClickCommand;

        /// <summary>
        /// 鼠标左键双击
        /// </summary>
        public RelayCommand MouseDoubleLeftClickCommand
        {
            get
            {
                return _mouseDoubleLeftClickCommand
                    ?? (_mouseDoubleLeftClickCommand = new RelayCommand(
                    () =>
                    {
                        m_view.WindowState = WindowState.Minimized;

                        UiElement.OnSelected = UiElement_OnMouseDoubleLeftClickSelected;
                        UiElement.StartElementHighlight();
                    }));
            }
        }

        /// <summary>
        /// 鼠标左键双击选中元素后的事件
        /// </summary>
        /// <param name="uiElement">选中的ui元素</param>
        private void UiElement_OnMouseDoubleLeftClickSelected(UiElement uiElement)
        {
            //单击要穿透到下面（模拟点击）
            uiElement.MouseDoubleClick(null);

            DoMouseSelect(uiElement, "DoubleClickActivity", (activity) =>
            {
                dynamic _activity = activity;
                _activity.SetMouseDoubleLeftClick();
            });
        }

        private RelayCommand _mouseHoverCommand;

        /// <summary>
        /// 鼠标悬浮
        /// </summary>
        public RelayCommand MouseHoverCommand
        {
            get
            {
                return _mouseHoverCommand
                    ?? (_mouseHoverCommand = new RelayCommand(
                    () =>
                    {
                        m_view.WindowState = WindowState.Minimized;

                        UiElement.OnSelected = UiElement_OnMouseHoverSelected;
                        UiElement.StartElementHighlight();
                    }));
            }
        }

        /// <summary>
        /// 鼠标悬浮后选中元素的事件
        /// </summary>
        /// <param name="uiElement">ui元素</param>
        private void UiElement_OnMouseHoverSelected(UiElement uiElement)
        {
            uiElement.MouseHover(null);

            DoMouseSelect(uiElement,"HoverClickActivity");
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private RelayCommand _keyboardInputCommand;

        /// <summary>
        /// 键盘输入
        /// </summary>
        public RelayCommand KeyboardInputCommand
        {
            get
            {
                return _keyboardInputCommand
                    ?? (_keyboardInputCommand = new RelayCommand(
                    () =>
                    {
                        m_view.WindowState = WindowState.Minimized;

                        UiElement.OnSelected = UiElement_OnKeyboardInputSelected;
                        UiElement.StartElementHighlight();
                    }));
            }
        }

        /// <summary>
        /// 键盘输入选中元素后触发
        /// </summary>
        /// <param name="uiElement">ui元素</param>
        private void UiElement_OnKeyboardInputSelected(UiElement uiElement)
        {
            IsRecorded = true;
            m_view.WindowState = WindowState.Normal;
            m_view.Topmost = true;

            Type _type = Type.GetType("RPA.UIAutomation.Activities.Keyboard.TypeIntoActivity,RPA.UIAutomation.Activities");
            dynamic activity = Activator.CreateInstance(_type);
            activity.SourceImgPath = uiElement.CaptureInformativeScreenshotToFile();
            activity.Selector = uiElement.Selector;
            activity.visibility = System.Windows.Visibility.Visible;
            activity.offsetX = uiElement.GetClickablePoint().X;
            activity.offsetY = uiElement.GetClickablePoint().Y;

            var append_displayName = " \"" + uiElement.ProcessName + " " + uiElement.Name + "\"";

            stuActivityInfo info = new stuActivityInfo();
            info.activity = activity;
            info.postAction = (modelItem) =>
            {
                modelItem.Properties["DisplayName"].SetValue(modelItem.Properties["DisplayName"].Value+ append_displayName);
            };
            m_activityRecordingList.Add(info);
        }

        private RelayCommand _keyboardHotKeyCommand;

        /// <summary>
        /// 热键
        /// </summary>
        public RelayCommand KeyboardHotKeyCommand
        {
            get
            {
                return _keyboardHotKeyCommand
                    ?? (_keyboardHotKeyCommand = new RelayCommand(
                    () =>
                    {
                        m_view.WindowState = WindowState.Minimized;

                        UiElement.OnSelected = UiElement_OnKeyboardHotKeySelected;
                        UiElement.StartElementHighlight();
                    }));
            }
        }

        /// <summary>
        /// 热键选中元素后触发
        /// </summary>
        /// <param name="uiElement">元素</param>
        private void UiElement_OnKeyboardHotKeySelected(UiElement uiElement)
        {
            IsRecorded = true;
            m_view.WindowState = WindowState.Normal;
            m_view.Topmost = true;

            Type _type = Type.GetType("RPA.UIAutomation.Activities.Keyboard.HotKeyActivity,RPA.UIAutomation.Activities");
            dynamic activity = Activator.CreateInstance(_type);
            activity.SourceImgPath = uiElement.CaptureInformativeScreenshotToFile();
            activity.Selector = uiElement.Selector;
            activity.visibility = System.Windows.Visibility.Visible;
            activity.offsetX = uiElement.GetClickablePoint().X;
            activity.offsetY = uiElement.GetClickablePoint().Y;

            var append_displayName = " \"" + uiElement.ProcessName + " " + uiElement.Name + "\"";

            stuActivityInfo info = new stuActivityInfo();
            info.activity = activity;
            info.postAction = (modelItem) =>
            {
                modelItem.Properties["DisplayName"].SetValue(modelItem.Properties["DisplayName"].Value + append_displayName);
            };
            m_activityRecordingList.Add(info);
        }
    }
}