using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using log4net;
using RPARobot.Librarys;

namespace RPAStudio.Services
{
    /// <summary>
    /// 发布项目时的服务封装类
    /// </summary>
    public class PublishService
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PublishService()
        {

        }

        /// <summary>
        /// 生成新GUID
        /// </summary>
        /// <returns>新的GUID字符串</returns>
        private string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 发布项目网络接口
        /// </summary>
        /// <param name="builder">包生成器</param>
        /// <param name="nupkgLocation">包位置</param>
        /// <param name="projectName">项目名称</param>
        /// <param name="publishVersion">发布版本</param>
        /// <param name="description">包描述</param>
        /// <returns></returns>
        public async Task<bool> Publish(PackageBuilder builder, string nupkgLocation, string projectName, string publishVersion,string description)
        {
            try
            {
                //增加一个流程
                string add_process_info = nupkgLocation + "/OSPServer/rest/dict/savedict";
                string upload_affix = nupkgLocation + "/OSPServer/rest/affix/dict/save";

                var guid = NewGuid();
                JObject jObj = new JObject();
                jObj["DCT_ID"] = "PROCESS_INFO";
                var jArr = new JArray();

                JObject jItem = new JObject();
                jItem["F_GUID"] = guid;
                jItem["PROCESSNAME"] = projectName;
                jItem["PROCESSVERSION"] = publishVersion;
                jItem["PROCESSDESCRIBE"] = description;
                jArr.Add(jItem);

                jObj["InsertPool"] = jArr;

                var result = await add_process_info.PostJsonAsync(jObj).ReceiveJson<JObject>();
                if((int)result["code"] != 1)
                {
                    Logger.Debug("增加流程信息失败！" + jObj.ToString(), logger);
                    return false;
                }

                //上传该流程对应的附件
                var fileName = projectName + "." + publishVersion + ".nupkg";
                var outputPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);
                if (System.IO.File.Exists(outputPath))
                {
                    System.IO.File.Delete(outputPath);
                }

                using (FileStream stream = File.Open(outputPath, FileMode.OpenOrCreate))
                {
                    builder.Save(stream);
                }

                jObj = new JObject();
                jObj["F_GUID"] = guid;
                jObj["F_CCLX"] = "FILE";
                jObj["MDLID"] = "PROCESS_ALLOCATION";
                jObj["YWLX"] = "";
                jObj["F_NAME"] = fileName;
                jObj["F_PATH"] = "";
                jObj["EXT_STR01"] = "";
                jObj["EXT_STR02"] = "";
                jObj["EXT_STR03"] = "";
                jObj["EXT_STR04"] = "";

                var resp = await upload_affix
                .PostMultipartAsync(mp => mp
                .AddFile("FILE", outputPath)
                .AddJson("AFFIXMSG", jObj));

                System.IO.File.Delete(outputPath);

                if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Logger.Debug("上传文件出错！", logger);
                    return false;
                }

                return true;
            }
            catch (Exception err)
            {
                //TODO WJF 失败的话可能需要删除PROCESS_INFO和PROCESS_ALLOCATION的guid对应的条目
                Logger.Debug(err, logger);
                return false;
            }
        }
    }
}
