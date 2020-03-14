using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.ComponentModel;
using Plugins.Shared.Library;

namespace RPA.Core.Activities.Workflow
{
    [Designer(typeof(RetryScopeDesigner))]
    public sealed class RetryScopeActivity : Activity
    { 
        //1
        [Browsable(false)]
		public ActivityDelegate ActivityBody
        {
            get;
            set;
        }

        [Browsable(false)]
        public ActivityFunc<bool> Condition
        {
            get;
            set;
        }
        [Category("Options")]
        [RequiredArgument]
        public InArgument<int> NumberOfRetries
        {
            get;
            set;
        }
        [Category("Options")]
        [RequiredArgument]
        public InArgument<TimeSpan> RetryInterval
        {
            get;
            set;
        }
        [DefaultValue(false)]
        [Category("Common")]
        public InArgument<bool> ContinueOnError
        {
            get;
            set;
        }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Workflow/ReteyScope.png";
            }
        }
        public RetryScopeActivity()
        {
          
            this.ActivityBody = new ActivityAction  
            {
                Handler = new Sequence
                {
                   DisplayName = "Action" 
                   // DisplayName = RPA_System_Activities_Design.Action
                },
               
            };
            Condition = new ActivityFunc<bool>();
            this.RetryInterval = new TimeSpan(0, 0, 5);
            NumberOfRetries = 3;
            Variable<int> retryNo = new Variable<int>("retryNo", (ActivityContext ctx) => this.NumberOfRetries.Get(ctx));
            Variable<TimeSpan> retryInterval = new Variable<TimeSpan>("retryInterval", (ActivityContext ctx) => this.RetryInterval.Get(ctx));
            Variable<bool> continueOnError = new Variable<bool>("continueOnError", (ActivityContext ctx) => this.ContinueOnError.Get(ctx));
            Variable<bool> conditionResult = new Variable<bool>("conditionResult");
            DelegateInArgument<Exception> exception = new DelegateInArgument<Exception>("exception");
            Exception ex = new Exception("Action failed to execute as expected.");
            this.Implementation = delegate
            {
                return new Sequence
                {
                    Activities =
                    {
                        new While
                        {
                            Variables =
                            {
                                retryNo,
                                retryInterval,
                                continueOnError,
                                conditionResult
                            },
                            Condition = new GreaterThan<int, int, bool>
                            {
                                Left = new InArgument<int>((ActivityContext ctx) => retryNo.Get(ctx)),
                                Right = 0
                            },
                            Body = new TryCatch
                            {
                                Try = new Sequence
                                {
                                    Activities =
                                    {
                                        new Assign<bool>
                                        {
                                            To = conditionResult,
                                            Value = true
                                        },
                                        new InvokeDelegate
                                        {
                                            //DisplayName = RPA_System_Activities.RetryScope,
                                              DisplayName="RetryScope",

                                             Delegate = this.ActivityBody

                                        },
                                        new If
                                        {
                                            Condition = new InArgument<bool>(this.Condition.Handler != null),
                                            Then = new InvokeFunc<bool>
                                            {
                                                Func = this.Condition,
                                                Result = new OutArgument<bool>(conditionResult)
                                            }
                                        },
                                        new If
                                        {
                                            Condition = conditionResult,
                                            Then = new Assign<int>
                                            {
                                                To = retryNo,
                                                Value = 0
                                            },
                                            Else = new Throw
                                            {
                                                Exception = new InArgument<Exception>((ActivityContext ctx) => ex),
                                                DisplayName = this.DisplayName
                                            }
                                        }
                                    },
                                   // DisplayName =RPA_System_Activities.Try
                                },
                                Catches =
                                {
                                    new Catch<Exception>
                                    {
                                        Action = new ActivityAction<Exception>
                                        {
                                            Argument = exception,
                                            Handler = new Sequence
                                            {
                                                Activities =
                                                {
                                                    new Assign<int>
                                                    {
                                                        To = retryNo,
                                                        Value = new InArgument<int>((ActivityContext ctx) => retryNo.Get(ctx) - 1)
                                                    },
                                                    new If
                                                    {
                                                        Condition = new InArgument<bool>((ActivityContext ctx) => retryNo.Get(ctx) == 0),
                                                        Then = new If
                                                        {
                                                            Condition = new InArgument<bool>((ActivityContext ctx) => continueOnError.Get(ctx) == false),
                                                            Then = new Throw
                                                            {
                                                                Exception = new InArgument<Exception>((ActivityContext ctx) => exception.Get(ctx)),
                                                                DisplayName = this.DisplayName
                                                            },
                                                            //Else = new LogMessage
                                                            //{
                                                            //    IsUserLog = false,
                                                            //    Message = new InArgument<string>((ActivityContext ctx) => exception.Get(ctx).Message),
                                                            //  //Level = CurentLogLevel.Trace
                                                            //}
                                                        }
                                                    },
                                                    new Delay
                                                    {
                                                        Duration = retryInterval
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            };



        }

    }

}
