using System;

namespace Interpolation
{
    public class Horner
    {
        public static double[,] ProductTable(double[] x, int precision)
        {
            int cols = x.Length + 1;
            int rows = x.Length + 1;
            double[,] productTable = new double[rows, cols];
            productTable[0, cols - 1] = 1;
            for (int i = 1; i < rows; i++)
            {
                for (int j = cols - 1; j >= cols - i - 1; j--)
                {
                    if (j == cols - 1)
                    {
                        productTable[i, j] = -x[i - 1] * productTable[i - 1, j];
                        productTable[i, j] = Math.Round(productTable[i, j], precision);
                    }
                    else if (j == cols - i - 1)
                    {
                        productTable[i, j] = 1;
                    }
                    else
                    {
                        productTable[i, j] = -x[i - 1] * productTable[i - 1, j] + productTable[i - 1, j + 1];
                        productTable[i, j] = Math.Round(productTable[i, j], precision);
                    }
                }
            }

            return productTable;

        }
    }
}
