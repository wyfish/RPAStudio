using AesEverywhere;
using Flurl.Http;
using log4net;
using Newtonsoft.Json.Linq;
using RPARobot.Librarys;
using RPARobot.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Services
{
    /// <summary>
    /// 控制中心服务，封装网络相关接口
    /// </summary>
    public class ControlServerService
    {
        /// <summary>
        /// 日志实例类
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 关于服务
        /// </summary>
        private AboutInfoService aboutInfoService;

        /// <summary>
        /// 流程状态
        /// </summary>
        public enum enProcessStatus
        {
            Stop,
            Start,
            Exception
        }

        /// <summary>
        /// AES加解密实例
        /// </summary>
        private AES256 aes256 = new AES256();

        public ControlServerService()
        {
            aboutInfoService = new AboutInfoService();
        }

        /// <summary>
        /// 新建GUID
        /// </summary>
        /// <returns>guid值</returns>
        private string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 注册机器信息到控制中心
        /// </summary>
        public async void Register()
        {
            if (!ViewModelLocator.Instance.UserPreferences.IsEnableControlServer)
            {
                return;
            }

            try
            {
                //先判断机器信息是否存在
                string find_dict = ViewModelLocator.Instance.UserPreferences.ControlServerUri + "/OSPServer/rest/dict/finddict";
                string add_info = ViewModelLocator.Instance.UserPreferences.ControlServerUri + "/OSPServer/rest/dict/savedict";
                string update_info = ViewModelLocator.Instance.UserPreferences.ControlServerUri + "/OSPServer/rest/dict/savedict";

                JObject jObj = new JObject();
                jObj["DCT_ID"] = "MACHINE_INFO";
                jObj["MACHINENAME"] = aboutInfoService.GetMachineName();

                //记录日志，以便排查错误
                Logger.Debug("Register() 查询机器信息 请求内容为:" + jObj.ToString(), logger);

                var result = await find_dict.PostJsonAsync(jObj).ReceiveJson<JObject>();
                if ((int)result["code"] != 1)
                {
                    Logger.Debug("查询机器信息失败！" + jObj.ToString(), logger);

                    return;
                }

                //记录日志，以便排查错误
                Logger.Debug("Register() 查询机器信息 请求结果为:" + result.ToString(), logger);

                if (result["data"]["rowSetArray"] == null)
                {
                    //说明之前未注册过该机器，则执行插入数据功能
                    var guid = NewGuid();
                    jObj = new JObject();
                    jObj["DCT_ID"] = "MACHINE_INFO";
                    var jArr = new JArray();

                    JObject jItem = new JObject();
                    jItem["F_GUID"] = guid;
                    jItem["MACHINENAME"] = aboutInfoService.GetMachineName();
                    jItem["ROBOTVERSION"] = aboutInfoService.GetVersion();
                    jItem["MACHINEIP"] = aboutInfoService.GetIp();
                    jArr.Add(jItem);

                    jObj["InsertPool"] = jArr;

                    //记录日志，以便排查错误
                    Logger.Debug("Register() 增加机器信息 请求内容为:" + jObj.ToString(), logger);

                    result = await add_info.PostJsonAsync(jObj).ReceiveJson<JObject>();
                    if ((int)result["code"] != 1)
                    {
                        Logger.Debug("增加机器信息失败！" + jObj.ToString(), logger);
                        return;
                    }

                    //记录日志，以便排查错误
                    Logger.Debug("Register() 增加机器信息 请求结果为:" + result.ToString(), logger);
                }
                else
                {
                    //之前已经存在过该机器的信息，需要执行更新逻辑
                    if (result["data"]["rowSetArray"].Count() == 1)
                    {
                        jObj = new JObject();
                        jObj["DCT_ID"] = "MACHINE_INFO";
                        var jArr = new JArray();

                        JObject jItem = result["data"]["rowSetArray"][0]["dataMap"] as JObject;
                        jItem.Remove("F_CHDATE");//更新时间不传，以便服务器自动生成
                        jItem["MACHINENAME"] = aboutInfoService.GetMachineName();
                        jItem["ROBOTVERSION"] = aboutInfoService.GetVersion();
                        jItem["MACHINEIP"] = aboutInfoService.GetIp();
                        jArr.Add(jItem);

                        jObj["UpdatePool"] = jArr;

                        //记录日志，以便排查错误
                        Logger.Debug("Register() 更新机器信息 请求内容为:" + jObj.ToString(), logger);

                        result = await update_info.PostJsonAsync(jObj).ReceiveJson<JObject>();
                        if ((int)result["code"] != 1)
                        {
                            Logger.Debug("更新机器信息失败！" + jObj.ToString(), logger);
                            return;
                        }

                        //记录日志，以便排查错误
                        Logger.Debug("Register() 更新机器信息 请求结果为:" + result.ToString(), logger);
                    }
                    else
                    {
                        Logger.Debug("查询机器信息发现数量超过1个！" + result.ToString(), logger);
                        return;
                    }
                }
            }
            catch (Exception err)
            {
                Logger.Debug(err, logger);
            }

        }

        /// <summary>
        /// 获取当前机器分配的所有流程列表
        /// </summary>
        /// <returns>流程列表</returns>
        public async Task<JArray> GetProcesses()
        {
            if (!ViewModelLocator.Instance.UserPreferences.IsEnableControlServer)
            {
                return null;
            }

            try
            {
                string find_dict = ViewModelLocator.Instance.UserPreferences.ControlServerUri + "/OSPServer/rest/dict/finddict";
                JObject jObj = new JObject();
                jObj["DCT_ID"] = "PROCESS_ALLOCATION";
                jObj["MACHINENAME"] = aboutInfoService.GetMachineName();

                //记录日志，以便排查错误
                Logger.Debug("GetProcesses() 查询流程信息 请求内容为:" + jObj.ToString(), logger);

                var result = await find_dict.PostJsonAsync(jObj).ReceiveJson<JObject>();
                if ((int)result["code"] != 1)
                {
                    Logger.Debug("查询流程信息失败！" + jObj.ToString(), logger);

                    return null;
                }

                //记录日志，以便排查错误
                Logger.Debug("GetProcesses() 查询流程信息 请求结果为:" + result.ToString(), logger);

                var jRetArray = new JArray();
                if (result["data"]["rowSetArray"] != null)
                {
                    JArray jArr = result["data"]["rowSetArray"] as JArray;
                    for (int i = 0; i < jArr.Count; i++)
                    {
                        jObj = new JObject();
                        var dataMap = jArr[i]["dataMap"];
                        jObj["PROCESSNAME"] = dataMap["PROCESSNAME"];
                        jObj["PROCESSVERSION"] = dataMap["PROCESSVERSION"];

                        string fileName = jObj["PROCESSNAME"].ToString() + "." + jObj["PROCESSVERSION"].ToString() + ".nupkg";
                        jObj["NUPKGFILENAME"] = fileName;

                        string nupkgUrl = $"{ViewModelLocator.Instance.UserPreferences.ControlServerUri}/OSPServer/rest/affix/dict/download{dataMap["AFFIXURL"].ToString()}";
                        jObj["NUPKGURL"] = nupkgUrl;

                        jRetArray.Add(jObj);
                    }
                }

                return jRetArray;
            }
            catch (Exception err)
            {
                Logger.Debug(err, logger);
            }

            return null;
        }

        /// <summary>
        /// 获取待运行的流程
        /// </summary>
        /// <returns>程序JSON对象</returns>
        public async Task<JObject> GetRunProcess()
        {
            if (!ViewModelLocator.Instance.UserPreferences.IsEnableControlServer)
            {
                return null;
            }

            try
            {
                string find_dict_status = ViewModelLocator.Instance.UserPreferences.ControlServerUri + "/OSPServer/rest/dict/finddictstatus";
                JObject jObj = new JObject();
                jObj["DCT_ID"] = "PROCESS_ALLOCATION";
                jObj["MACHINENAME"] = aboutInfoService.GetMachineName();

                //记录日志，以便排查错误
                Logger.Debug("GetRunProcess() 查询流程状态为启动时的信息 请求内容为:" + jObj.ToString(), logger);

                var result = await find_dict_status.PostJsonAsync(jObj).ReceiveJson<JObject>();
                if ((int)result["code"] != 1)
                {
                    Logger.Debug("查询流程状态为启动时的信息失败！" + jObj.ToString(), logger);

                    return null;
                }

                //记录日志，以便排查错误
                Logger.Debug("GetRunProcess() 查询流程状态为启动时的信息 请求结果为:" + result.ToString(), logger);

                if (result["data"]["rowSetArray"] != null)
                {
                    JArray jArr = result["data"]["rowSetArray"] as JArray;
                    if(jArr.Count() == 1)
                    {
                        return jArr[0]["dataMap"] as JObject;
                    }
                    else
                    {
                        Logger.Debug("查询流程状态为启动时的信息发现数量超过1个！" + result.ToString(), logger);
                        return null;
                    }
                }
            }
            catch (Exception err)
            {
                Logger.Debug(err, logger);
            }

            return null;
        }

        /// <summary>
        /// 更新运行状态到控制中心
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="projectVersion">项目版本</param>
        /// <param name="status">状态</param>
        public async Task UpdateRunStatus(string projectName,string projectVersion, enProcessStatus status)
        {
            if (!ViewModelLocator.Instance.UserPreferences.IsEnableControlServer)
            {
                return;
            }

            try
            {
                string find_dict = ViewModelLocator.Instance.UserPreferences.ControlServerUri + "/OSPServer/rest/dict/finddict";
                string update_info = ViewModelLocator.Instance.UserPreferences.ControlServerUri + "/OSPServer/rest/dict/savedict";

                //先查询指定机器名对应的所有流程及版本信息，然后找到待更新的条目，再更新（后端后期需要优化这块，不然查询数据量越来越大）
                JObject jObj = new JObject();
                jObj["DCT_ID"] = "PROCESS_ALLOCATION";
                jObj["MACHINENAME"] = aboutInfoService.GetMachineName();

                //记录日志，以便排查错误
                Logger.Debug("UpdateRunStatus() 查询本机所对应的的所有流程分配信息 请求内容为:" + jObj.ToString(), logger);

                var result = await find_dict.PostJsonAsync(jObj).ReceiveJson<JObject>();
                if ((int)result["code"] != 1)
                {
                    Logger.Debug("查询本机所对应的的所有流程分配信息失败！" + jObj.ToString(), logger);
                    return ;
                }

                //记录日志，以便排查错误
                Logger.Debug("UpdateRunStatus() 查询本机所对应的的所有流程分配信息 请求结果为:" + result.ToString(), logger);

                JObject machedObject = null;
                if (result["data"]["rowSetArray"] != null)
                {
                    var jArr = result["data"]["rowSetArray"] as JArray;
                    if (jArr.Count() > 0 )
                    {
                        for(int i=0;i< jArr.Count();i++)
                        {
                            var dataMap = jArr[i]["dataMap"] as JObject;

                            if (dataMap["PROCESSNAME"].ToString() == projectName && dataMap["PROCESSVERSION"].ToString() == projectVersion)
                            {
                                machedObject = dataMap;
                            }
                        }
                    }

                }

                if(machedObject != null)
                {
                    jObj = new JObject();
                    jObj["DCT_ID"] = "PROCESS_ALLOCATION";
                    var jArr = new JArray();

                    JObject jItem = machedObject;
                    jItem.Remove("F_CHDATE");//更新时间不传，以便服务器自动生成
                    jItem["PROCESSSTATUS"] = ((int)status).ToString();
                    jArr.Add(jItem);

                    jObj["UpdatePool"] = jArr;

                    //记录日志，以便排查错误
                    Logger.Debug("UpdateRunStatus() 更新流程状态信息 请求内容为:" + jObj.ToString(), logger);

                    result = await update_info.PostJsonAsync(jObj).ReceiveJson<JObject>();
                    if ((int)result["code"] != 1)
                    {
                        Logger.Debug("更新流程状态信息失败！" + jObj.ToString(), logger);
                        return;
                    }

                    //记录日志，以便排查错误
                    Logger.Debug("UpdateRunStatus() 更新流程状态信息 请求结果为:" + result.ToString(), logger);
                }
            }
            catch (Exception err)
            {
                Logger.Debug(err, logger);
            }
        }

        /// <summary>
        /// 日志记录到控制中心
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="projectVersion">项目版本</param>
        /// <param name="level">日志等级</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        public async Task Log(string projectName,string projectVersion, string level,string msg)
        {
            if (!ViewModelLocator.Instance.UserPreferences.IsEnableControlServer)
            {
                return;
            }

            //增加一个日志
            string add_info = ViewModelLocator.Instance.UserPreferences.ControlServerUri + "/OSPServer/rest/dict/savedict";

            JObject jObj = new JObject();
            jObj["DCT_ID"] = "LOG_INFO";
            var jArr = new JArray();

            JObject jItem = new JObject();
            jItem["F_GUID"] = NewGuid();
            jItem["MACHINENAME"] = aboutInfoService.GetMachineName();

            jItem["PROCESSNAME"] = projectName;
            jItem["PROCESSVERSION"] = projectVersion;

            jItem["LOGLEVEL"] = level;
            jItem["LOGCONTENT"] = aes256.Encrypt(msg, "ABCDE");

            jArr.Add(jItem);

            jObj["InsertPool"] = jArr;

            //记录日志，以便排查错误
            Logger.Debug("Log() 增加日志信息 请求内容为:" + jObj.ToString(), logger);

            var result = await add_info.PostJsonAsync(jObj).ReceiveJson<JObject>();
            if ((int)result["code"] != 1)
            {
                Logger.Debug("增加日志信息失败！" + jObj.ToString(), logger);
                return;
            }

            //记录日志，以便排查错误
            Logger.Debug("Log() 增加日志信息 请求结果为:" + result.ToString(), logger);
        }


    }
}
