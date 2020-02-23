using System;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Markup;

namespace RPA.Core.Activities.DataTableActivity
{
    // https://github.com/microsoft/referencesource/blob/master/System.Activities/System/Activities/Statements/ForEach.cs
    // https://github.com/microsoft/referencesource/blob/f82e13c3820cd04553c21bf6da01262b95d9bd43/System.Activities.Core.Presentation/System/Activities/Core/Presentation/ForEachDesigner.xaml
    [Designer(typeof(ForEachRowDesigner))]
    [ContentProperty("Body")]
    public sealed class ForEachRow<T> : NativeActivity
    {
        Variable<IEnumerator<DataRow>> _valueEnumerator;
        CompletionCallback onChildComplete;

        public ForEachRow()
            : base()
        {
            this._valueEnumerator = new Variable<IEnumerator<DataRow>>();

            Body = new ActivityAction<DataRow>
            {
                Argument = new DelegateInArgument<DataRow>()
                {
                    Name = "row"
                },
                Handler = new Sequence()
                {
                    DisplayName = "Body"
                }
            };
        }

        public new string DisplayName {
            get {
                return "For Each Row";
            }
        }

        [Browsable(false)]
        [DefaultValue(null)]
        [DependsOn("Values")]
        public ActivityAction<DataRow> Body {
            get;
            set;
        }

        [RequiredArgument]
        [DefaultValue(null)]
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("xDataTableFieldHint")] //DataTable变量 //DataTable Variable //DataTable変数
        public InArgument<DataTable> Values {
            get;
            set;
        }

        [Browsable(false)]
        public string icoPath {
            get {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }

        CompletionCallback OnChildComplete {
            get {
                if (this.onChildComplete == null)
                {
                    this.onChildComplete = new CompletionCallback(GetStateAndExecute);
                }

                return this.onChildComplete;
            }
        }

        protected override void OnCreateDynamicUpdateMap(System.Activities.DynamicUpdate.NativeActivityUpdateMapMetadata metadata, Activity originalActivity)
        {
            metadata.AllowUpdateInsideThisActivity();
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            RuntimeArgument valuesArgument = new RuntimeArgument("Values", typeof(DataTable), ArgumentDirection.In, true);
            metadata.Bind(this.Values, valuesArgument);

            metadata.AddArgument(valuesArgument);
            metadata.AddDelegate(this.Body);
            metadata.AddImplementationVariable(this._valueEnumerator);
        }

        protected override void Execute(NativeActivityContext context)
        {
            DataTable values = this.Values.Get(context);
            if (values == null)
            {
                throw new InvalidOperationException(string.Format("ForEachRow requires a non null DataTable argument ({0})", this.DisplayName));
            }

            IEnumerator<DataRow> valueEnumerator = values.AsEnumerable().GetEnumerator();
            this._valueEnumerator.Set(context, valueEnumerator);

            if (this.Body == null || this.Body.Handler == null)
            {
                while (valueEnumerator.MoveNext())
                {
                    // do nothing
                };
                OnForEachComplete(valueEnumerator);
                return;
            }
            InternalExecute(context, valueEnumerator);
        }

        void GetStateAndExecute(NativeActivityContext context, ActivityInstance completedInstance)
        {
            IEnumerator<DataRow> valueEnumerator = this._valueEnumerator.Get(context);
            InternalExecute(context, valueEnumerator);
        }

        void InternalExecute(NativeActivityContext context, IEnumerator<DataRow> valueEnumerator)
        {
            if (!valueEnumerator.MoveNext())
            {
                OnForEachComplete(valueEnumerator);
                return;
            }

            // After making sure there is another value, let's check for cancelation
            if (context.IsCancellationRequested)
            {
                context.MarkCanceled();
                OnForEachComplete(valueEnumerator);
                return;
            }

            context.ScheduleAction(this.Body, valueEnumerator.Current, this.OnChildComplete);
        }

        void OnForEachComplete(IEnumerator valueEnumerator)
        {
            IDisposable disposable = (valueEnumerator as IDisposable);
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
