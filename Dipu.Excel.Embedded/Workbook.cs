﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InteropWorkbook = Microsoft.Office.Interop.Excel.Workbook;
using InteropWorksheet = Microsoft.Office.Interop.Excel.Worksheet;

namespace Dipu.Excel.Embedded
{
    public class Workbook : IWorkbook
    {
        private readonly Dictionary<InteropWorksheet, Worksheet> worksheetByInteropWorksheet;

        public Workbook(AddIn addIn, InteropWorkbook interopWorkbook)
        {
            this.AddIn = addIn;
            this.InteropWorkbook = interopWorkbook;
            this.worksheetByInteropWorksheet = new Dictionary<InteropWorksheet, Worksheet>();
        }

        public AddIn AddIn { get; }

        public InteropWorkbook InteropWorkbook { get; }

        public IWorksheet CreateSheet(int? index, IWorksheet before = null, IWorksheet after = null)
        {
            InteropWorksheet interopWorksheet;

            if (index.HasValue && index.Value == 0)
            {
                interopWorksheet = (InteropWorksheet)this.InteropWorkbook.Sheets.Add();
            }
            else
            {
                if (before != null)
                {
                    interopWorksheet = (InteropWorksheet)this.InteropWorkbook.Sheets.Add(((Worksheet)before).InteropWorksheet);
                }
                else if (after != null)
                {
                    interopWorksheet = (InteropWorksheet)this.InteropWorkbook.Sheets.Add(null, ((Worksheet)after).InteropWorksheet);
                }
                else
                {
                    var sortedWorksheets = this.worksheetByInteropWorksheet.OrderBy(v => v.Value.Index).Select(v => v.Key).ToArray();
                    InteropWorksheet append = null;
                    if (sortedWorksheets.Any())
                    {
                        if (!index.HasValue || index > sortedWorksheets.Length - 1)
                        {
                            index = sortedWorksheets.Length - 1;
                        }

                        append = sortedWorksheets[index.Value];
                    }

                    interopWorksheet = (InteropWorksheet)this.InteropWorkbook.Sheets.Add(Missing.Value, append, Missing.Value, Missing.Value);
                }
            }

            var worksheet = new Worksheet(this, interopWorksheet);
            this.worksheetByInteropWorksheet.Add(interopWorksheet, worksheet);
            return worksheet;
        }

        public IWorksheet[] Worksheets => this.worksheetByInteropWorksheet.Values.Cast<IWorksheet>().ToArray();
        public void Close(bool? saveChanges = null, string fileName = null)
        {
            this.InteropWorkbook.Close((object)saveChanges ?? Missing.Value, (object)fileName ?? Missing.Value, Missing.Value);
        }

        public Worksheet New(InteropWorksheet interopWorksheet)
        {
            var worksheet = new Worksheet(this, interopWorksheet);
            this.worksheetByInteropWorksheet.Add(interopWorksheet, worksheet);
            return worksheet;
        }
    }
}