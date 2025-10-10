using System;

namespace Interpolation
{
    public class Newton
    {
        public static double?[,] BuildDividedDifferenceTable(double[] x, double[] y, int precision)
        {
            int n = x.Length;
            double?[,] table = new double?[n, n + 1];

            // Cột X
            for (int i = 0; i < x.Length; i++)
            {
                table[i, 0] = Math.Round(x[i], precision);
            }
            // Cột Y
            for (int i = 0; i < y.Length; i++)
            {
                table[i, 1] = Math.Round(y[i], precision);
            }
            // Tỷ sai phân
            for (int j = 2; j <= n; j++)
            {
                for (int i = j - 1; i < n; i++)
                {
                    table[i, j] = (table[i, j - 1] - table[i - 1, j - 1]) / (table[i, 0] - table[i - (j - 1), 0]);
                    table[i, j] = table[i, j] != null ? Math.Round(table[i, j].Value, precision) : (double?)null;
                }
            }
            return table;
        }
    }
}
