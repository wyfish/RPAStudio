using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace RPA.Integration.Activities.ExcelPlugins
{
    static class RangeFunction
    {
        internal static void GetRange(Excel.Application excelApp,
                               AsyncCodeActivityContext context,
                               InArgument<string> SheetName,
                               InArgument<string> CellName_Begin,
                               InArgument<string> CellName_End,
                               InArgument<int> CellRow_Begin,
                               InArgument<int> CellColumn_Begin,
                               InArgument<int> CellRow_End,
                               InArgument<int> CellColumn_End,
                               out Excel::_Worksheet sheet,
                               out Excel::Range rangeBegin,
                               out Excel::Range rangeEnd)
        {
            string sheetName = SheetName.Get(context);
            string cellName_Begin = CellName_Begin.Get(context);
            string cellName_End = CellName_End.Get(context);
            int cellRow_Begin = CellRow_Begin.Get(context);
            int cellColumn_Begin = CellColumn_Begin.Get(context);
            int cellRow_End = CellRow_End.Get(context);
            int cellColumn_End = CellColumn_End.Get(context);

            if (sheetName != null && sheetName.Length > 0)
                sheet = excelApp.ActiveWorkbook.Sheets[sheetName];
            else
                sheet = excelApp.ActiveSheet;

            if (cellRow_Begin == 0 && cellColumn_Begin == 0 && string.IsNullOrEmpty(cellName_Begin) &&
                cellRow_End == 0 && cellColumn_End == 0 && string.IsNullOrEmpty(cellName_End))
            {
                // Set UsedRange if every range related values are empty.
                cellRow_Begin = sheet.UsedRange.Row;
                cellColumn_Begin = sheet.UsedRange.Column;
                cellRow_End = cellRow_Begin + sheet.UsedRange.Rows.Count - 1;
                cellColumn_End = cellColumn_Begin + sheet.UsedRange.Columns.Count - 1;
                rangeBegin = sheet.Cells[cellRow_Begin, cellColumn_Begin];
                rangeEnd = sheet.Cells[cellRow_End, cellColumn_End];
            }
            else
            {
                rangeBegin = cellName_Begin == null ? sheet.Cells[cellRow_Begin, cellColumn_Begin] : sheet.Range[cellName_Begin];
                rangeEnd = cellName_End == null ? sheet.Cells[cellRow_End, cellColumn_End] : sheet.Range[cellName_End];
            }
        }
    }
}
