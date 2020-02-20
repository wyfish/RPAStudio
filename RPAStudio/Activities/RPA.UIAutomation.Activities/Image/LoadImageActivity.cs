using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using Plugins.Shared.Library;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(LoadImageDesigner))]
    public sealed class LoadImageActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Load Image"; } }

        [Localize.LocalizedCategory("Category1")]//公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")]//错误执行 //Error Execution //エラー実行
        [Localize.LocalizedDescription("Description1")]//指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDescription("Description88")]//要载入的图像的完整路径。//The full path of the image to load. //読み込む画像のフルパス。
        public InArgument<string> FileName { get; set; }

        [RequiredArgument]
        [Localize.LocalizedCategory("Category4")]//输出//Output//出力
        [Localize.LocalizedDisplayName("DisplayName68")]//Image//Image//Image
        [Browsable(true)]
        [Localize.LocalizedDescription("Description89")]//要载入的图像。仅支持Image变量。//The image to load. Only image variables are supported.//読み込む画像。Image変数のみをサポートします。
        public OutArgument<System.Drawing.Image> Image { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/LoadImage.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string fileName = this.FileName.Get(context);
                Bitmap bit = new Bitmap(fileName);
                System.Drawing.Image img = bit as System.Drawing.Image;
                Image.Set(context, img);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "载入图片失败", e.Message);
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
