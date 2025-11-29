using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Interpolation.Utilities
{
    public static class InputHelper
    {
        public static void OpenExcelAndLoadData(DataGridView dgv)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dgv.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dgv.Rows.Add(xValues[i], yValues[i]);
                        }

                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        public static void ReadExcelData(string filePath, out double[] xValues, out double[] yValues)
        {
            var xList = new List<double>();
            var yList = new List<double>();
            var seenXValues = new HashSet<double>();
            int duplicateCount = 0;

            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension?.Rows ?? 0;
                int startRow = 1;
                for (int row = startRow; row <= rowCount; row++)
                {
                    var xCell = worksheet.Cells[row, 1].Value;
                    var yCell = worksheet.Cells[row, 2].Value;
                    if (xCell != null && yCell != null)
                    {
                        if (double.TryParse(xCell.ToString(), out double xValue) &&
                            double.TryParse(yCell.ToString(), out double yValue))
                        {
                            if (!seenXValues.Contains(xValue))
                            {
                                xList.Add(xValue);
                                yList.Add(yValue);
                                seenXValues.Add(xValue);
                            }
                            else
                            {
                                duplicateCount++;
                            }
                        }
                    }
                }
            }
            xValues = xList.ToArray();
            yValues = yList.ToArray();
            if (xValues.Length == 0)
            {
                throw new Exception("Không tìm thấy dữ liệu hợp lệ");
            }
            if (duplicateCount > 0)
            {
                MessageBox.Show(
                    $"Đã loại bỏ {duplicateCount} điểm có giá trị x trùng lặp.\n" +
                    $"Số điểm còn lại: {xValues.Length}",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        public static double[] GetXValues(DataGridView dataGridView)
        {
            int rows = dataGridView.Rows.Count - 1;
            double[] x = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                x[i] = Convert.ToDouble(dataGridView.Rows[i].Cells[0].Value);
            }
            return x;
        }
        public static double[] GetYValues(DataGridView dataGridView)
        {
            int rows = dataGridView.Rows.Count - 1;
            double[] y = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                y[i] = Convert.ToDouble(dataGridView.Rows[i].Cells[1].Value);
            }
            return y;
        }
        public static double[] GetCoeffsP(DataGridView dataGridView)
        {
            int rows = dataGridView.Rows.Count - 1;
            double[] coeffsP = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                coeffsP[i] = Convert.ToDouble(dataGridView.Rows[rows - 1 - i].Cells[0].Value);
            }
            return coeffsP;
        }
        public static void RemoveDuplicate(DataGridView dataGridView)
        {
            var seenXValues = new HashSet<double>();
            int rowIndex = 0;
            while (rowIndex < dataGridView.Rows.Count - 1)
            {
                var cellValue = dataGridView.Rows[rowIndex].Cells[0].Value;
                double xValue = Convert.ToDouble(cellValue);
                if (seenXValues.Contains(xValue))
                {
                    dataGridView.Rows.RemoveAt(rowIndex);
                }
                else
                {
                    seenXValues.Add(xValue);
                    rowIndex++;
                }
            }
        }
        public static void SetupDataGridViewColumnTypes(DataGridView dgv, string xColumnName, string yColumnName)
        {
            if (dgv == null) return;

            SetColumnType(dgv, xColumnName, typeof(double));
            SetColumnType(dgv, yColumnName, typeof(double));
        }
        private static void SetColumnType(DataGridView dgv, string columnName, Type valueType)
        {
            if (dgv?.Columns.Contains(columnName) == true)
            {
                dgv.Columns[columnName].ValueType = valueType;
            }
        }
        public static void ReadExcelDataWithoutDuplicateRemoval(string filePath, out double[] xValues, out double[] yValues)
        {
            var xList = new List<double>();
            var yList = new List<double>();

            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension?.Rows ?? 0;
                int startRow = 1;
                for (int row = startRow; row <= rowCount; row++)
                {
                    var xCell = worksheet.Cells[row, 1].Value;
                    var yCell = worksheet.Cells[row, 2].Value;
                    if (xCell != null && yCell != null)
                    {
                        if (double.TryParse(xCell.ToString(), out double xValue) &&
                            double.TryParse(yCell.ToString(), out double yValue))
                        {
                            xList.Add(xValue);
                            yList.Add(yValue);
                        }
                    }
                }
            }
            xValues = xList.ToArray();
            yValues = yList.ToArray();
            if (xValues.Length == 0)
            {
                throw new Exception("Không tìm thấy dữ liệu hợp lệ");
            }
        }

    }
}
