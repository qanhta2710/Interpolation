using AngouriMath;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpolation
{
    public class ODESolver
    {
        public enum RK2ParamType { Alpha2, R1, R2, Beta11 }
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
                for (int i = 0; i < n; i++) y_temp[i] = y[i] + k1[i]; 

                double[] f2 = f(t + h, y_temp);
                double[] k2 = new double[n];
                for (int i = 0; i < n; i++) k2[i] = h * f2[i];

                // 3. Cập nhật y: y + 0.5*(k1 + k2)
                for (int i = 0; i < n; i++)
                {
                    y[i] = y[i] + 0.5 * (k1[i] + k2[i]); 
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
                double[] f1 = f(t, y);
                double[] k1 = new double[n];
                for (int i = 0; i < n; i++) k1[i] = h * f1[i];

                double[] y_for_k2 = new double[n];
                for (int i = 0; i < n; i++) y_for_k2[i] = y[i] + 0.5 * k1[i];

                double[] f2 = f(t + 0.5 * h, y_for_k2);
                double[] k2 = new double[n];
                for (int i = 0; i < n; i++) k2[i] = h * f2[i];

                double[] y_for_k3 = new double[n];
                for (int i = 0; i < n; i++) y_for_k3[i] = y[i] - k1[i] + 2 * k2[i];

                double[] f3 = f(t + h, y_for_k3);
                double[] k3 = new double[n];
                for (int i = 0; i < n; i++) k3[i] = h * f3[i];

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
                double[] f1 = f(t, y);
                double[] k1 = new double[n];
                for (int i = 0; i < n; i++) k1[i] = h * f1[i];
                
                double[] y_for_k2 = new double[n];
                for (int i = 0; i < n; i++) y_for_k2[i] = y[i] + 0.5 * k1[i];

                double[] f2 = f(t + 0.5 * h, y_for_k2);
                double[] k2 = new double[n];
                for (int i = 0; i < n; i++) k2[i] = h * f2[i];

                double[] y_for_k3 = new double[n];
                for (int i = 0; i < n; i++) y_for_k3[i] = y[i] + 0.5 * k2[i];

                double[] f3 = f(t + 0.5 * h, y_for_k3);
                double[] k3 = new double[n];
                for (int i = 0; i < n; i++) k3[i] = h * f3[i];

                double[] y_for_k4 = new double[n];
                for (int i = 0; i < n; i++) y_for_k4[i] = y[i] + k3[i];

                double[] f4 = f(t + h, y_for_k4);
                double[] k4 = new double[n];
                for (int i = 0; i < n; i++) k4[i] = h * f4[i];

                for (int i = 0; i < n; i++)
                {
                    y[i] = y[i] + (1.0 / 6.0) * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
                }

                t += h;
                SaveState(result, t, y);
            }
            return result;
        }
        public static List<double[]> SolveAdams(DerivativeFunc f, double[] y0, double t0, double tend, double h, int s, double epsilon)
        {
            var result = new List<double[]>();
            int n = y0.Length;

            // B1: Khởi tạo s điểm đầu bằng RK4
            double t_init_limit = t0 + (s - 1) * h;
            var initPoints = SolveRK4(f, y0, t0, t_init_limit, h);
            for (int i = 0; i < s && i < initPoints.Count; i++) result.Add(initPoints[i]);

            // Chuẩn bị History F
            List<double[]> f_history = new List<double[]>();
            foreach (var pt in result)
            {
                double t_val = pt[0];
                double[] y_val = new double[n];
                Array.Copy(pt, 1, y_val, 0, n);
                f_history.Add(f(t_val, y_val));
            }

            double[] beta = GetAdamsBashforthCoeffs(s);
            double[] gamma = GetAdamsMoultonCoeffs(s);
            double t = result.Last()[0];

            // B2: Vòng lặp chính
            while (t < tend - 1e-9)
            {
                double[] y_curr = new double[n];
                Array.Copy(result.Last(), 1, y_curr, 0, n);

                // Dự báo (AB)
                double[] y_pred = new double[n];
                for (int d = 0; d < n; d++)
                {
                    double sum = 0;
                    for (int j = 0; j < s; j++)
                    {
                        int hIdx = f_history.Count - 1 - j;
                        sum += beta[j] * f_history[hIdx][d];
                    }
                    y_pred[d] = y_curr[d] + h * sum;
                }

                double t_next = t + h;

                // Hiệu chỉnh (AM)
                double[] am_tail = new double[n];
                for (int d = 0; d < n; d++)
                {
                    for (int j = 1; j < s; j++)
                    {
                        int hIdx = f_history.Count - j;
                        am_tail[d] += gamma[j] * f_history[hIdx][d];
                    }
                }

                double[] y_corr = (double[])y_pred.Clone();
                for (int k = 0; k < 100; k++)
                {
                    double[] f_next = f(t_next, y_corr);
                    double[] y_new = new double[n];
                    double error = 0;
                    for (int d = 0; d < n; d++)
                    {
                        y_new[d] = y_curr[d] + h * (gamma[0] * f_next[d] + am_tail[d]);
                        error += Math.Abs(y_new[d] - y_corr[d]);
                    }
                    Array.Copy(y_new, y_corr, n);
                    if (error < epsilon) break;
                }

                double[] nextRow = new double[n + 1];
                nextRow[0] = t_next;
                Array.Copy(y_corr, 0, nextRow, 1, n);
                result.Add(nextRow);

                f_history.Add(f(t_next, y_corr));
                if (f_history.Count > s + 2) f_history.RemoveAt(0);

                t = t_next;
            }
            return result;
        }

        // RK2 Custom
        public static List<double[]> SolveCustomRK2(DerivativeFunc f, double[] y0, double t0, double tend, double h,
                                                    RK2ParamType paramType, double paramValue)
        {
            double r1 = 0, r2 = 0, alpha2 = 0, beta11 = 0;

            switch (paramType)
            {
                case RK2ParamType.Alpha2:
                    if (paramValue == 0) throw new ArgumentException("Alpha2 phải khác 0");
                    alpha2 = paramValue;
                    r2 = 1.0 / (2.0 * alpha2);
                    r1 = 1.0 - r2;
                    beta11 = alpha2;
                    break;

                case RK2ParamType.R2: 
                    if (paramValue == 0) throw new ArgumentException("r2 phải khác 0");
                    r2 = paramValue;
                    r1 = 1.0 - r2;
                    alpha2 = 1.0 / (2.0 * r2);
                    beta11 = alpha2;
                    break;

                case RK2ParamType.R1: 
                    r1 = paramValue;
                    r2 = 1.0 - r1;
                    if (r2 == 0) throw new ArgumentException("r1=1 dẫn đến r2=0, không tính được Alpha.");
                    alpha2 = 1.0 / (2.0 * r2);
                    beta11 = alpha2;
                    break;
                case RK2ParamType.Beta11:
                    if (paramValue == 0) throw new ArgumentException("Beta11 phải khác 0");
                    beta11 = paramValue;
                    alpha2 = beta11;
                    r2 = 1.0 / (2.0 * alpha2);
                    r1 = 1.0 - r2;
                    break;
            }

            // --- Thực hiện giải ---
            var result = new List<double[]>();
            SaveState(result, t0, y0);

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;

            while (t < tend - 1e-9)
            {
                // k1 = h * f(t, y)
                double[] f1 = f(t, y);
                double[] k1 = new double[n];
                for (int i = 0; i < n; i++) k1[i] = h * f1[i];

                // k2 = h * f(t + alpha2*h, y + beta11*k1)
                double[] y_temp = new double[n];
                for (int i = 0; i < n; i++) y_temp[i] = y[i] + beta11 * k1[i];

                double[] f2 = f(t + alpha2 * h, y_temp);
                double[] k2 = new double[n];
                for (int i = 0; i < n; i++) k2[i] = h * f2[i];

                // y_new = y + r1*k1 + r2*k2
                for (int i = 0; i < n; i++)
                {
                    y[i] = y[i] + (r1 * k1[i] + r2 * k2[i]);
                }

                t += h;
                SaveState(result, t, y);
            }

            return result;
        }
        // RK3 Custom
        public static List<double[]> SolveCustomRK3(DerivativeFunc f, double[] y0, double t0, double tend, double h,
                                            double alpha2, double alpha3)
        {
            if (Math.Abs(alpha2 - alpha3) < 1e-9) throw new ArgumentException("Alpha2 và Alpha3 phải khác nhau.");
            if (alpha2 == 0 || alpha3 == 0) throw new ArgumentException("Alpha phải khác 0.");

            double r2 = (3 * alpha3 - 2) / (6 * alpha2 * (alpha3 - alpha2));

            double r3 = (2 - 3 * alpha2) / (6 * alpha3 * (alpha3 - alpha2));

            double r1 = 1 - r2 - r3;

            double beta11 = alpha2; 

            if (r3 == 0) throw new ArgumentException("Bộ tham số này khiến r3=0, không tính được beta22.");
            double beta22 = 1.0 / (6 * r3 * alpha2);

            double beta21 = alpha3 - beta22;

            var result = new List<double[]>();
            SaveState(result, t0, y0);
            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;

            while (t < tend - 1e-9)
            {
                // k1 = h * f(t, y)
                double[] f1 = f(t, y);
                double[] k1 = new double[n];
                for (int i = 0; i < n; i++) k1[i] = h * f1[i];

                // k2 = h * f(t + alpha2*h, y + beta11*k1)
                double[] y_k2 = new double[n];
                for (int i = 0; i < n; i++) y_k2[i] = y[i] + beta11 * k1[i];

                double[] f2 = f(t + alpha2 * h, y_k2);
                double[] k2 = new double[n];
                for (int i = 0; i < n; i++) k2[i] = h * f2[i];

                // k3 = h * f(t + alpha3*h, y + beta21*k1 + beta22*k2)
                double[] y_k3 = new double[n];
                for (int i = 0; i < n; i++) y_k3[i] = y[i] + beta21 * k1[i] + beta22 * k2[i];

                double[] f3 = f(t + alpha3 * h, y_k3);
                double[] k3 = new double[n];
                for (int i = 0; i < n; i++) k3[i] = h * f3[i];

                // Update y
                for (int i = 0; i < n; i++)
                {
                    y[i] = y[i] + (r1 * k1[i] + r2 * k2[i] + r3 * k3[i]);
                }

                t += h;
                SaveState(result, t, y);
            }
            return result;
        }
        public static double[] GetAdamsBashforthCoeffs(int s)
        {
            switch (s)
            {
                case 2: return new double[] { 1.5, -0.5 };
                case 3: return new double[] { 23.0 / 12.0, -16.0 / 12.0, 5.0 / 12.0 };
                case 4: return new double[] { 55.0 / 24.0, -59.0 / 24.0, 37.0 / 24.0, -9.0 / 24.0 };
                case 5: return new double[] { 1901.0 / 720.0, -2774.0 / 720.0, 2616.0 / 720.0, -1274.0 / 720.0, 251.0 / 720.0 };
                default: throw new ArgumentException("S=" + s);
            }
        }

        public static double[] GetAdamsMoultonCoeffs(int s)
        {
            switch (s)
            {
                case 2: return new double[] { 5.0 / 12.0, 8.0 / 12.0, -1.0 / 12.0 };
                case 3: return new double[] { 9.0 / 24.0, 19.0 / 24.0, -5.0 / 24.0, 1.0 / 24.0 };
                case 4: return new double[] { 251.0 / 720.0, 646.0 / 720.0, -264.0 / 720.0, 106.0 / 720.0, -19.0 / 720.0 };
                case 5: return new double[] { 475.0 / 1440.0, 1427.0 / 1440.0, -798.0 / 1440.0, 482.0 / 1440.0, -173.0 / 1440.0, 27.0 / 1440.0 };
                default: throw new ArgumentException("S=" + s);
            }
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