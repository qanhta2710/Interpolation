using AngouriMath;
using System;
using System.Collections.Generic;

namespace Interpolation
{
    public class ODESolver
    {
        public delegate double[] DerivativeFunc(double t, double[] y);

        public static DerivativeFunc CompileSystem(string[] strFuncs)
        {
            int dim = strFuncs.Length;
            var compiledExprs = new AngouriMath.Core.FastExpression[dim];

            for (int i = 0; i < dim; i++)
            {
                Entity expr = strFuncs[i];
                compiledExprs[i] = expr.Compile("t", "x", "y", "z");
            }

            return (t, yVal) =>
            {
                double[] dy = new double[dim];
                double x = yVal.Length > 0 ? yVal[0] : 0;
                double y = yVal.Length > 1 ? yVal[1] : 0;
                double z = yVal.Length > 2 ? yVal[2] : 0;

                for (int i = 0; i < dim; i++)
                {
                    dy[i] = (double)compiledExprs[i].Call(t, x, y, z).Real;
                }
                return dy;
            };
        }
        // ==========================================================
        // 2. THUẬT TOÁN EULER HIỆN (EULER Explicit) - Dùng Lặp đơn
        // ==========================================================
        public static List<double[]> SolveEulerExplicit(DerivativeFunc f, double[] y0, double t0, double tend, double h)
        {
            var result = new List<double[]>();
            SaveState(result, t0, y0);

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;

            while (t < tend - 1e-9)
            {
                double[] dy = f(t, y);

                for (int i = 0; i < n; i++)
                {
                    y[i] = y[i] + h * dy[i];
                }

                t += h;

                SaveState(result, t, y);
            }
            return result;
        }

        // ==========================================================
        // 2. THUẬT TOÁN EULER ẨN (EULER IMPLICIT) - Dùng Lặp đơn
        // ==========================================================
        public static List<double[]> SolveEulerImplicit(DerivativeFunc f, double[] y0, double t0, double tend, double h, double epsilon = 1e-4)
        {
            var result = new List<double[]>();
            SaveState(result, t0, y0);

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;
            int maxIter = 1000;

            while (t < tend - 1e-9)
            {
                double t_next = t + h;

                double[] dy_curr = f(t, y);
                double[] y_star = new double[n];
                for (int i = 0; i < n; i++) y_star[i] = y[i] + h * dy_curr[i];

                for (int k = 0; k < maxIter; k++)
                {
                    double[] dy_next = f(t_next, y_star);

                    double[] y_temp = new double[n];
                    double error = 0;

                    for (int i = 0; i < n; i++)
                    {
                        y_temp[i] = y[i] + h * dy_next[i];

                        error += Math.Abs(y_temp[i] - y_star[i]);
                    }

                    Array.Copy(y_temp, y_star, n);

                    if (error < epsilon) break;
                }

                y = y_star;
                t = t_next;
                SaveState(result, t, y);
            }
            return result;
        }

        // ==========================================================
        // 3. THUẬT TOÁN HÌNH THANG (TRAPEZOIDAL) - Dùng Lặp đơn
        // ==========================================================
        public static List<double[]> SolveTrapezoidal(DerivativeFunc f, double[] y0, double t0, double tend, double h, double epsilon = 1e-4)
        {
            var result = new List<double[]>();
            SaveState(result, t0, y0);

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;
            int maxIter = 1000;

            while (t < tend - 1e-9)
            {
                double t_next = t + h;

                double[] dy_curr = f(t, y);

                double[] y_star = new double[n];
                for (int i = 0; i < n; i++) y_star[i] = y[i] + h * dy_curr[i];

                for (int k = 0; k < maxIter; k++)
                {
                    double[] dy_next = f(t_next, y_star);

                    double[] y_temp = new double[n];
                    double error = 0;

                    for (int i = 0; i < n; i++)
                    {
                        y_temp[i] = y[i] + (h / 2.0) * (dy_curr[i] + dy_next[i]);

                        error += Math.Abs(y_temp[i] - y_star[i]);
                    }

                    Array.Copy(y_temp, y_star, n);

                    if (error < epsilon) break;
                }

                y = y_star;
                t = t_next;
                SaveState(result, t, y);
            }
            return result;
        }

        // ==========================================================
        // 4. RUNGE-KUTTA 2 (HEUN) - Chuẩn k = h*f
        // k1 = h * f(t, y)
        // k2 = h * f(t + h, y + k1)
        // y_new = y + 1/2 * (k1 + k2)
        // ==========================================================
        public static List<double[]> SolveRK2(DerivativeFunc f, double[] y0, double t0, double tend, double h)
        {
            var result = new List<double[]>();
            SaveState(result, t0, y0);

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;

            while (t < tend - 1e-9)
            {
                // 1. Tính k1 = h * f(t, y)
                double[] f1 = f(t, y);
                double[] k1 = new double[n];
                for (int i = 0; i < n; i++) k1[i] = h * f1[i];

                // 2. Tính k2 = h * f(t + h, y + k1)
                double[] y_temp = new double[n];
                for (int i = 0; i < n; i++) y_temp[i] = y[i] + k1[i]; // Không cần nhân h vì k1 đã có h

                double[] f2 = f(t + h, y_temp);
                double[] k2 = new double[n];
                for (int i = 0; i < n; i++) k2[i] = h * f2[i];

                // 3. Cập nhật y: y + 0.5*(k1 + k2)
                for (int i = 0; i < n; i++)
                {
                    y[i] = y[i] + 0.5 * (k1[i] + k2[i]); // Không nhân h ở ngoài nữa
                }

                t += h;
                SaveState(result, t, y);
            }
            return result;
        }

        // ==========================================================
        // 5. RUNGE-KUTTA 3 (CLASSIC) - Chuẩn k = h*f
        // k1 = h * f(t, y)
        // k2 = h * f(t + h/2, y + k1/2)
        // k3 = h * f(t + h, y - k1 + 2k2)
        // y_new = y + 1/6 * (k1 + 4k2 + k3)
        // ==========================================================
        public static List<double[]> SolveRK3(DerivativeFunc f, double[] y0, double t0, double tend, double h)
        {
            var result = new List<double[]>();
            SaveState(result, t0, y0);

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;

            while (t < tend - 1e-9)
            {
                // k1
                double[] f1 = f(t, y);
                double[] k1 = new double[n];
                for (int i = 0; i < n; i++) k1[i] = h * f1[i];

                // k2 (dùng k1/2 để update y)
                double[] y_for_k2 = new double[n];
                for (int i = 0; i < n; i++) y_for_k2[i] = y[i] + 0.5 * k1[i];

                double[] f2 = f(t + 0.5 * h, y_for_k2);
                double[] k2 = new double[n];
                for (int i = 0; i < n; i++) k2[i] = h * f2[i];

                // k3 (dùng -k1 + 2k2 để update y)
                double[] y_for_k3 = new double[n];
                for (int i = 0; i < n; i++) y_for_k3[i] = y[i] - k1[i] + 2 * k2[i];

                double[] f3 = f(t + h, y_for_k3);
                double[] k3 = new double[n];
                for (int i = 0; i < n; i++) k3[i] = h * f3[i];

                // Update y
                for (int i = 0; i < n; i++)
                {
                    y[i] = y[i] + (1.0 / 6.0) * (k1[i] + 4 * k2[i] + k3[i]);
                }

                t += h;
                SaveState(result, t, y);
            }
            return result;
        }

        // ==========================================================
        // 6. RUNGE-KUTTA 4 (CLASSIC) - Chuẩn k = h*f
        // k1 = h * f(t, y)
        // k2 = h * f(t + h/2, y + k1/2)
        // k3 = h * f(t + h/2, y + k2/2)
        // k4 = h * f(t + h, y + k3)
        // y_new = y + 1/6 * (k1 + 2k2 + 2k3 + k4)
        // ==========================================================
        public static List<double[]> SolveRK4(DerivativeFunc f, double[] y0, double t0, double tend, double h)
        {
            var result = new List<double[]>();
            SaveState(result, t0, y0);

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;

            while (t < tend - 1e-9)
            {
                // k1
                double[] f1 = f(t, y);
                double[] k1 = new double[n];
                for (int i = 0; i < n; i++) k1[i] = h * f1[i];

                // k2 (y + k1/2)
                double[] y_for_k2 = new double[n];
                for (int i = 0; i < n; i++) y_for_k2[i] = y[i] + 0.5 * k1[i];

                double[] f2 = f(t + 0.5 * h, y_for_k2);
                double[] k2 = new double[n];
                for (int i = 0; i < n; i++) k2[i] = h * f2[i];

                // k3 (y + k2/2)
                double[] y_for_k3 = new double[n];
                for (int i = 0; i < n; i++) y_for_k3[i] = y[i] + 0.5 * k2[i];

                double[] f3 = f(t + 0.5 * h, y_for_k3);
                double[] k3 = new double[n];
                for (int i = 0; i < n; i++) k3[i] = h * f3[i];

                // k4 (y + k3)
                double[] y_for_k4 = new double[n];
                for (int i = 0; i < n; i++) y_for_k4[i] = y[i] + k3[i];

                double[] f4 = f(t + h, y_for_k4);
                double[] k4 = new double[n];
                for (int i = 0; i < n; i++) k4[i] = h * f4[i];

                // Update y
                for (int i = 0; i < n; i++)
                {
                    y[i] = y[i] + (1.0 / 6.0) * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
                }

                t += h;
                SaveState(result, t, y);
            }
            return result;
        }

        private static void SaveState(List<double[]> list, double t, double[] y)
        {
            double[] row = new double[y.Length + 1];
            row[0] = t;
            Array.Copy(y, 0, row, 1, y.Length);
            list.Add(row);
        }
    }
}