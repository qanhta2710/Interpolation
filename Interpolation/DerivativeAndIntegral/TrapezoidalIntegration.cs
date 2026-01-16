using AngouriMath;
using System;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public class TrapezoidalIntegration
    {
        // Properties
        public double[] XData { get; set; }
        public double[] YData { get; set; }
        public Entity FunctionExpr { get; set; }

        public double A { get; set; } // Cận dưới
        public double B { get; set; } // Cận trên
        public double H { get; set; } // Bước nhảy
        public int N { get; set; } // Số khoảng chia
        public double Epsilon { get; set; }
        public double M2 { get; set; } // Max |f''(x)| trên [a, b]
        public double Result { get; set; }
        public double EstimatedError { get; set; }
        public bool IsFromData { get; private set; }

        private StringBuilder calculationSteps;

        // Dữ liệu rời rạc
        public TrapezoidalIntegration(double[] x, double[] y, double a, double b)
        {
            XData = x;
            YData = y;
            A = a;
            B = b;
            IsFromData = true;
            Solve();
            EstimateErrorGeneral();
        }
        // Hàm F(x) với sai số cho trước
        public TrapezoidalIntegration(string functionStr, double a, double b, double epsilon, double m2)
        {
            try
            {
                FunctionExpr = functionStr;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}");
            }

            A = a;
            B = b;
            Epsilon = epsilon;
            M2 = m2;
            IsFromData = false;

            CalculateOptimalN();
            Solve();
        }
        // Hàm F(x) với khoảng chia cho trước
        public TrapezoidalIntegration(string functionStr, double a, double b, int n, double m2)
        {
            try
            {
                FunctionExpr = functionStr;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}");
            }

            A = a;
            B = b;
            N = n;
            M2 = m2;
            IsFromData = false;
            Solve();
            CalculateTheoreticalError();
            EstimateErrorGeneral();
        }

        private void Solve()
        {
            calculationSteps = new StringBuilder();

            if (IsFromData)
            {
                SolveFromData();
            }
            else
            {
                SolveFromFunction();
            }
        }

        // Dữ liệu rời rạc
        private void SolveFromData()
        {
            if (XData == null || YData == null)
                throw new ArgumentException("Dữ liệu không được null");

            if (XData.Length != YData.Length)
                throw new ArgumentException("Số lượng điểm x và y không khớp");

            if (XData.Length < 2)
                throw new ArgumentException("Cần ít nhất 2 điểm dữ liệu");

            int indexA = Array.IndexOf(XData, A);
            int indexB = Array.IndexOf(XData, B);

            if (indexA == -1)
                throw new ArgumentException($"Không tìm thấy cận dưới a = {A} trong dữ liệu");

            if (indexB == -1)
                throw new ArgumentException($"Không tìm thấy cận trên b = {B} trong dữ liệu");

            if (indexA >= indexB)
                throw new ArgumentException("Cận dưới phải nhỏ hơn cận trên");

            N = indexB - indexA;
            H = (B - A) / N;

            calculationSteps.AppendLine("═══ CÔNG THỨC HÌNH THANG - DỮ LIỆU RỜI RẠC ═══");
            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"Cận tích phân: [{A}, {B}]");
            calculationSteps.AppendLine($"Số khoảng chia: n = {N}");
            calculationSteps.AppendLine($"Bước nhảy: h = (b-a)/n = ({B}-{A})/{N} = {H}");
            if (Epsilon > 0) calculationSteps.AppendLine($"Sai số cho phép: ε = {Epsilon}");
            calculationSteps.AppendLine();

            calculationSteps.AppendLine("Công thức: I ≈ (h/2) × [y₀ + 2(y₁ + y₂ + ... + y_(n-1)) + yₙ]");
            calculationSteps.AppendLine();

            double sum = YData[indexA] + YData[indexB];
            calculationSteps.AppendLine($"y₀ = {YData[indexA]}");
            calculationSteps.AppendLine($"yₙ = {YData[indexB]}");

            if (N > 1)
            {
                double middleSum = 0;

                for (int i = indexA + 1; i < indexB; i++)
                {
                    middleSum += YData[i];
                }

                sum += 2 * middleSum;
            }

            Result = (H / 2.0) * sum;

            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"I ≈ ({H}/2) × [{YData[indexA]} + 2×{(sum - YData[indexA] - YData[indexB]) / 2} + {YData[indexB]}]");
            calculationSteps.AppendLine($"  = ({H}/2) × {sum}");
            calculationSteps.AppendLine($"  = {Result}");
        }

        // Hàm F(x)
        private void SolveFromFunction()
        {
            H = (B - A) / N;

            XData = new double[N + 1];
            YData = new double[N + 1];

            calculationSteps.AppendLine("═══ CÔNG THỨC HÌNH THANG - HÀM LIÊN TỤC ═══");
            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"Hàm số: f(x) = {FunctionExpr}");
            calculationSteps.AppendLine($"Cận tích phân: [{A}, {B}]");

            if (Epsilon > 0 && M2 > 0)
            {
                calculationSteps.AppendLine();
                calculationSteps.AppendLine("TÍNH SỐ KHOẢNG CHIA:");
                calculationSteps.AppendLine($"Sai số cho phép: ε = {Epsilon}");
                calculationSteps.AppendLine($"Max |f''(x)| trên [{A}, {B}]: M₂ = {M2}");
                calculationSteps.AppendLine();
                calculationSteps.AppendLine($"Công thức: n = ⌈√((b-a)³ × M₂ / (12ε))⌉ = {N}");
            }
            else
            {
                calculationSteps.AppendLine($"Số khoảng chia (cho trước): n = {N}");
                calculationSteps.AppendLine($"Max |f''(x)| trên [{A}, {B}]: M₂ = {M2}");
            }

            calculationSteps.AppendLine($"Bước nhảy: h = (b-a)/n = {H}");
            calculationSteps.AppendLine();

            calculationSteps.AppendLine("Công thức: I ≈ (h/2) × [f(a) + 2∑f(xᵢ) + f(b)]");
            calculationSteps.AppendLine();

            var compiledFunc = FunctionExpr.Compile("x");

            double sum = 0;
            double middleSum = 0;

            for (int i = 0; i <= N; i++)
            {
                double xi = A + i * H;
                double fxi = EvaluateFunction(compiledFunc, xi);

                XData[i] = xi;
                YData[i] = fxi;

                if (i == 0 || i == N)
                    sum += fxi;
                else
                    middleSum += fxi;
            }

            calculationSteps.AppendLine($"f(a) = f({A}) = {YData[0]:F8}");
            calculationSteps.AppendLine($"f(b) = f({B}) = {YData[N]:F8}");
            calculationSteps.AppendLine();

            sum += 2 * middleSum;

            Result = (H / 2.0) * sum;

            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"I ≈ ({H}/2) × [{YData[0]:F8} + 2×{middleSum:F8} + {YData[N]:F8}]");
            calculationSteps.AppendLine($"  = ({H}/2) × {sum:F8}");
            calculationSteps.AppendLine($"  = {Result}");
        }

        private void CalculateTheoreticalError()
        {
            double theoError = Math.Pow(B - A, 3) / (12 * Math.Pow(N, 2)) * M2;
            calculationSteps.AppendLine();
            calculationSteps.AppendLine("═══ SAI SỐ LÝ THUYẾT ═══");
            calculationSteps.AppendLine($"Công thức: R = (b-a)³/(12n²) × M₂ = {theoError}");
        }

        private void EstimateErrorGeneral()
        {
            if (N < 2)
            {
                EstimatedError = 0;
                return;
            }
            int p = -1;
            for (int i = 2; i <= Math.Sqrt(N); i++)
            {
                if (N % i == 0)
                {
                    p = i;
                    break;
                }
            }

            if (p == -1)
            {
                calculationSteps.AppendLine();
                calculationSteps.AppendLine($"(!) N = {N} là số nguyên tố. Không thể chia lưới đều để đánh giá sai số. (Tách thành 2 khoảng rồi tính / Đang hoàn thiện tính năng)");
                EstimatedError = 0;
                return;
            }

            int n_coarse = N / p;
            double h_coarse = H * p;

            int indexA = Array.IndexOf(XData, A);

            if (indexA == -1 && !IsFromData) indexA = 0;

            double sumCoarse = YData[indexA] + YData[indexA + N];

            for (int i = 1; i < n_coarse; i++)
            {
                int originalIndex = indexA + (i * p);
                sumCoarse += 2 * YData[originalIndex];
            }

            double I_coarse = (h_coarse / 2.0) * sumCoarse;

            double denominator = Math.Pow(p, 2) - 1;
            EstimatedError = Math.Abs(Result - I_coarse) / denominator;

            calculationSteps.AppendLine();
            calculationSteps.AppendLine("═══ SAI SỐ LƯỚI PHỦ (Runge) ═══");
            calculationSteps.AppendLine($"Lưới ban đầu (mịn): N = {N} khoảng, h = {H}");
            calculationSteps.AppendLine($"Lưới thô: n = {n_coarse} khoảng, h' = {h_coarse}");
            calculationSteps.AppendLine($"Tỉ lệ làm mịn: p = {p} (N = p × n)");
            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"I_N   (lưới mịn - {N} khoảng)    = {Result}");
            calculationSteps.AppendLine($"I_n   (lưới thô - {n_coarse} khoảng)     = {I_coarse}");
            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"Công thức Runge: R ≈ |I_N - I_n| / (p² - 1)");
            calculationSteps.AppendLine($"                  = |{Result} - {I_coarse}| / ({p}² - 1)");
            calculationSteps.AppendLine($"                  = {Math.Abs(Result - I_coarse)} / {denominator}");
            calculationSteps.AppendLine($"R ≈ {EstimatedError}");

        }
        private double EvaluateFunction(AngouriMath.Core.FastExpression compiledFunc, double x)
        {
            try
            {
                var result = compiledFunc.Call(x);
                return (double)result.Real;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Lỗi tính f({x}): {ex.Message}");
            }
        }

        public void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(calculationSteps.ToString());

            sb.AppendLine();
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine($"KẾT QUẢ: ∫f(x)dx ≈ {Result}");

            rtb.Text = sb.ToString();
        }
        private void CalculateOptimalN()
        {
            double numerator = Math.Pow(B - A, 3) * M2;
            double denominator = 12 * Epsilon;
            N = (int)Math.Ceiling(Math.Sqrt(numerator / denominator));
        }
    }
}