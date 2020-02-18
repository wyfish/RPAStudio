using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Identifiers;
using FlaUI.Core;

namespace RPA.UIAutomation.Activities.Attribute
{
    [Designer(typeof(GetAttrDesigner))]
    public sealed class GetAttr : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Get Attribute"; } }

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //ウィンドウインジケータ
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description3")] //输入UIElement //Enter UIElement //UIElementを入力
        public InArgument<UiElement> Element { get; set; }

        [Browsable(false)]
        public IEnumerable<AttributeEnums> AttrEnums
        {
            get
            {
                return Enum.GetValues(typeof(AttributeEnums)).Cast<AttributeEnums>();
            }
        }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName4")] //属性 //Attributes //属性
        [Localize.LocalizedDescription("Description4")] // 要检索的属性的名称。该字段仅支持字符串 //The name of the property to retrieve.  This field only supports strings //取得するプロパティの名前。 このフィールドは文字列のみをサポートします
        public InArgument<string> AttrName { get; set; }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName5")] //结果 //result //結果
        [Localize.LocalizedDescription("Description5")] //指定属性的值 //Specify the value of the attribute //属性の値を指定します
        public OutArgument<object> Result { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Attribute/GetAttr.png";
            }
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

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var selStr = Selector.Get(context);
                object attrValue = null;
                string attrName = AttrName.Get(context);

                UiElement element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                AutomationElement autoEle = element.NativeObject as AutomationElement;
                FrameworkAutomationElementBase baseFrame = autoEle.FrameworkAutomationElement;
                PropertyId[] ids = autoEle.GetSupportedPropertiesDirect();
                for (int i=0; i< ids.Length; i++)
                {
                    if(String.Equals(ids[i].Name, attrName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        attrValue = baseFrame.GetPropertyValue(ids[i]);
                        break;
                    }
                }
                if(attrValue == null)
                {
                    Result.Set(context, "");
                }
                else
                {
                    Result.Set(context, attrValue);
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取元素属性失败", e.Message);
                if (ContinueOnError.Get(context))
                {
                    return;
                }
                else
                {
                    throw e;
                }
            }
        }
    }
}
