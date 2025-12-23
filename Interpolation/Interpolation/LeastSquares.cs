using AngouriMath;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public abstract class LeastSquaresBase
    {
        public double[] XData { get; protected set; }
        public double[] YData { get; protected set; }
        public double MSE { get; protected set; }
        public double[,] ThetaMatrix { get; protected set; }
        public double[,] MMatrix { get; protected set; }

        protected LeastSquaresBase(double[] x, double[] y)
        {
            if (x == null || y == null || x.Length == 0 || x.Length != y.Length)
                throw new ArgumentException("Dữ liệu đầu vào không hợp lệ");

            XData = x;
            YData = y;
        }

        public abstract void Solve();
        public abstract void DisplayResults(RichTextBox rtb);

        protected void ValidateDataSigns()
        {
            bool allPositive = Array.TrueForAll(YData, v => v > 0);
            bool allNegative = Array.TrueForAll(YData, v => v < 0);

            if (!allPositive && !allNegative)
                throw new ArgumentException("Dữ liệu y chứa cả giá trị dương và âm, người dùng cần tự xử lý dữ liệu để chỉ chứa giá trị dương hoặc âm");
        }

        protected double EvaluatePhi(string phiExpression, double xValue)
        {
            try
            {
                Entity expr = phiExpression;
                var compiled = expr.Compile("x");
                var result = compiled.Call(xValue);
                return (double)result.Real;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Lỗi đánh giá hàm '{phiExpression}' tại x={xValue}: {ex.Message}");
            }
        }
    }

    public class LinearLeastSquares : LeastSquaresBase
    {
        public string[] PhiExpressions { get; private set; }
        public double[] Coefficients { get; private set; }
        public double[] BVector { get; private set; }
        public double[] YPredicted { get; private set; }
        public string Equation { get; private set; }

        public LinearLeastSquares(double[] x, double[] y, string[] phiExpressions)
            : base(x, y)
        {
            if (phiExpressions == null || phiExpressions.Length == 0)
                throw new ArgumentException("Phải cung cấp ít nhất 1 hàm cơ sở");

            PhiExpressions = phiExpressions;
        }

        public override void Solve()
        {
            int n = XData.Length;
            int m = PhiExpressions.Length;

            // Xây dựng ma trận Theta sử dụng AngouriMath
            ThetaMatrix = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    ThetaMatrix[i, j] = EvaluatePhi(PhiExpressions[j], XData[i]);
                }
            }

            var Theta = Matrix<double>.Build.DenseOfArray(ThetaMatrix);
            var Y = Vector<double>.Build.Dense(YData);

            var M = Theta.TransposeThisAndMultiply(Theta);
            var b = Theta.TransposeThisAndMultiply(Y);

            MMatrix = M.ToArray();
            BVector = b.ToArray();

            var coeffs = M.Solve(b);
            Coefficients = coeffs.ToArray();

            var yPred = Theta * coeffs;
            YPredicted = yPred.ToArray();

            MSE = Math.Sqrt((Y - yPred).PointwisePower(2).Sum() / n);

            Equation = "y ≈ " + string.Join(" + ",
                PhiExpressions.Select((phi, j) => $"{Coefficients[j]:F6}*({phi})"));
        }

        public override void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ PHƯƠNG PHÁP BÌNH PHƯƠNG TỐI THIỂU - DẠNG TUYẾN TÍNH");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            sb.AppendLine("TẬP HÀM CƠ SỞ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int j = 0; j < PhiExpressions.Length; j++)
            {
                sb.AppendLine($"  φ{j + 1}(x) = {PhiExpressions[j]}");
            }
            sb.AppendLine();

            sb.AppendLine("MA TRẬN Θ (THETA):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("   i |");
            for (int j = 0; j < PhiExpressions.Length; j++)
            {
                sb.Append($"    φ{j + 1}(xi)    |");
            }
            sb.AppendLine();
            sb.AppendLine(new string('─', 10 + PhiExpressions.Length * 16));

            for (int i = 0; i < ThetaMatrix.GetLength(0); i++)
            {
                sb.Append($"  {i + 1,2} |");
                for (int j = 0; j < ThetaMatrix.GetLength(1); j++)
                {
                    sb.Append($" {ThetaMatrix[i, j],12:F6} |");
                }
                sb.AppendLine();
            }
            sb.AppendLine();

            sb.AppendLine("MA TRẬN M = Θ^T × Θ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < MMatrix.GetLength(0); i++)
            {
                sb.Append("  [");
                for (int j = 0; j < MMatrix.GetLength(1); j++)
                {
                    sb.Append($" {MMatrix[i, j],12:F6}");
                }
                sb.AppendLine(" ]");
            }
            sb.AppendLine();

            sb.AppendLine("VECTOR b = Θ^T × y:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("  b = [");
            for (int i = 0; i < BVector.Length; i++)
            {
                sb.Append($" {BVector[i],12:F6}");
                if (i < BVector.Length - 1) sb.Append(",");
            }
            sb.AppendLine(" ]^T\n");

            sb.AppendLine("HỆ SỐ TÌM ĐƯỢC (Giải M × a = b):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < Coefficients.Length; i++)
            {
                sb.AppendLine($"  a{i + 1} = {Coefficients[i]:F8}");
            }
            sb.AppendLine();

            sb.AppendLine("PHƯƠNG TRÌNH XẤP XỈ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  {Equation}");
            sb.AppendLine();

            sb.AppendLine("GIÁ TRỊ Y DỰ ĐOÁN:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < XData.Length; i++)
            {
                sb.AppendLine($"  x = {XData[i],10:F6}  →  y = {YPredicted[i],12:F6}");
            }
            sb.AppendLine();

            sb.AppendLine("SAI SỐ TRUNG BÌNH PHƯƠNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  MSE = {MSE:F8}");
            sb.AppendLine();

            rtb.AppendText(sb.ToString());
        }
    }

    public class PowerLawLeastSquares : LeastSquaresBase
    {
        public double A { get; private set; }
        public double P { get; private set; }
        public double[] RVector { get; private set; }

        public PowerLawLeastSquares(double[] x, double[] y)
            : base(x, y)
        {
            ValidateDataSigns();
        }

        public override void Solve()
        {
            int n = XData.Length;

            bool allNegative = Array.TrueForAll(YData, v => v < 0);

            double[] X = XData.Select(xi => Math.Log(xi)).ToArray();
            double[] Y = YData.Select(yi => Math.Log(Math.Abs(yi))).ToArray();

            ThetaMatrix = new double[n, 2];
            for (int i = 0; i < n; i++)
            {
                ThetaMatrix[i, 0] = 1.0;
                ThetaMatrix[i, 1] = X[i];
            }

            var Theta = Matrix<double>.Build.DenseOfArray(ThetaMatrix);
            var Yvec = Vector<double>.Build.Dense(Y);

            var M = Theta.TransposeThisAndMultiply(Theta);
            var r = Theta.TransposeThisAndMultiply(Yvec);

            MMatrix = M.ToArray();
            RVector = r.ToArray();

            var coeffs = M.Solve(r);

            A = Math.Exp(coeffs[0]);
            P = coeffs[1];

            if (allNegative)
                A = -A;

            var yPred = Theta * coeffs;
            MSE = Math.Sqrt((Yvec - yPred).PointwisePower(2).Sum() / n);
        }

        public override void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ PHƯƠNG PHÁP BÌNH PHƯƠNG TỐI THIỂU - DẠNG LUỸ THỪA");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            sb.AppendLine("MÔ HÌNH:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("  y = a × x^p");
            sb.AppendLine("  Biến đổi logarit: ln(y) = ln(a) + p × ln(x)");
            sb.AppendLine("  Đặt: Y = ln(y), X = ln(x), A₀ = ln(a), A₁ = p");
            sb.AppendLine("  => Y = A₀ + A₁ × X\n");

            sb.AppendLine("MA TRẬN Θ SAU KHI BIẾN ĐỔI:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("   i |      1       |    ln(xi)    |");
            sb.AppendLine(new string('─', 40));

            for (int i = 0; i < ThetaMatrix.GetLength(0); i++)
            {
                sb.Append($"  {i + 1,2} |");
                for (int j = 0; j < ThetaMatrix.GetLength(1); j++)
                {
                    sb.Append($" {ThetaMatrix[i, j],12:F6} |");
                }
                sb.AppendLine();
            }
            sb.AppendLine();

            sb.AppendLine("DỮ LIỆU GỐC VÀ SAU BIẾN ĐỔI:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("   i |      x       |      y       |    ln(x)     |    ln(|y|)   |");
            sb.AppendLine(new string('─', 70));

            for (int i = 0; i < XData.Length; i++)
            {
                sb.AppendLine($"  {i + 1,2} | {XData[i],12:F6} | {YData[i],12:F6} | {Math.Log(XData[i]),12:F6} | {Math.Log(Math.Abs(YData[i])),12:F6} |");
            }
            sb.AppendLine();

            sb.AppendLine("MA TRẬN M = Θ^T × Θ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < MMatrix.GetLength(0); i++)
            {
                sb.Append("  [");
                for (int j = 0; j < MMatrix.GetLength(1); j++)
                {
                    sb.Append($" {MMatrix[i, j],12:F6}");
                }
                sb.AppendLine(" ]");
            }
            sb.AppendLine();

            sb.AppendLine("VECTOR r = Θ^T × Y:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("  r = [");
            for (int i = 0; i < RVector.Length; i++)
            {
                sb.Append($" {RVector[i],12:F6}");
                if (i < RVector.Length - 1) sb.Append(",");
            }
            sb.AppendLine(" ]^T\n");

            sb.AppendLine("HỆ SỐ TÌM ĐƯỢC:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  A₀ = ln(|a|) = {Math.Log(Math.Abs(A)):F8}");
            sb.AppendLine($"  A₁ = p       = {P:F8}");
            sb.AppendLine();

            sb.AppendLine("THAM SỐ CUỐI CÙNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  a = {A:F8}");
            sb.AppendLine($"  p = {P:F8}");
            sb.AppendLine();

            sb.AppendLine("PHƯƠNG TRÌNH XẤP XỈ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  y ≈ {A:F8} × x^({P:F8})");
            sb.AppendLine();

            sb.AppendLine("SAI SỐ TRUNG BÌNH PHƯƠNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  MSE = {MSE:F8}");
            sb.AppendLine("  (Tính trên không gian logarit: ln(y))");
            sb.AppendLine();

            rtb.AppendText(sb.ToString());
        }
    }

    public class ExponentialLeastSquares : LeastSquaresBase
    {
        public string[] PhiExpressions { get; private set; }
        public double A { get; private set; }
        public double[] BCoefficients { get; private set; }
        public double[] RVector { get; private set; }

        public ExponentialLeastSquares(double[] x, double[] y, string[] phiExpressions)
            : base(x, y)
        {
            if (phiExpressions == null || phiExpressions.Length == 0)
                throw new ArgumentException("Phải cung cấp ít nhất 1 hàm cơ sở");

            PhiExpressions = phiExpressions;
            ValidateDataSigns();
        }

        public override void Solve()
        {
            int n = XData.Length;
            int m = PhiExpressions.Length;

            bool allNegative = Array.TrueForAll(YData, v => v < 0);

            double[] Y = YData.Select(yi => Math.Log(Math.Abs(yi))).ToArray();

            // Xây dựng ma trận Theta sử dụng AngouriMath
            ThetaMatrix = new double[n, m + 1];
            for (int i = 0; i < n; i++)
            {
                ThetaMatrix[i, 0] = 1.0;
                for (int j = 0; j < m; j++)
                {
                    ThetaMatrix[i, j + 1] = EvaluatePhi(PhiExpressions[j], XData[i]);
                }
            }

            var Theta = Matrix<double>.Build.DenseOfArray(ThetaMatrix);
            var Yvec = Vector<double>.Build.Dense(Y);

            var M = Theta.TransposeThisAndMultiply(Theta);
            var r = Theta.TransposeThisAndMultiply(Yvec);

            MMatrix = M.ToArray();
            RVector = r.ToArray();

            var C = M.Solve(r).ToArray();

            A = Math.Exp(C[0]);
            if (allNegative) A = -A;

            BCoefficients = C.Skip(1).ToArray();

            var yPred = Theta * Vector<double>.Build.Dense(C);
            MSE = Math.Sqrt((Yvec - yPred).PointwisePower(2).Sum() / n);
        }

        public override void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ PHƯƠNG PHÁP BÌNH PHƯƠNG TỐI THIỂU - DẠNG MŨ");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            sb.AppendLine("MÔ HÌNH:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("  y = a × e^(");
            var expTerms = new List<string>();
            for (int j = 0; j < PhiExpressions.Length; j++)
            {
                expTerms.Add($"b{j + 1}×{PhiExpressions[j]}");
            }
            sb.Append(string.Join(" + ", expTerms));
            sb.AppendLine(")");

            sb.Append("  Biến đổi logarit: ln(|y|) = ln(a) + ");
            sb.AppendLine(string.Join(" + ", expTerms));
            sb.AppendLine("  Đặt: Y = ln(|y|), C₀ = ln(|a|), Cᵢ = bᵢ");
            sb.AppendLine();

            sb.AppendLine("MA TRẬN Θ SAU KHI BIẾN ĐỔI:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("   i |      1       |");
            for (int j = 0; j < PhiExpressions.Length; j++)
            {
                sb.Append($"    φ{j + 1}(xi)    |");
            }
            sb.AppendLine();
            sb.AppendLine(new string('─', 20 + PhiExpressions.Length * 16));

            for (int i = 0; i < ThetaMatrix.GetLength(0); i++)
            {
                sb.Append($"  {i + 1,2} |");
                for (int j = 0; j < ThetaMatrix.GetLength(1); j++)
                {
                    sb.Append($" {ThetaMatrix[i, j],12:F6} |");
                }
                sb.AppendLine();
            }
            sb.AppendLine();

            sb.AppendLine("DỮ LIỆU GỐC VÀ SAU BIẾN ĐỔI:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("   i |      x       |      y       |");
            for (int j = 0; j < PhiExpressions.Length; j++)
            {
                sb.Append($"   φ{j + 1}(x)     |");
            }
            sb.AppendLine("   ln(|y|)    |");
            sb.AppendLine(new string('─', 40 + PhiExpressions.Length * 14 + 14));

            for (int i = 0; i < XData.Length; i++)
            {
                sb.Append($"  {i + 1,2} | {XData[i],12:F6} | {YData[i],12:F6} |");
                for (int j = 0; j < PhiExpressions.Length; j++)
                {
                    sb.Append($" {ThetaMatrix[i, j + 1],12:F6} |");
                }
                sb.AppendLine($" {Math.Log(Math.Abs(YData[i])),12:F6} |");
            }
            sb.AppendLine();

            sb.AppendLine("MA TRẬN M = Θ^T × Θ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < MMatrix.GetLength(0); i++)
            {
                sb.Append("  [");
                for (int j = 0; j < MMatrix.GetLength(1); j++)
                {
                    sb.Append($" {MMatrix[i, j],12:F6}");
                }
                sb.AppendLine(" ]");
            }
            sb.AppendLine();

            sb.AppendLine("VECTOR r = Θ^T × Y:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("  r = [");
            for (int i = 0; i < RVector.Length; i++)
            {
                sb.Append($" {RVector[i],12:F6}");
                if (i < RVector.Length - 1) sb.Append(",");
            }
            sb.AppendLine(" ]^T\n");

            sb.AppendLine("HỆ SỐ TÌM ĐƯỢC:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  C₀ = ln(|a|) = {Math.Log(Math.Abs(A)):F8}");
            for (int j = 0; j < BCoefficients.Length; j++)
            {
                sb.AppendLine($"  C{j + 1} = b{j + 1}     = {BCoefficients[j]:F8}");
            }
            sb.AppendLine();

            sb.AppendLine("THAM SỐ CUỐI CÙNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  a  = {A:F8}");
            for (int j = 0; j < BCoefficients.Length; j++)
            {
                sb.AppendLine($"  b{j + 1} = {BCoefficients[j]:F8}");
            }
            sb.AppendLine();

            sb.AppendLine("PHƯƠNG TRÌNH XẤP XỈ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append($"  y ≈ {A:F8} × e^(");
            var finalTerms = new List<string>();
            for (int j = 0; j < PhiExpressions.Length; j++)
            {
                string sign = BCoefficients[j] >= 0 ? "+" : "";
                finalTerms.Add($"{sign}{BCoefficients[j]:F8}×({PhiExpressions[j]})");
            }
            sb.Append(string.Join(" ", finalTerms));
            sb.AppendLine(")");
            sb.AppendLine();

            sb.AppendLine("SAI SỐ TRUNG BÌNH PHƯƠNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  MSE = {MSE:F8}");
            sb.AppendLine("  (Tính trên không gian logarit: ln(y))");
            sb.AppendLine();

            rtb.AppendText(sb.ToString());
        }
    }
}