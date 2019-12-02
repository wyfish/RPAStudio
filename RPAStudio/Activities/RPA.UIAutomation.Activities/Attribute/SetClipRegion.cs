using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Windows;

namespace RPA.UIAutomation.Activities.Attribute
{
    [Designer(typeof(SetClipRegionDesigner))]
    public sealed class SetClipRegion : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Set Clipping Region";
            }
        }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("选项")]
        [Browsable(true)]
        [DisplayName("区域剪切方向")]
        [Description("扩展剪切区域的方向")]
        public DirectionEnums Direction { get; set; }

        [Category("UI对象")]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("使用另一个活动返回的UiElement变量。此属性不能与Selector属性一起使用。该字段仅支持UiElement变量")]
        public InArgument<UiElement> Element { get; set; }


        [Category("UI元素剪切矩阵")]
        [Browsable(true)]
        public InArgument<Int32> Left { get; set; }
        [Category("UI元素剪切矩阵")]
        [Browsable(true)]
        public InArgument<Int32> Right { get; set; }
        [Category("UI元素剪切矩阵")]
        [Browsable(true)]
        public InArgument<Int32> Top { get; set; }
        [Category("UI元素剪切矩阵")]
        [Browsable(true)]
        public InArgument<Int32> Bottom { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Attribute/SetClipRegion.png";
            }
        }
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override void Execute(CodeActivityContext context)
        {
            UiElement uiElement = this.Element.Get(context);
            int left = Left.Get(context);
            int right = Right.Get(context);
            int top = Top.Get(context);
            int bottom = Bottom.Get(context);                

            Rectangle rect1 = uiElement.BoundingRectangle;
            Rectangle rect2 = default(Rectangle);
            switch (this.Direction)
            {
                case DirectionEnums.左:
                    if (left != 0)
                    {
                        rect2 = Rectangle.FromLTRB(rect1.Left - left, rect1.Top, rect1.Right, rect1.Bottom);
                    }
                    break;
                case DirectionEnums.上:
                    if (top != 0)
                    {
                        rect2 = Rectangle.FromLTRB(rect1.Left, rect1.Top - top, rect1.Right, rect1.Bottom);
                    }
                    break;
                case DirectionEnums.右:
                    if (right != 0)
                    {
                        rect2 = Rectangle.FromLTRB(rect1.Left, rect1.Top, rect1.Right + right, rect1.Bottom);
                    }
                    break;
                case DirectionEnums.下:
                    if (bottom != 0)
                    {
                        rect2 = Rectangle.FromLTRB(rect1.Left, rect1.Top, rect1.Right, rect1.Bottom + bottom);
                    }
                    break;
                case DirectionEnums.矩形:
                    rect2 = Rectangle.FromLTRB(rect1.Left - left, rect1.Top - top, rect1.Right + right, rect1.Bottom + bottom);
                    break;
            }
            //if (!rect2.IsEmpty)
            //{
            //    uiElement.ClippingRegion = new Region
            //    {
            //        Rectangle = new Rectangle?(rect2)
            //    };
            //    return;
            //}
            uiElement.BoundingRectangle = rect2;
            return;
        }
    }
}
