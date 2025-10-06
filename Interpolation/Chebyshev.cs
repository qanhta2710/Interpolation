using System;

namespace Interpolation
{
    public class InterpolationPoints
    {
        public static double[] FindOptimizedPoints(int n, double a, double b, int precision)
        {
            double[] x = new double[n + 1];
            for (int i = 0; i <= n; i++)
            {
                x[i] = 0.5 * (a + b) + 0.5 * (b - a) * Math.Cos((2 * i + 1) * Math.PI / (2 * (n + 1)));
                x[i] = Math.Round(x[i], precision);
            }
            return x;
        }
    }
}
