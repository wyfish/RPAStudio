using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(SaveImageDesigner))]
    public sealed class SaveImageActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Save Image"; } }

        [Localize.LocalizedCategory("Category1")]//公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")]//错误执行 //Error Execution //エラー実行
        [Localize.LocalizedDescription("Description1")]//指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDescription("Description91")]//保存图像的完整文件路径及其名称。//Saves the full file path and name of the image. //画像の完全なファイルパスと名前を保存します。
        public InArgument<string> FileName { get; set; }

        [RequiredArgument]
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName68")]//Image//Image//Image
        [Browsable(true)]
        [Localize.LocalizedDescription("Description92")]//要保存的图像。仅支持Image变量。//Image to save. Only image variables are supported.//保存する画像。Image変数のみをサポートします。
        public InArgument<System.Drawing.Image> Image { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/SaveImage.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string fileName = FileName.Get(context);
                System.Drawing.Image image = Image.Get(context);
                image.Save(fileName);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "保存图片失败", e.Message);
                if (ContinueOnError.Get(context))
                {

                }
                else
                {
                    throw e;
                }
            }
        }
    }
}
