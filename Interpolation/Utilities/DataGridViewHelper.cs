using System.Collections.Generic;
using System.Windows.Forms;

namespace Interpolation.Utilities
{
    public static class DataGridViewHelper
    {
        public static void SetupColumns(DataGridView dgv, int dataCols, string noteHeaderText)
        {
            for (int j = 0; j < dataCols; j++)
            {
                dgv.Columns.Add($"col{j}", $"Cột {j}");
            }
            dgv.Columns.Add("note", noteHeaderText);
            dgv.Columns["note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

        }
        public static void AddRow(DataGridView dgv, double[] data, string note)
        {
            var row = new List<object>();
            for (int i = 0; i < data.Length; i++)
            {
                row.Add(data[i]);
            }
            while (row.Count < dgv.Columns.Count - 1) row.Add(""); // Thêm ô trống tới khi đến cột ghi chú
            row.Add(note);
            dgv.Rows.Add(row.ToArray());
        }
        public static void AddTable(DataGridView dgv, double[,] table, string startNote, string endNote = null)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                double[] rowData = new double[table.GetLength(1)];
                for (int j = 0; j < table.GetLength(1); j++) rowData[j] = table[i, j];

                string note = (i == 0) ? startNote : (i == table.GetLength(0) - 1) ? endNote : "";
                AddRow(dgv, rowData, note);
            }
        }
        public static void AddNullableTable(DataGridView dgv, double?[,] table, string note)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                object[] row = new object[dgv.Columns.Count];
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    row[j] = table[i, j]?.ToString() ?? "";
                }
                if (i == table.GetLength(0) - 1) row[dgv.Columns.Count - 1] = note;
                dgv.Rows.Add(row);
            }
        }
        public static void AddSectionBreak(DataGridView dgv) => dgv.Rows.Add();
    }
}
