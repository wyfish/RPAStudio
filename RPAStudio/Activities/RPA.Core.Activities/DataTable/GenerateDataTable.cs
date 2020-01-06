using RPA.Core.Activities.DataTableActivity.Operators;
using Plugins.Shared.Library.Librarys;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(GenerateDataTableDesigner))]
    public sealed class GenerateDataTable : TaskAsyncCodeActivity<DataTable>
    {
        public GenerateDataTable()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "GenerateDataTable";
            }
        }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDescription("Description32")] //指定要转换为表的非结构化数据的来源。该字段仅支持String变量 //Specifies the source of unstructured data to be converted to a table.  This field only supports String variables. //テーブルに変換する非構造化データのソースを指定します。 このフィールドは文字列変数のみをサポートします。
        public InArgument<string> InputString { get; set; }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(false)]
        [Localize.LocalizedDisplayName("DisplayName10")] //位置 //position //場所
        [Localize.LocalizedDescription("Description33")] //如果使用OCR进行抓取，并且选中了“获取单词信息”复选框，则此字段包含IEnumerable<KeyValuePair<Rectangle,String>>具有WordsInfo值的变量 //If you use OCR for fetching and the Get Word Information check box is selected, this field contains IEnumerable<KeyValuePair<Rectangle,String>> variables with WordsInfo values //フェッチにOCRを使用し、[単語情報の取得]チェックボックスがオンになっている場合、このフィールドにはWordsInfo値を持つIEnumerable <KeyValuePair <Rectangle、String >>変数が含まれます。
        public InArgument<IEnumerable<KeyValuePair<Rectangle, String>>> Positions { get; set; }

        [Localize.LocalizedCategory("Category5")] //固定格式选项 //Fixed format option //固定形式オプション
        [Localize.LocalizedDisplayName("DisplayName11")] //列宽 //Column width //列幅
        [Localize.LocalizedDescription("Description34")] //指定要在表中创建的列的大小。该字段仅支持IEnumerable <Int32>变量。 //Specifies the size of the column to be created in the table.  This field only supports the IEnumerable <Int32> variable. //テーブルに作成される列のサイズを指定します。 このフィールドは、IEnumerable <Int32>変数のみをサポートします。
        public InArgument<IEnumerable<Int32>> ColumnSizes { get; set; }

        [Localize.LocalizedCategory("Category6")] //格式化选项 //Formatting options //書式設定オプション
        [Localize.LocalizedDisplayName("DisplayName12")] //列分隔符 //Column separator //列区切り
        [Localize.LocalizedDescription("Description35")] //指定要用作列分隔符的字符。该字段仅支持String变量 //Specifies the character to use as the column separator.  This field only supports String variables. //列区切りとして使用する文字を指定します。 このフィールドは文字列変数のみをサポートします。
        public InArgument<string> ColumnSeparators { get; set; }

        [Localize.LocalizedCategory("Category6")] //格式化选项 //Formatting options //書式設定オプション
        [Localize.LocalizedDisplayName("DisplayName13")] //行分隔符 //Line separator //行区切り
        [Localize.LocalizedDescription("Description36")] //指定要用作换行符分隔符的字符。该字段仅支持String变量 //Specifies the character to be used as a newline separator.  This field only supports String variables. //改行セパレーターとして使用される文字を指定します。 このフィールドは文字列変数のみをサポートします。
        public InArgument<string> NewLineSeparator { get; set; }

        [Localize.LocalizedCategory("Category6")] //格式化选项 //Formatting options //書式設定オプション
        [Localize.LocalizedDisplayName("DisplayName14")] //CSV解析 //CSV parsing //CSV解析
        [Localize.LocalizedDescription("DisplayName14")] //CSV解析 //CSV parsing //CSV解析
        public InArgument<bool> CSVParse { get; set; }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName15")] //类型自动检测 //Type automatic detection //タイプ自動検出
        [Localize.LocalizedDescription("Description37")] //选中后，自动检测列或行类型，无论是String，Int32等。 //When selected, the column or row type is automatically detected, whether it is String, Int32, etc. //選択すると、文字列、Int32など、列または行のタイプが自動的に検出されます。
        public bool AutoDetectTypes { get; set; }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName16")] //列标题 //Column heading //列見出し
        [Localize.LocalizedDescription("Description38")] //如果选中，则使用第一个标识的列作为列标题 //If checked, the first identified column is used as the column header //チェックした場合、最初に識別された列が列ヘッダーとして使用されます
        public bool UseColumnHeader { get; set; }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName17")] //行标题 //Row header //行ヘッダー
        [Localize.LocalizedDescription("Description39")] //如果选中，则使用第一个标识的行作为行标题 //If checked, the first identified row is used as the row header //チェックした場合、最初に識別された行が行ヘッダーとして使用されます
        public bool UseRowHeader { get; set; }


        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description40")] //生成的DataTable变量 //Generated DataTable variable //生成されたDataTable変数
        public OutArgument<DataTable> DataTable { get; set; }


        [Browsable(false), DefaultValue(false)]
        public bool PreserveStrings
        {
            get;
            set;
        }
        [Browsable(false), DefaultValue(false)]
        public bool PreserveNewLines
        {
            get;
            set;
        }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }



        private delegate string runDelegate();
        
        public string Run()
        {
            return DisplayName;
        }

        protected override Task<DataTable> ExecuteAsyncWithResult(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            IEnumerable<KeyValuePair<Rectangle, string>> positions = this.Positions.Get(context);
            string input = this.InputString.Get(context);
            ITableOptions tableOptions = this.GetTableOptions();
            IFormatOptions formatOptions = this.GetFormatOptions(context);
            return Task.Factory.StartNew<DataTable>(delegate
            {
                TableFormatter tableFormatter = new TableFormatter();
                if (positions == null)
                {
                    return tableFormatter.Format(input, formatOptions, tableOptions);
                }
                return tableFormatter.Format(positions, tableOptions);
            });
        }

        protected override void OutputResult(AsyncCodeActivityContext context, DataTable result)
        {
            this.DataTable.Set(context, result);
        }

        private ITableOptions GetTableOptions()
        {
            return new TableOptions
            {
                AutoDetectTypes = this.AutoDetectTypes,
                UseColumnHeader = this.UseColumnHeader,
                UseRowHeader = this.UseRowHeader
            };
        }

        private IFormatOptions GetFormatOptions(AsyncCodeActivityContext context)
        {
            return new FormatOptions
            {
                ColumnSeparators = this.ColumnSeparators.Get(context),
                NewLineSeparator = this.NewLineSeparator.Get(context),
                CSVParsing = this.CSVParse.Get(context),
                ColumnSizes = this.ColumnSizes.Get(context),
                PreserveNewLines = this.PreserveNewLines,
                PreserveStrings = this.PreserveStrings
            };
        }
    }


    public abstract class TaskAsyncCodeActivity<T> : AsyncCodeActivity
    {
        [DefaultValue(null)]
        public InArgument<bool> ContinueOnError
        {
            get;
            set;
        }

        protected sealed override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            context.UserState = cancellationTokenSource;
            Task<T> arg_24_0 = this.ExecuteAsyncWithResult(context, cancellationTokenSource.Token);
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>(state);
            arg_24_0.Unwrap(taskCompletionSource, callback, cancellationTokenSource);
            return taskCompletionSource.Task;
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Task<T> task = (Task<T>)result;
            if (task == null)
            {
                this.OutputResult(context, default(T));
            }
            try
            {
                if (task.IsFaulted)
                {
                    throw task.Exception.InnerException ?? task.Exception;
                }
                if (task.IsCanceled || context.IsCancellationRequested)
                {
                    context.MarkCanceled();
                }
                else
                {
                    this.OutputResult(context, task.Result);
                }
            }
            catch (OperationCanceledException)
            {
                context.MarkCanceled();
            }
            catch
            {
                if (!this.ContinueOnError.Get(context))
                {
                    throw;
                }
            }
        }

        protected override void Cancel(AsyncCodeActivityContext context)
        {
            ((CancellationTokenSource)context.UserState).Cancel();
        }

        protected abstract Task<T> ExecuteAsyncWithResult(AsyncCodeActivityContext context, CancellationToken cancellationToken);

        protected abstract void OutputResult(AsyncCodeActivityContext context, T result);
    }
}
