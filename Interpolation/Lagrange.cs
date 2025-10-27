using System;

namespace Interpolation
{
    public class Lagrange
    {
        public static double[] CoeffsD(double[] x, double[] y, int precision, out double[,] arrMatrix)
        {
            int n = x.Length;
            double[,] arr = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        arr[i, j] = 1;
                    }
                    else
                    {
                        arr[i, j] = x[j] - x[i];
                    }
                }
            }
            arrMatrix = arr;
            double[] productNotation = new double[arr.GetLength(0)];
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                double prod = 1;
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    if (i != j)
                    {
                        prod *= arr[i, j];
                    }
                }
                productNotation[j] = prod;
            }
            double[] coeffsD = new double[productNotation.Length];
            for (int i = 0; i < n; i++)
            {
                coeffsD[i] = y[i] / productNotation[i];
                coeffsD[i] = Math.Round(coeffsD[i], precision);
            }
            return coeffsD;
        }

        public static double[,] DivideTable(double[] x, double[] y, double[] coeffsW, int precision)
        {
            int n = x.Length;
            double[,] divideTable = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (j == 0)
                    {
                        divideTable[i, j] = 1;
                    }
                    else
                    {
                        divideTable[i, j] = x[i] * divideTable[i, j - 1] + coeffsW[j];
                        divideTable[i, j] = Math.Round(divideTable[i, j], precision);
                    }
                }
            }
            return divideTable;
        }
    }
}
