using Plugins.Shared.Library.Converters;
using Plugins.Shared.Library.Librarys;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPA.Core.Activities.DataTableActivity.Operators
{
    static class DataTableExtensions
    {
        public static int? GetColumnIndex(this DataTable dt, InArgument arg, ActivityContext context, bool throwIfNotFound = false)
        {
            object argumentValue = (arg != null) ? arg.Get<object>(context) : null;
            if (dt == null)
            {
                return null;
            }
            return dt.GetColumnIndex(argumentValue, throwIfNotFound);
        }


            // Token: 0x06000073 RID: 115 RVA: 0x00003894 File Offset: 0x00001A94
        public static void SetFirstFilterOperator(this List<FilterOperationArgument> filters, BooleanOperator booleanOperator)
        {
            FilterOperationArgument filterOperationArgument = (filters != null) ? filters.FirstOrDefault<FilterOperationArgument>() : null;
            if (filterOperationArgument != null)
            {
                filterOperationArgument.BooleanOperator = booleanOperator;
            }
        }
        

        public static int? GetColumnIndex(this DataTable dt, object argumentValue, bool throwIfNotFound = false)
        {
            int? result = null;
            if (dt == null || argumentValue == null)
            {
                result = null;
            }
            if (argumentValue != null)
            {
                GenericValue value = argumentValue as GenericValue;
                argumentValue = GenericValue.GetRawValue(value);
            }
            int? num = argumentValue as int?;
            DataColumn dataColumn = argumentValue as DataColumn;
            string text = argumentValue as string;
            text = (string.IsNullOrEmpty(text) ? null : text);
            if (dataColumn != null)
            {
                num = new int?(dt.Columns.IndexOf(dataColumn));
            }
            if (text != null)
            {
                num = new int?(dt.Columns.IndexOf(text));
            }
            if (num.HasValue)
            {
                result = ((num < 0 || dt.Columns.Count <= num) ? null : num);
            }
            if (!result.HasValue & throwIfNotFound)
            {
                string text2 = (argumentValue is string) ? 
                    string.Format("ValueNotSetForArgument", "ColumnName") : ((argumentValue is int) ? 
                    string.Format("ValueNotSetForArgument", "ColumnIndex") : string.Format("ValueNotSetForArgument", "Column"));
                text2 = char.ToLowerInvariant(text2[0]).ToString() + text2.Substring(1);
                throw new ArgumentException(text2);
            }
            return result;
        }

        public static Task Unwrap<T>(this Task<T> task, TaskCompletionSource<T> tcs, AsyncCallback callback = null, CancellationTokenSource cts = null)
        {
            return task.ContinueWith(delegate (Task<T> t)
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (cts.IsCancellationRequested || t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(t.Result);
                }
                AsyncCallback expr_5C = callback;
                if (expr_5C == null)
                {
                    return;
                }
                expr_5C(tcs.Task);
            });
        }
    }
}
