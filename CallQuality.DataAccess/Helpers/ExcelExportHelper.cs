using CallQuality.Core.Manager.ExportManager.Models;
using ClosedXML.Excel;
using System.Data;
using System.Reflection;


namespace CallQuality.Utilities
{
    public static class ExcelExportHelper 
    {
        public static byte[] ExportDataTablesToExcel(List<ExcelExportDTO> data)
        {
            using var workbook = new XLWorkbook();

            foreach (var item in data)
            {
                var sheetName = string.IsNullOrWhiteSpace(item.AssessorsName)
                    ? "Sheet" + (workbook.Worksheets.Count + 1)
                    : item.AssessorsName;

                if (sheetName.Length > 31)
                    sheetName = sheetName[..31];

                var table = item.AssessorChartData;
                var ws = workbook.Worksheets.Add(sheetName);

                // Header
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    ws.Cell(1, col + 1).Value = table.Columns[col].ColumnName;
                    ws.Cell(1, col + 1).Style.Font.Bold = true;
                }

                // Data
                for (int row = 0; row < table.Rows.Count; row++)
                {
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        var cellValue = table.Rows[row][col];
                        ws.Cell(row + 2, col + 1).Value = cellValue switch
                        {
                            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm"),
                            bool b => b ? "Yes" : "No",
                            _ => cellValue?.ToString() ?? string.Empty
                        };
                    }
                }

                ws.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);

            var dataTable = new DataTable(typeof(T).Name);

            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop =>
                    prop.CanRead &&
                    prop.GetIndexParameters().Length == 0)
                .ToArray();

            // Create DataTable columns
            foreach (var prop in properties)
            {
                var columnType =
                    Nullable.GetUnderlyingType(prop.PropertyType)
                    ?? prop.PropertyType;

                dataTable.Columns.Add(prop.Name, columnType);
            }

            // Populate rows
            foreach (var item in items)
            {
                var values = new object[properties.Length];

                for (var i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item) ?? DBNull.Value;
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

    }
}
