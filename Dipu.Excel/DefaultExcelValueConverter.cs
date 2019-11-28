﻿using System.Linq;

namespace Dipu.Excel
{
    public class DefaultExcelConverter : IExcelValueConverter
    {
        public object Convert(ICell cell, object excelValue)
        {
            if (cell.Value is decimal @decimal && excelValue is double @double)
            {
                const double decimalMin = (double)decimal.MinValue;
                const double decimalMax = (double)decimal.MaxValue;

                if (@double < decimalMin)
                {
                    return decimalMin;
                }

                if (@double > decimalMax)
                {
                    return decimalMax;
                }

                return System.Convert.ToDecimal(excelValue);
            }

            if (cell.Value is string @string)
            {
                if (excelValue == null)
                {
                    return string.Empty;
                }

                if (excelValue is double)
                {
                    return excelValue.ToString();
                }
            }
            
            return excelValue;
        }
    }
}
