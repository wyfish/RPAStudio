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

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName7")] //区域剪切方向 //Area shear direction //面せん断方向
        [Localize.LocalizedDescription("Description7")] //扩展剪切区域的方向 //Extend the direction of the clipping region //クリッピング領域の方向を拡張します
        public DirectionEnums Direction { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description8")] //使用另一个活动返回的UiElement变量。此属性不能与Selector属性一起使用。该字段仅支持UiElement变量 //Use the UiElement variable returned by another activity.  This property cannot be used with the Selector property.  This field only supports UiElement variables //別のアクティビティによって返されたUiElement変数を使用します。 このプロパティをSelectorプロパティと一緒に使用することはできません。 このフィールドはUiElement変数のみをサポートします
        public InArgument<UiElement> Element { get; set; }


        [Localize.LocalizedCategory("Category6")] //UI元素剪切矩阵 //UI element clipping matrix //UI要素のクリッピング行列
        [Browsable(true)]
        public InArgument<Int32> Left { get; set; }
        [Localize.LocalizedCategory("Category6")] //UI元素剪切矩阵 //UI element clipping matrix //UI要素のクリッピング行列
        [Browsable(true)]
        public InArgument<Int32> Right { get; set; }
        [Localize.LocalizedCategory("Category6")] //UI元素剪切矩阵 //UI element clipping matrix //UI要素のクリッピング行列
        [Browsable(true)]
        public InArgument<Int32> Top { get; set; }
        [Localize.LocalizedCategory("Category6")] //UI元素剪切矩阵 //UI element clipping matrix //UI要素のクリッピング行列
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
