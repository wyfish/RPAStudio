using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Activities.Expressions;
using Microsoft.VisualBasic.Activities;
using Plugins.Shared.Library;

namespace RPA.Core.Activities.DebugActivity
{
    [Designer(typeof(LogMessageDesigner))]
    public sealed class LogMessageActivity : CodeActivity
    {
        private InArgument<object> _message;

        [Category("Log")]
        [RequiredArgument]
        [DisplayName("Message")]
        public InArgument<object> Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = CopyFromLiteralValue(value);
            }
        }

        [Category("Log")]
        [DisplayName("Level")]
        public SharedObject.enOutputType Level
        {
            get;
            set;
        }

        public LogMessageActivity()
        {
            Level = SharedObject.enOutputType.Information;
        }

        private InArgument<object> CopyFromLiteralValue(InArgument<object> arg)
        {
            Literal<object> literal;
            if ((literal = (((arg != null) ? arg.Expression : null) as Literal<object>)) != null)
            {
                return new InArgument<object>((Activity<object>)new VisualBasicValue<object>($"\"{literal.Value?.ToString() ?? string.Empty}\""));
            }
            return arg;
        }


        protected override void Execute(CodeActivityContext context)
        {
            string message = Message.Get(context)?.ToString();

            SharedObject.Instance.Output(Level, message, message);

        }
    }
}
