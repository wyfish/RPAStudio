using System.Collections.Generic;
using System.Activities;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;

namespace RPA.Core.Activities.StringActivity
{
    public sealed class Matches: CodeActivity<IEnumerable<Match>>
    {
        public new string DisplayName
        {
            get
            {
                return "Matches";
            }
        }

        [Localize.LocalizedCategory("Category3")] //输入
        [RequiredArgument]
        [DisplayName("Input")]
        [Localize.LocalizedDescription("key253")] //要搜索匹配项的字符串。
        public InArgument<string> Input { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入
        [RequiredArgument]
        [DisplayName("Pattern")]
        [Localize.LocalizedDescription("key254")] //要匹配的正则表达式模式。
        public InArgument<string> Pattern { get; set;}

        [Localize.LocalizedCategory("Category3")] //输入
        [RequiredArgument]
        [DisplayName("RegexOption")]
        public RegexOptions RegexOption { get; set; }

        public Matches()
        {
            this.RegexOption = (RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }      

        protected override IEnumerable<Match> Execute(CodeActivityContext context)
        {
            return Regex.Matches(this.Input.Get(context), this.Pattern.Get(context), this.RegexOption).Cast<Match>();
        }
    }
}
