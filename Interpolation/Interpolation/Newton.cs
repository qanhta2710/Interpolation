using Interpolation.Utilities;
using System;
using System.Linq;
using System.Windows.Forms;
namespace Interpolation
{
    public class Newton
    {
        // Properties
        public double[] Polynomial { get; set; }
        public double?[,] DiffTable { get; set; }
        public double[,] ProductTable { get; set; }
        public double[] DiagonalCoeffs { get; set; }
        public int NodeCount { get; set; }
        public bool? IsForward { get; set; }
        public bool IsDividedDifference { get; set; }
        // Methods
        public Newton(double[] x, double[] y, int precision)
        {
            NodeCount = x.Length;
            IsForward = null;
            IsDividedDifference = true;
            SolveDividedDifference(x, y, precision);
        }
        public Newton(double[] x, double[] y, bool isForward, int precision)
        {
            NodeCount = x.Length;
            IsForward = isForward;
            IsDividedDifference = false;
            SolveFiniteDifference(x, y, precision, isForward);
        }
        private void SolveDividedDifference(double[] x, double[] y, int precision)
        {
            DiffTable = DifferenceTable.BuildDividedDifferenceTable(x, y, precision);
            double[] x_newton = x.Take(x.Length - 1).ToArray();
            ProductTable = Horner.ProductTable(x_newton, precision);
            DiagonalCoeffs = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                DiagonalCoeffs[i] = DiffTable[i, i + 1] ?? 0.0;
            }
            Polynomial = Function.FindPolynomial(DiagonalCoeffs, ProductTable, precision);
        }
        private void SolveFiniteDifference(double[] x, double[] y, int precision, bool isForward)
        {
            DiffTable = DifferenceTable.BuildFiniteDifferenceTable(x, y, precision);
            double[] x_newton = new double[x.Length - 1];
            if (isForward)
            {
                for (int i = 0; i < x.Length - 1; i++)
                {
                    x_newton[i] = i;
                }
            }
            else
            {
                for (int i = 0; i < x.Length - 1; i++)
                {
                    x_newton[i] = -i;
                }
            }
            ProductTable = Horner.ProductTable(x_newton, precision);
            DiagonalCoeffs = new double[x.Length];
            if (isForward)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    DiagonalCoeffs[i] = (DiffTable[i, i + 1] / Function.Factorial(i)) ?? 0.0;
                }
            }
            else
            {
                int lastRow = x.Length - 1;
                for (int i = 0; i < x.Length; i++)
                {
                    DiagonalCoeffs[i] = (DiffTable[lastRow, i + 1] / Function.Factorial(i)) ?? 0.0;
                }
            }
            Polynomial = Function.FindPolynomial(DiagonalCoeffs, ProductTable, precision);
        }
        public void DisplayResults(DataGridView dgv)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            DataGridViewHelper.SetupColumns(dgv, NodeCount + 1, "Ghi chú");

            string tableNote = IsDividedDifference ? "Bảng tỷ sai phân" : "Bảng sai phân hữu hạn";
            DataGridViewHelper.AddNullableTable(dgv, DiffTable, tableNote);
            DataGridViewHelper.AddSectionBreak(dgv);

            DataGridViewHelper.AddRow(dgv, DiagonalCoeffs, "Hệ số trích xuất từ bảng TSP");
            DataGridViewHelper.AddSectionBreak(dgv);

            string productNote = IsDividedDifference
                ? "Bảng tích"
                : (IsForward == true ? "Bảng tích (t = 0, 1, 2, ...)" : "Bảng tích (t = 0, -1, -2, ...)");
            DataGridViewHelper.AddTable(dgv, ProductTable, productNote, "w_{n+1}");
            DataGridViewHelper.AddSectionBreak(dgv);

            DataGridViewHelper.AddRow(dgv, Polynomial, "Hệ số đa thức nội suy");
        }
    }
}
