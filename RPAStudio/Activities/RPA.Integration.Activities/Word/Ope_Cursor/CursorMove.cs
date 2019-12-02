using System;
using System.Activities;
using System.ComponentModel;
using Microsoft.Office.Interop.Word;
using Plugins.Shared.Library;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(CursorMoveDesigner))]
    public sealed class CursorMove : AsyncCodeActivity
    {
        InArgument<Int32> _MovePos = 1;
        [Category("选项")]
        [DisplayName("游标移动距离")]
        [Browsable(true)]
        public InArgument<Int32> MovePos
        {
            get
            {
                return _MovePos;
            }
            set
            {
                _MovePos = value;
            }
        }

        [Category("选项")]
        [DisplayName("是否选中")]
        [Browsable(true)]
        public bool IsSelect
        {
            get;set;
        }

        [Category("方向")]
        [DisplayName("左")]
        [Browsable(true)]
        public bool Left
        {
            get;set;
        }
        [Category("方向")]
        [DisplayName("右")]
        [Browsable(true)]
        public bool Right
        {
            get; set;
        }
        [Category("方向")]
        [DisplayName("上")]
        [Browsable(true)]
        public bool Up
        {
            get; set;
        }
        [Category("方向")]
        [DisplayName("下")]
        [Browsable(true)]
        public bool Down
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/move1.png"; } }


        [Browsable(false)]
        public string ClassName { get { return "CursorMove"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            PropertyDescriptor property = context.DataContext.GetProperties()[WordCreate.GetWordAppTag];
            Application wordApp = property.GetValue(context.DataContext) as Application;
            try
            {
                Int32 _movePos = MovePos.Get(context);
                Selection sel = wordApp.Selection;

                if (Left)
                {
                    if (IsSelect)
                        sel.MoveLeft(WdUnits.wdCharacter, _movePos, WdMovementType.wdExtend);
                    else
                        sel.MoveLeft(WdUnits.wdCharacter, _movePos, WdMovementType.wdMove);
                }
                else if (Right)
                {
                    if (IsSelect)
                        sel.MoveRight(WdUnits.wdCharacter, _movePos, WdMovementType.wdExtend);
                    else
                        sel.MoveRight(WdUnits.wdCharacter, _movePos, WdMovementType.wdMove);
                }
                else if (Up)
                {
                    if (IsSelect)
                        sel.MoveUp(WdUnits.wdLine, _movePos, WdMovementType.wdExtend);
                    else
                        sel.MoveUp(WdUnits.wdLine, _movePos, WdMovementType.wdMove);
                }
                else if (Down)
                {
                    if (IsSelect)
                        sel.MoveDown(WdUnits.wdLine, _movePos, WdMovementType.wdExtend);
                    else
                        sel.MoveDown(WdUnits.wdLine, _movePos, WdMovementType.wdMove);
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "Word执行过程出错", e.Message);
                CommonVariable.realaseProcessExit(wordApp);
            }

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
