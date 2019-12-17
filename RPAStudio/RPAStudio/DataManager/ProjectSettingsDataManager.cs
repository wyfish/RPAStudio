using log4net;
using Newtonsoft.Json.Linq;
using RPAStudio.Librarys;

namespace RPAStudio.DataManager
{
    class ProjectSettingsDataManager
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ProjectSettingsDataManager Instance = new ProjectSettingsDataManager();

        public string m_projectPath { get; set; }

        /// <summary>
        /// 保存项目断点信息
        /// </summary>
        public ProjectBreakpointsDataManager m_projectBreakpointsDataManager = new ProjectBreakpointsDataManager();


        public static void ResetInstance()
        {
            Instance = new ProjectSettingsDataManager();
        }

        /// <summary>
        /// 放在项目刚打开的时候，加载该项目的ProjectSettings.json
        /// </summary>
        public void Load(string projectPath)
        {
            Logger.Debug(string.Format("打开项目{0}", projectPath), logger);

            m_projectPath = projectPath;
            //加载项目配置
            var projectSettingsjson = m_projectPath + @"\.local\ProjectSettings.json";
            if (System.IO.File.Exists(projectSettingsjson))
            {
                string json = System.IO.File.ReadAllText(projectSettingsjson);
                JObject rootJsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json) as JObject;

                var valueJsonObj = rootJsonObj["ProjectBreakpoints"]["Value"];
                if (valueJsonObj != null)
                {
                    foreach (JProperty jp in valueJsonObj)
                    {
                        m_projectBreakpointsDataManager.m_breakpointsDict[jp.Name] = (JArray)jp.Value;
                    }
                }
            }

        }

        /// <summary>
        /// 放在项目刚关闭的时候，保存该项目的ProjectSettings.json
        /// </summary>
        public void Unload()
        {
            if (!string.IsNullOrEmpty(m_projectPath))
            {
                Logger.Debug(string.Format("关闭项目{0}", m_projectPath), logger);

                if (m_projectBreakpointsDataManager.m_breakpointsDict.Count > 0)
                {
                    //保存项目配置
                    JObject rootJsonObj = new JObject();
                    JObject projectBreakpointsjsonObj = new JObject();
                    rootJsonObj["ProjectBreakpoints"] = projectBreakpointsjsonObj;

                    JObject valueJsonObj = new JObject();
                    projectBreakpointsjsonObj["Value"] = valueJsonObj;

                    foreach (var item in m_projectBreakpointsDataManager.m_breakpointsDict)
                    {
                        JArray jarrayJsonObj = new JArray();
                        valueJsonObj[item.Key] = jarrayJsonObj;

                        foreach (JToken ji in item.Value)
                        {
                            JObject itemJsonObj = new JObject();
                            itemJsonObj["ActivityId"] = (ji as JObject)["ActivityId"].ToString();
                            itemJsonObj["IsValid"] = true;//暂时让IsValid必为true
                            itemJsonObj["IsEnabled"] = (bool)(ji as JObject)["IsEnabled"];
                            jarrayJsonObj.Add(itemJsonObj);
                        }
                        
                    }

                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(rootJsonObj, Newtonsoft.Json.Formatting.Indented);
                    var projectSettingsjson = m_projectPath + @"\.local\ProjectSettings.json";

                    //创建.local隐藏目录
                    var localDir = m_projectPath + @"\.local";
                    if (!System.IO.Directory.Exists(localDir))
                    {
                        System.IO.Directory.CreateDirectory(localDir);
                        System.IO.File.SetAttributes(localDir, System.IO.FileAttributes.Hidden);
                    }

                    System.IO.File.WriteAllText(projectSettingsjson, output);
                }

            }

        }
    }
}
