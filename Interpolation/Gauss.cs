using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation
{
    public class Gauss
    {
        public static double?[,] BuildGaussForwardTable(double[] x, double[] y, int precision)
        {
            int n = x.Length;
            double?[,] table = new double?[n, n + 1];

            // Cột đầu tiên: giá trị x
            for (int i = 0; i < n; i++)
                table[i, 0] = Math.Round(x[i], precision);

            // Cột thứ hai: giá trị y
            for (int i = 0; i < n; i++)
                table[i, 1] = Math.Round(y[i], precision);

            // Các cột sai phân Δy, Δ²y, Δ³y, ...
            for (int j = 2; j <= n; j++)
            {
                for (int i = 0; i <= n - j; i++)
                {
                    // Sai phân tiến: Δ^k y_i = Δ^(k-1) y_(i+1) - Δ^(k-1) y_i
                    if (table[i + 1, j - 1] != null && table[i, j - 1] != null)
                        table[i, j] = table[i + 1, j - 1] - table[i, j - 1];
                    else
                        table[i, j] = null;

                    if (table[i, j] != null)
                        table[i, j] = Math.Round(table[i, j].Value, precision);
                }
            }
            return table;
        }
    }
}
