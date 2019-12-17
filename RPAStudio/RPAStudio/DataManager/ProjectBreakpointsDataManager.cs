using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPAStudio.DataManager
{
    class ProjectBreakpointsDataManager
    {
        //////断点位置保存和加载
        /*{
"ProjectBreakpoints": {
		"Value": {
			"Main.xaml": [{
				"ActivityId": "1.4",
				"IsEnabled": true,
				"IsValid": true

            }, {
				"ActivityId": "1.2",
				"IsEnabled": true,
				"IsValid": true
			}],
			"Sequence.xaml": [{
				"ActivityId": "1.2",
				"IsEnabled": true,
				"IsValid": true
			}, {
				"ActivityId": "1.4",
				"IsEnabled": true,
				"IsValid": true
			}, {
				"ActivityId": "1.5",
				"IsEnabled": true,
				"IsValid": true
			}]
		}
	}
}
*/


        public Dictionary<string, JArray> m_breakpointsDict = new Dictionary<string, JArray>();


        public void AddBreakpointLocation(string relativeXamlPath, string ActivityId, bool isEnabled)
        {
            RemoveBreakpointLocation(relativeXamlPath, ActivityId);

            if (!m_breakpointsDict.ContainsKey(relativeXamlPath))
            {
                m_breakpointsDict.Add(relativeXamlPath,new JArray());
            }

            var jarr = m_breakpointsDict[relativeXamlPath];
            JObject jobj = new JObject();
            jobj["ActivityId"] = ActivityId;
            jobj["IsEnabled"] = isEnabled;
            jarr.Add(jobj);
        }

        public void RemoveBreakpointLocation(string relativeXamlPath, string ActivityId)
        {
            if (m_breakpointsDict.ContainsKey(relativeXamlPath))
            {
                var jarr = m_breakpointsDict[relativeXamlPath];

                foreach (JToken ji in jarr)
                {
                    if (((JObject)ji)["ActivityId"].ToString() == ActivityId)
                    {
                        ji.Remove();
                        break;
                    }
                }
            }
        }

        public void RemoveAllBreakpointsLocation(string relativeXamlPath)
        {
            m_breakpointsDict.Clear();
        }

        public bool IsBreakpointLocationExist(string relativeXamlPath, string ActivityId,ref bool IsEnabled)
        {
            if (m_breakpointsDict.ContainsKey(relativeXamlPath))
            {
                var jarr = m_breakpointsDict[relativeXamlPath];

                foreach (JToken ji in jarr)
                {
                    if (((JObject)ji)["ActivityId"].ToString() == ActivityId)
                    {
                        IsEnabled = (bool)(((JObject)ji)["IsEnabled"]);
                    }
                }
            }

            return false;
        }

        
    }
}
