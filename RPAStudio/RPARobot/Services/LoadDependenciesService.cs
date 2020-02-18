using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using Plugins.Shared.Library.Nuget;
using RPARobot.Librarys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RPARobot.Services
{
    /// <summary>
    /// 加载项目依赖包服务
    /// </summary>
    public class LoadDependenciesService
    {
        /// <summary>
        /// 日志实例
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 当前项目project.json文件路径
        /// </summary>
        public string CurrentProjectJsonFile { get;}

        public LoadDependenciesService(string projectJsonFile)
        {
            CurrentProjectJsonFile = projectJsonFile;
        }

        /// <summary>
        /// 加载项目依赖包
        /// </summary>
        public async Task LoadDependencies()
        {
            var json_cfg = ProcessProjectJsonConfig();
            foreach (JProperty jp in (JToken)json_cfg.dependencies)
            {
                var ver_range = VersionRange.Parse((string)jp.Value);
                if (ver_range.IsMinInclusive)
                {
                    var target_ver = NuGet.Versioning.NuGetVersion.Parse(ver_range.MinVersion.ToString());
                    await NuGetPackageController.Instance.DownloadAndInstall(new NuGet.Packaging.Core.PackageIdentity(jp.Name, target_ver));
                }
                else
                {
                    //TODO WJF 大于但不等于低版本时如何处理？
                }
            }

            //加载依赖的DLL
            var targetDir = NuGetPackageController.Instance.TargetFolder;
            string[] dll_files = Directory.GetFiles(targetDir, "*.dll", SearchOption.TopDirectoryOnly);//搜索TargetDir最外层目录下的dll文件

            foreach (var dll_file in dll_files)
            {
                //判断dll_file对应的文件是否在主程序所在目录存在，若存在，则加载主程序所在目录下的DLL，避免加载不同路径下的相同DLL文件
                var checkPath = System.Environment.CurrentDirectory + @"\" + Path.GetFileName(dll_file);
                if (System.IO.File.Exists(checkPath))
                {
                    Logger.Debug($"发现主程序所在目录下存在同名dll，忽略加载 {checkPath}", logger);
                    continue;//避免加载重复的DLL
                }

                var asm = Assembly.LoadFrom(dll_file);
                var dll_file_name_without_ext = Path.GetFileNameWithoutExtension(dll_file);
            }


        }

        /// <summary>
        /// 处理project.json文件
        /// </summary>
        /// <returns>json对象结构</returns>
        public ProjectJsonConfig ProcessProjectJsonConfig()
        {
            var json_str = File.ReadAllText(CurrentProjectJsonFile);
            try
            {
                var json_cfg = JsonConvert.DeserializeObject<ProjectJsonConfig>(json_str);
                if (json_cfg.Upgrade())
                {
                    //文件格式需要升级转换
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(json_cfg, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(CurrentProjectJsonFile, output);

                    json_str = File.ReadAllText(CurrentProjectJsonFile);
                    json_cfg = JsonConvert.DeserializeObject<ProjectJsonConfig>(json_str);
                }

                return json_cfg;
            }
            catch (Exception)
            {
                throw;
            }


        }



    }


    /// <summary>
    /// project.json文件结构
    /// </summary>
    public class ProjectJsonConfig
    {
        private static readonly string initial_schema_version = "2.0.0";//新建项目时的的project.json文件版本
        private static readonly string initial_project_version = "2.0.0";//新建项目时的的项目版本，发布项目时会显示出来

        [JsonProperty(Order = 1)]
        public string schemaVersion { get; set; }//schema版本，project.json文件格式有变时该版本要变动

        [JsonProperty(Required = Required.Always, Order = 2)]
        public string studioVersion { get; set; } //设计器版本

        [JsonProperty(Required = Required.Always, Order = 3)]
        public string projectType { get; set; }//项目类型

        [JsonProperty(Order = 4)]
        public string projectVersion { get; set; }//项目版本

        [JsonProperty(Required = Required.Always, Order = 5)]
        public string name { get; set; }//名称

        [JsonProperty(Order = 6)]
        public string description { get; set; }//描述

        [JsonProperty(Required = Required.Always, Order = 7)]
        public string main { get; set; }//主文件

        [JsonProperty(Order = 8)]
        public JObject dependencies = new JObject();//依赖项

        /// <summary>
        /// 升级project.json配置
        /// </summary>
        /// <returns>是否做了升级操作</returns>
        public bool Upgrade()
        {
            var schemaVersionTmp = schemaVersion;

            Upgrade(ref schemaVersionTmp, initial_schema_version);

            if (schemaVersion == schemaVersionTmp)
            {
                return false;
            }
            else
            {
                schemaVersion = schemaVersionTmp;
                return true;
            }
        }

        /// <summary>
        /// 升级
        /// </summary>
        /// <param name="schemaVersion">schema版本信息</param>
        /// <param name="newSchemaVersion">新的schema版本信息</param>
        private void Upgrade(ref string schemaVersion, string newSchemaVersion)
        {
            if (schemaVersion == newSchemaVersion)
            {
                return;
            }

            Version currentVersion = new Version(schemaVersion);
            Version latestVersion = new Version(newSchemaVersion);

            if (currentVersion < new Version("2.0.0.0"))
            {
                //提示用户不再支持老版本项目，后期考虑再自动升级项目并备份老项目
                //TODO WJF 旧项目如何升级
                var err = "当前程序不再支持V2.0版本以下的旧版本项目！";
                AutoCloseMessageBoxService.Show(err);
                throw new Exception(err);
            }

            Upgrade(ref schemaVersion, initial_schema_version);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            schemaVersion = initial_schema_version;
            projectVersion = initial_project_version;
        }
    }




}
