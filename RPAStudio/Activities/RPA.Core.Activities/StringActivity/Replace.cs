using System.Activities;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace RPA.Core.Activities.StringActivity
{
    public sealed class Replace: CodeActivity<string>
    {
        public new string DisplayName
        {
            get
            {
                return "Replace";
            }
        }

        [Localize.LocalizedCategory("Category3")] //输入
        [RequiredArgument]
        [DisplayName("Input")]
        [Localize.LocalizedDescription("key255")] //要替换的字符串。
        [Description("")]
        public InArgument<string> Input
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category3")] //输入
        [RequiredArgument]
        [DisplayName("Pattern")]
        [Localize.LocalizedDescription("key254")] //要匹配的正则表达式模式。
        public InArgument<string> Pattern
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category3")] //输入
        [RequiredArgument]
        [DisplayName("RegexOption")]
        public RegexOptions RegexOption
        {
            get;
            set;
        }
        public Replace()
        {
            this.RegexOption = (RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        [Localize.LocalizedCategory("Category3")] //输入
        [RequiredArgument]
        [DisplayName("Replacement")]
        [Localize.LocalizedDescription("key256")] //替换字符串。
        public InArgument<string> Replacement
        {
            get;
            set;
        }

        protected override string Execute(CodeActivityContext context)
        {
            return Regex.Replace(this.Input.Get(context), this.Pattern.Get(context), this.Replacement.Get(context), this.RegexOption);
        }
    }
}
