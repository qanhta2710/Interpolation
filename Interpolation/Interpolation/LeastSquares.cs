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
        public double YShift { get; protected set; } = 0;
        public bool IsShiftedDataNegative { get; protected set; } = false;
        public double XShift { get; protected set; } = 0;
        public bool IsXShifted { get; protected set; } = false;

        protected LeastSquaresBase(double[] x, double[] y)
        {
            if (x == null || y == null || x.Length == 0 || x.Length != y.Length)
                throw new ArgumentException("Dữ liệu đầu vào không hợp lệ");

            XData = x;
            YData = y;
        }
        protected void CalculateYShift()
        {
            double minY = YData.Min();
            double maxY = YData.Max();

            bool hasPositive = YData.Any(y => y > 0);
            bool hasNegative = YData.Any(y => y < 0);

            bool needsShift = (hasPositive && hasNegative) || YData.Any(y => y == 0);

            if (!needsShift)
            {
                YShift = 0;
                IsShiftedDataNegative = (maxY < 0);
                return;
            }

            double shiftUpAmount = Math.Abs(minY) + 1.0;
            double shiftDownAmount = -(Math.Abs(maxY) + 1.0);

            if (Math.Abs(shiftDownAmount) < Math.Abs(shiftUpAmount))
            {
                YShift = shiftDownAmount;
                IsShiftedDataNegative = true;
            }
            else
            {
                YShift = shiftUpAmount;
                IsShiftedDataNegative = false;
            }
        }
        protected void CalculateXShift()
        {
            double minX = XData.Min();
            if (minX <= 0)
            {
                XShift = Math.Abs(minX) + 1.0;
                IsXShifted = true;
            }
            else
            {
                XShift = 0;
                IsXShifted = false;
            }
        }
        public abstract void Solve();
        public abstract void DisplayResults(RichTextBox rtb);

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
            sb.AppendLine($"  MSE = sqrt( Σ( ln|y| - ln|y_pred| )² = {MSE:F8}");
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
            : base(x, y) { }

        public override void Solve()
        {
            CalculateYShift();
            CalculateXShift();

            int n = XData.Length;

            double[] X_Log = new double[n];
            double[] Y_Log = new double[n];

            for (int i = 0; i < n; i++)
            {
                X_Log[i] = Math.Log(XData[i] + XShift);

                Y_Log[i] = Math.Log(Math.Abs(YData[i] + YShift));
            }

            ThetaMatrix = new double[n, 2];
            for (int i = 0; i < n; i++)
            {
                ThetaMatrix[i, 0] = 1.0;
                ThetaMatrix[i, 1] = X_Log[i];
            }

            var Theta = Matrix<double>.Build.DenseOfArray(ThetaMatrix);
            var Yvec = Vector<double>.Build.Dense(Y_Log);
            var M = Theta.TransposeThisAndMultiply(Theta);
            var r = Theta.TransposeThisAndMultiply(Yvec);

            MMatrix = M.ToArray();
            RVector = r.ToArray();

            var coeffs = M.Solve(r);

            double rawA = Math.Exp(coeffs[0]);
            A = IsShiftedDataNegative ? -rawA : rawA;
            P = coeffs[1];

            var yPredLog = Theta * coeffs;

            MSE = Math.Sqrt((Yvec - yPredLog).PointwisePower(2).Sum() / n);
        }

        public override void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ BÌNH PHƯƠNG TỐI THIỂU - DẠNG LUỸ THỪA");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            if (YShift != 0 || XShift != 0)
            {
                sb.AppendLine("⚠️ XỬ LÝ DỮ LIỆU:");
                if (XShift != 0)
                    sb.AppendLine($"   [Trục X] Dữ liệu chứa giá trị <= 0. Tịnh tiến trục hoành: Sx = {XShift:F6}");

                if (YShift != 0)
                {
                    sb.Append($"   [Trục Y] Tịnh tiến trục tung: Sy = {YShift:F6} ");
                    sb.AppendLine(IsShiftedDataNegative ? "(Dịch về ÂM)" : "(Dịch về DƯƠNG)");
                }
                sb.AppendLine();
            }

            sb.AppendLine("MÔ HÌNH TOÁN HỌC:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");

            string xTerm = XShift != 0 ? $"(x + {XShift:F4})" : "x";
            string yTerm = YShift != 0 ? $"(y + {YShift:F4})" : "y";

            sb.AppendLine($"  Dạng gốc: {yTerm} = a × {xTerm}^p");
            sb.AppendLine($"  Tuyến tính hóa: ln|{yTerm}| = ln|a| + p × ln({xTerm})");
            sb.AppendLine($"  Đặt: Y = ln|{yTerm}|, X = ln({xTerm})");
            sb.AppendLine($"  => Y = A₀ + A₁ × X");
            sb.AppendLine();

            sb.AppendLine("MA TRẬN Θ (THETA):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            // Header động
            string lnXHeader = XShift != 0 ? $"ln(xi + {XShift:F2})" : "ln(xi)";
            sb.AppendLine($"   i |      1       | {lnXHeader,-12} |");
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

            sb.AppendLine("DỮ LIỆU CHUYỂN ĐỔI:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            string hX = XShift != 0 ? "x+Sx" : "x";
            string hY = YShift != 0 ? "y+Sy" : "y";
            sb.AppendLine($"   i |      x       |      y       |   {hX,-10} |   {hY,-10} |  ln({hX})   |  ln|{hY}|  |");
            sb.AppendLine(new string('─', 95));

            for (int i = 0; i < XData.Length; i++)
            {
                double valX = XData[i] + XShift;
                double valY = YData[i] + YShift;
                double lnX = Math.Log(valX);
                double lnY = Math.Log(Math.Abs(valY));

                sb.Append($"  {i + 1,2} | {XData[i],12:F6} | {YData[i],12:F6} |");
                sb.Append($" {valX,12:F6} | {valY,12:F6} |");
                sb.AppendLine($" {lnX,10:F6} | {lnY,10:F6} |");
            }
            sb.AppendLine();

            sb.AppendLine("HỆ PHƯƠNG TRÌNH (M × A = r):");
            sb.AppendLine("Ma trận M = Θ^T × Θ:");
            for (int i = 0; i < MMatrix.GetLength(0); i++)
            {
                sb.Append("  [");
                for (int j = 0; j < MMatrix.GetLength(1); j++) sb.Append($" {MMatrix[i, j],12:F6}");
                sb.AppendLine(" ]");
            }
            sb.AppendLine("Vector r = Θ^T × Y:");
            sb.Append("  r = [");
            for (int i = 0; i < RVector.Length; i++)
            {
                sb.Append($" {RVector[i],12:F6}");
                if (i < RVector.Length - 1) sb.Append(",");
            }
            sb.AppendLine(" ]^T\n");

            sb.AppendLine("KẾT QUẢ HỆ SỐ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  A₀ = ln(|a|) = {Math.Log(Math.Abs(A)):F8} => |a| = {Math.Abs(A):F8}");
            sb.AppendLine($"  A₁ = p       = {P:F8}");
            sb.AppendLine();
            sb.AppendLine($"  => Hệ số a = {A:F8}");
            if (IsShiftedDataNegative) sb.AppendLine("  (Dấu âm do y được dịch về phía âm)");
            sb.AppendLine();

            sb.AppendLine("PHƯƠNG TRÌNH CUỐI CÙNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");

            sb.Append($"  y ≈ {A:F8} × (");
            if (XShift != 0) sb.Append($"x + {XShift:F6}");
            else sb.Append("x");
            sb.Append($")^({P:F8})");

            if (YShift > 0) sb.Append($" - {Math.Abs(YShift):F6}");
            else if (YShift < 0) sb.Append($" + {Math.Abs(YShift):F6}");

            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("BẢNG SO SÁNH GIÁ TRỊ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("   i |      x       |    y_thực    |   y_dự_đoán  |");
            sb.AppendLine(new string('─', 58));

            for (int i = 0; i < XData.Length; i++)
            {
                double x_term = XData[i] + XShift;
                double y_pred = (A * Math.Pow(x_term, P)) - YShift;

                sb.AppendLine($"  {i + 1,2} | {XData[i],12:F6} | {YData[i],12:F6} | {y_pred,12:F6} |");
            }
            sb.AppendLine();

            // --- Phần 7: Sai số ---
            sb.AppendLine("SAI SỐ TRUNG BÌNH PHƯƠNG (MSE):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  MSE = sqrt( Σ( ln|y+S| - ln|y_pred+S| )² = {MSE:F10}");
            sb.AppendLine("  (Tính trên không gian Logarit)");

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
        }
        public override void Solve()
        {
            CalculateYShift();

            int n = XData.Length;
            int m = PhiExpressions.Length;

            double[] Y_Log = new double[n];
            for (int i = 0; i < n; i++)
            {
                Y_Log[i] = Math.Log(Math.Abs(YData[i] + YShift));
            }

            // Ma trận Theta
            ThetaMatrix = new double[n, m + 1];
            for (int i = 0; i < n; i++)
            {
                ThetaMatrix[i, 0] = 1.0;
                for (int j = 0; j < m; j++)
                {
                    ThetaMatrix[i, j + 1] = EvaluatePhi(PhiExpressions[j], XData[i]);
                }
            }

            // Giải hệ phương trình
            var Theta = Matrix<double>.Build.DenseOfArray(ThetaMatrix);
            var Yvec = Vector<double>.Build.Dense(Y_Log);

            var M = Theta.TransposeThisAndMultiply(Theta);
            var r = Theta.TransposeThisAndMultiply(Yvec);

            MMatrix = M.ToArray();
            RVector = r.ToArray();

            var C = M.Solve(r).ToArray();

            double rawA = Math.Exp(C[0]);
            A = IsShiftedDataNegative ? -rawA : rawA;

            BCoefficients = C.Skip(1).ToArray();

            var yPred = Theta * Vector<double>.Build.Dense(C);
            MSE = Math.Sqrt((Yvec - yPred).PointwisePower(2).Sum() / n);
        }
        public override void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ BÌNH PHƯƠNG TỐI THIỂU - DẠNG MŨ TỔNG QUÁT");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            // --- Phần 1: Thông tin mô hình và dịch chuyển ---
            if (YShift != 0)
            {
                sb.AppendLine("⚠️ XỬ LÝ DỮ LIỆU:");
                sb.AppendLine($"   Dữ liệu gốc có dấu hỗn hợp hoặc chứa số 0.");
                sb.AppendLine($"   Đã thực hiện tịnh tiến trục tung một lượng S = {YShift:F6}");
                if (IsShiftedDataNegative)
                    sb.AppendLine("   (Dịch về phía ÂM)");
                else
                    sb.AppendLine("   (Dịch về phía DƯƠNG)");
                sb.AppendLine();
            }

            sb.AppendLine("MÔ HÌNH TOÁN HỌC:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("Mô hình: (y + S) = a × e^(");
            var expTerms = new List<string>();
            for (int j = 0; j < PhiExpressions.Length; j++)
            {
                expTerms.Add($"b{j + 1}×φ{j + 1}(x)");
            }
            sb.Append(string.Join(" + ", expTerms));
            sb.AppendLine(")");

            sb.Append("  Tuyến tính hóa: ln|y + S| = ln|a| + ");
            sb.AppendLine(string.Join(" + ", expTerms));
            sb.AppendLine();

            // --- Phần 2: Ma trận Theta ---
            sb.AppendLine("MA TRẬN Θ (THETA):");
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

            // --- Phần 3: Dữ liệu chuyển đổi ---
            sb.AppendLine("DỮ LIỆU CHUYỂN ĐỔI (LOGARIT):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("   i |      x       |      y       |   y + S      |  ln|y + S|   |");
            sb.AppendLine(new string('─', 75));

            for (int i = 0; i < XData.Length; i++)
            {
                double shiftedY = YData[i] + YShift;
                double logY = Math.Log(Math.Abs(shiftedY));
                sb.AppendLine($"  {i + 1,2} | {XData[i],12:F6} | {YData[i],12:F6} | {shiftedY,12:F6} | {logY,12:F6} |");
            }
            sb.AppendLine();

            // --- Phần 4: Ma trận M và Vector r ---
            sb.AppendLine("HỆ PHƯƠNG TRÌNH (M × C = r):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("Ma trận M = Θ^T × Θ:");
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

            sb.AppendLine("Vector r = Θ^T × ln|y+S|:");
            sb.Append("  r = [");
            for (int i = 0; i < RVector.Length; i++)
            {
                sb.Append($" {RVector[i],12:F6}");
                if (i < RVector.Length - 1) sb.Append(",");
            }
            sb.AppendLine(" ]^T\n");

            // --- Phần 5: Kết quả hệ số ---
            sb.AppendLine("KẾT QUẢ HỆ SỐ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  C₀ = {Math.Log(Math.Abs(A)):F8}  => |a| = {Math.Abs(A):F8}");
            for (int j = 0; j < BCoefficients.Length; j++)
            {
                sb.AppendLine($"  C{j + 1} = b{j + 1} = {BCoefficients[j]:F8}");
            }
            sb.AppendLine();

            sb.AppendLine($"  => Hệ số a = {A:F8}");
            if (IsShiftedDataNegative) sb.AppendLine("  (Dấu âm do dữ liệu được dịch về phía âm)");
            sb.AppendLine();

            // --- Phần 6: Phương trình cuối cùng ---
            sb.AppendLine("PHƯƠNG TRÌNH XẤP XỈ CUỐI CÙNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append($"  y ≈ {A:F6} × e^(");

            var finalTerms = new List<string>();
            for (int j = 0; j < PhiExpressions.Length; j++)
            {
                string sign = BCoefficients[j] >= 0 ? "+" : "";
                if (j == 0 && BCoefficients[j] >= 0) sign = "";
                finalTerms.Add($"{sign}{BCoefficients[j]:F6}·({PhiExpressions[j]})");
            }
            sb.Append(string.Join(" ", finalTerms));
            sb.Append(")");

            if (YShift > 0)
            {
                sb.Append($" - {Math.Abs(YShift):F6}");
            }
            else if (YShift < 0)
            {
                sb.Append($" + {Math.Abs(YShift):F6}"); 
            }

            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("BẢNG SO SÁNH GIÁ TRỊ THỰC TẾ VÀ DỰ ĐOÁN:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("   i |      x       |    y_thực    |   y_dự_đoán  |");
            sb.AppendLine(new string('─', 58));

            for (int i = 0; i < XData.Length; i++)
            {
                double exponent = 0;
                for (int j = 0; j < PhiExpressions.Length; j++)
                {
                    exponent += BCoefficients[j] * EvaluatePhi(PhiExpressions[j], XData[i]);
                }

                double y_pred = (A * Math.Exp(exponent)) - YShift;

                sb.AppendLine($"  {i + 1,2} | {XData[i],12:F6} | {YData[i],12:F6} | {y_pred,12:F6} |");
            }
            sb.AppendLine();

            // --- Phần 7: Sai số ---
            sb.AppendLine("SAI SỐ TRUNG BÌNH PHƯƠNG (MSE):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  MSE = sqrt( Σ( ln|y+S| - ln|y_pred+S| )² / n ) = {MSE:F10}");
            sb.AppendLine("  Lưu ý: Sai số tính trên không gian Logarit");
            sb.AppendLine();

            rtb.AppendText(sb.ToString());
        }
    }
}