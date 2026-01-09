using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpolation.Methods
{
    public class EigenResult
    {
        public List<double> Eigenvalues { get; set; }
        public List<double[]> Eigenfunctions { get; set; }
        public double[] X { get; set; }
        public string Log { get; set; }
    }

    public class EigenvalueSolver
    {
        public static EigenResult SolveSturmLiouville(
            BoundaryValueSolver.FuncX p,
            BoundaryValueSolver.FuncX q,
            BoundaryValueSolver.FuncX r,
            double a, double b, double h)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== BÀI TOÁN TRỊ RIÊNG (BIÊN DIRICHLET ĐỒNG NHẤT) ===");
            sb.AppendLine("Phương trình: [p(x)u']' - q(x)u = λ·r(x)·u");
            sb.AppendLine("Điều kiện biên: u(a) = u(b) = 0");
            sb.AppendLine($"Lưới: [{a}, {b}], Bước h = {h}");
            sb.AppendLine($"Số điểm lưới N = (b - a) / h = {(b - a) / h}");

            int N = (int)Math.Round((b - a) / h);
            double[] x = new double[N + 1];
            for (int i = 0; i <= N; i++) x[i] = a + i * h;

            // 1. CÔNG THỨC & HỆ SỐ
            sb.AppendLine("\n1. RỜI RẠC HÓA & HỆ SỐ:");
            sb.AppendLine("   Sử dụng sai phân trung tâm tại điểm x_i:");
            sb.AppendLine("   (p(x_i - h/2)/h^2)*u_{i-1} - ((p(x_i - h/2) + p(x_i + h/2))/h^2 + q(x_i))*u_i + (p(x_i + h/2)/h^2)*u_{i+1} = -λ * r(x_i) * u_i");
            sb.AppendLine();
            sb.AppendLine("   Đặt các hệ số:");
            sb.AppendLine("     A_i = p(x_i - h/2) / h^2");
            sb.AppendLine("     C_i = p(x_i + h/2) / h^2");
            sb.AppendLine("     B_i = -(A_i + C_i + q(x_i))");
            sb.AppendLine("     D_i = -r(x_i)");
            sb.AppendLine();
            sb.AppendLine("   Hệ phương trình tuyến tính:");
            sb.AppendLine("     A_i·u_{i-1} + B_i·u_i + C_i·u_{i+1} = λ · D_i · u_i");
            sb.AppendLine("   Trong đó: u = [u(x_1), u(x_2)...]^T");

            sb.AppendLine("\n   BẢNG GIÁ TRỊ CÁC HỆ SỐ:");
            sb.AppendLine(string.Format("{0,5} | {1,15} {2,15} {3,15} | {4,15}",
                "Idx", "A (u_{i-1})", "B (u_i)", "C (u_{i+1})", "D (r_i)"));
            sb.AppendLine(new string('-', 85));

            int innerSize = N - 1;
            double[,] M = new double[innerSize, innerSize];
            double[,] D_inv = new double[innerSize, innerSize];

            for (int i = 0; i < innerSize; i++)
            {
                int realIdx = i + 1; 

                double p_plus = p(x[realIdx] + h / 2.0);  
                double p_minus = p(x[realIdx] - h / 2.0); 
                double q_val = q(x[realIdx]);
                double r_val = r(x[realIdx]);

                double val_A = p_minus / (h * h);
                double val_C = p_plus / (h * h);
                double val_B = -(val_A + val_C + q_val);

                if (i > 0) M[i, i - 1] = val_A;
                M[i, i] = val_B;
                if (i < innerSize - 1) M[i, i + 1] = val_C;

                double d_val = -r_val;
                if (Math.Abs(d_val) < 1e-12) d_val = -1e-12;
                D_inv[i, i] = 1.0 / d_val;

                if (i < 3 || i >= innerSize - 3)
                {
                    sb.AppendLine(string.Format("{0,5} | {1,15:F4} {2,15:F4} {3,15:F4} | {4,15:F4}",
                        realIdx, val_A, val_B, val_C, r_val));
                }
                else if (i == 3)
                {
                    sb.AppendLine("  ...         ...             ...             ...              ...");
                }
            }
            sb.AppendLine();

            // 2. HIỂN THỊ MA TRẬN M
            sb.AppendLine("2. MA TRẬN M:");
            sb.AppendLine("   M = [A_i, B_i, C_i]");
            PrintMatrixSmart(sb, M);

            // 3. TÍNH MA TRẬN PHI
            double[,] Phi = MultiplyMatrices(D_inv, M);

            sb.AppendLine("\n3. MA TRẬN Φ:");
            sb.AppendLine("   Ta có: M·u = λ·(-r)·u  =>  (-1/r)·M·u = λ·u");
            sb.AppendLine("   Ma trận Φ = D⁻¹ · M");
            PrintMatrixSmart(sb, Phi);

            // 4. GIẢI TRỊ RIÊNG
            List<double> eigenvals;
            List<double[]> eigenvecsInner;
            SolveEigenSystemQR(Phi, out eigenvals, out eigenvecsInner);

            List<double[]> fullEigenfunctions = new List<double[]>();
            for (int k = 0; k < eigenvecsInner.Count; k++)
            {
                double[] u_full = new double[N + 1];
                u_full[0] = 0; u_full[N] = 0;
                for (int i = 0; i < innerSize; i++) u_full[i + 1] = eigenvecsInner[k][i];
                NormalizeVector(u_full);
                fullEigenfunctions.Add(u_full);
            }

            return new EigenResult
            {
                Eigenvalues = eigenvals,
                Eigenfunctions = fullEigenfunctions,
                X = x,
                Log = sb.ToString()
            };
        }

        private static void PrintMatrixSmart(StringBuilder sb, double[,] mat)
        {
            int n = mat.GetLength(0);
            int m = mat.GetLength(1);
            int showCount = 4;

            sb.Append("      ");
            for (int j = 0; j < m; j++)
            {
                if (j < showCount || j >= m - showCount)
                    sb.Append($"{j,10} ");
                else if (j == showCount)
                    sb.Append("    ...    ");
            }
            sb.AppendLine();
            sb.AppendLine(new string('-', 85));

            for (int i = 0; i < n; i++)
            {
                if (i == showCount && n > 2 * showCount)
                {
                    sb.AppendLine("  ...      ...        ...        ...         ...      ...");
                    i = n - showCount - 1;
                    continue;
                }

                sb.Append($"{i,4} |");
                for (int j = 0; j < m; j++)
                {
                    if (j == showCount && m > 2 * showCount)
                    {
                        sb.Append("    ...    ");
                        j = m - showCount - 1;
                        continue;
                    }
                    sb.Append($"{mat[i, j],10:F4} ");
                }
                sb.AppendLine("|");
            }
        }

        private static void SolveEigenSystemQR(double[,] A, out List<double> lambdas, out List<double[]> eigenvectors)
        {
            int n = A.GetLength(0);
            double[,] Ak = (double[,])A.Clone();
            double[,] U = IdentityMatrix(n);
            int maxIter = 1000;

            for (int iter = 0; iter < maxIter; iter++)
            {
                QRDecomposition(Ak, out double[,] Q, out double[,] R);
                Ak = MultiplyMatrices(R, Q);
                U = MultiplyMatrices(U, Q);
            }

            lambdas = new List<double>();
            eigenvectors = new List<double[]>();

            for (int i = 0; i < n; i++)
            {
                lambdas.Add(Ak[i, i]);
                double[] vec = new double[n];
                for (int r = 0; r < n; r++) vec[r] = U[r, i];
                eigenvectors.Add(vec);
            }
        }

        private static void QRDecomposition(double[,] A, out double[,] Q, out double[,] R)
        {
            int n = A.GetLength(0);
            Q = new double[n, n];
            R = new double[n, n];

            for (int j = 0; j < n; j++) for (int i = 0; i < n; i++) Q[i, j] = A[i, j];

            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < j; k++)
                {
                    double dot = 0;
                    for (int i = 0; i < n; i++) dot += Q[i, k] * A[i, j];
                    R[k, j] = dot;
                    for (int i = 0; i < n; i++) Q[i, j] -= R[k, j] * Q[i, k];
                }
                double norm = 0;
                for (int i = 0; i < n; i++) norm += Q[i, j] * Q[i, j];
                norm = Math.Sqrt(norm);
                R[j, j] = norm;
                if (norm > 1e-12) for (int i = 0; i < n; i++) Q[i, j] /= norm;
            }
        }

        private static double[,] MultiplyMatrices(double[,] A, double[,] B)
        {
            int n = A.GetLength(0);
            double[,] C = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int k = 0; k < n; k++)
                {
                    double temp = A[i, k];
                    for (int j = 0; j < n; j++) C[i, j] += temp * B[k, j];
                }
            return C;
        }

        private static double[,] IdentityMatrix(int n)
        {
            double[,] I = new double[n, n];
            for (int i = 0; i < n; i++) I[i, i] = 1.0;
            return I;
        }

        private static void NormalizeVector(double[] v)
        {
            double max = v.Select(Math.Abs).Max();
            if (max > 1e-12) for (int i = 0; i < v.Length; i++) v[i] /= max;
        }
    }
}