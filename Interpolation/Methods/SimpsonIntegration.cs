using AngouriMath;
using System;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public class SimpsonIntegration
    {
        // Properties
        public double[] XData { get; set; }
        public double[] YData { get; set; }
        public Entity FunctionExpr { get; set; }

        public double A { get; set; }  // Cận dưới
        public double B { get; set; }  // Cận trên
        public double H { get; set; }  // Bước nhảy
        public int N { get; set; }     // Số khoảng chia (phải chẵn)
        public double Epsilon { get; set; }  // Sai số cho phép
        public double M4 { get; set; }  // |f⁽⁴⁾(x)| max trên [a,b]

        public double Result { get; set; }
        public double EstimatedError { get; set; }
        public bool IsFromData { get; private set; }

        private StringBuilder calculationSteps;

        // Dữ liệu rời rạc
        public SimpsonIntegration(double[] x, double[] y, double a, double b, double epsilon)
        {
            XData = x;
            YData = y;
            A = a;
            B = b;
            Epsilon = epsilon;
            IsFromData = true;
            Solve();
        }

        // Hàm F(x) với sai số cho trước
        public SimpsonIntegration(string functionStr, double a, double b, double epsilon, double m4)
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
            M4 = m4;
            IsFromData = false;

            CalculateOptimalN();
            Solve();
        }

        // Hàm F(x) với khoảng chia cho trước
        public SimpsonIntegration(string functionStr, double a, double b, int n, double m4)
        {
            try
            {
                FunctionExpr = functionStr;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}");
            }

            if (n % 2 != 0)
            {
                throw new ArgumentException("Công thức Simpson yêu cầu n phải chẵn!");
            }

            A = a;
            B = b;
            N = n;
            M4 = m4;
            IsFromData = false;
            Solve();
        }

        private void CalculateOptimalN()
        {
            double numerator = Math.Pow(B - A, 5) * M4;
            double denominator = 180 * Epsilon;
            int n = (int)Math.Ceiling(Math.Pow(numerator / denominator, 0.25));

            if (n % 2 != 0)
            {
                n++;
            }

            N = n;
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

            // Gọi hàm tính sai số lưới phủ sau khi đã có kết quả
            EstimateErrorGeneral();
        }

        // Dữ liệu rời rạc
        private void SolveFromData()
        {
            if (XData == null || YData == null)
                throw new ArgumentException("Dữ liệu không được null");

            if (XData.Length != YData.Length)
                throw new ArgumentException("Số lượng điểm x và y không khớp");

            if (XData.Length < 3)
                throw new ArgumentException("Cần ít nhất 3 điểm dữ liệu");

            int indexA = Array.IndexOf(XData, A);
            int indexB = Array.IndexOf(XData, B);

            if (indexA == -1)
                throw new ArgumentException($"Không tìm thấy cận dưới a = {A} trong dữ liệu");

            if (indexB == -1)
                throw new ArgumentException($"Không tìm thấy cận trên b = {B} trong dữ liệu");

            if (indexA >= indexB)
                throw new ArgumentException("Cận dưới phải nhỏ hơn cận trên");

            N = indexB - indexA;

            if (N % 2 != 0)
            {
                throw new ArgumentException("Số khoảng chia phải chẵn cho công thức Simpson!");
            }

            H = (B - A) / N;

            calculationSteps.AppendLine("═══ CÔNG THỨC SIMPSON - DỮ LIỆU RỜI RẠC ═══");
            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"Cận tích phân: [{A}, {B}]");
            calculationSteps.AppendLine($"Số khoảng chia: n = {N}");
            calculationSteps.AppendLine($"Bước nhảy: h = (b-a)/n = ({B}-{A})/{N} = {H}");
            if (Epsilon > 0) calculationSteps.AppendLine($"Sai số cho phép: ε = {Epsilon}");
            calculationSteps.AppendLine();

            calculationSteps.AppendLine("Công thức: I ≈ (h/3) × [y₀ + 4(y₁ + y₃ + ... + y_(n-1)) + 2(y₂ + y₄ + ... + y_(n-2)) + yₙ]");
            calculationSteps.AppendLine();

            double sum = YData[indexA] + YData[indexB];
            double oddSum = 0;   // y₁, y₃, y₅, ... (chỉ số lẻ)
            double evenSum = 0;  // y₂, y₄, y₆, ... (chỉ số chẵn)

            calculationSteps.AppendLine($"y₀ = {YData[indexA]}");
            calculationSteps.AppendLine($"yₙ = {YData[indexB]}");
            calculationSteps.AppendLine();

            for (int i = 1; i < N; i++)
            {
                if (i % 2 == 1)
                {
                    oddSum += YData[indexA + i];
                }
                else
                {
                    evenSum += YData[indexA + i];
                }
            }

            calculationSteps.AppendLine($"Tổng các điểm chỉ số lẻ: {oddSum:F8}");
            calculationSteps.AppendLine($"Tổng các điểm chỉ số chẵn: {evenSum:F8}");

            Result = (H / 3.0) * (sum + 4 * oddSum + 2 * evenSum);

            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"I ≈ ({H}/3) × [{YData[indexA]} + 4×{oddSum:F8} + 2×{evenSum:F8} + {YData[indexB]}]");
            calculationSteps.AppendLine($"  = ({H}/3) × {sum + 4 * oddSum + 2 * evenSum:F8}");
            calculationSteps.AppendLine($"  = {Result}");
        }

        // Hàm F(x)
        private void SolveFromFunction()
        {
            H = (B - A) / N;

            // Khởi tạo mảng dữ liệu để phục vụ tính sai số lưới phủ
            XData = new double[N + 1];
            YData = new double[N + 1];

            calculationSteps.AppendLine("═══ CÔNG THỨC SIMPSON - HÀM LIÊN TỤC ═══");
            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"Hàm số: f(x) = {FunctionExpr}");
            calculationSteps.AppendLine($"Cận tích phân: [{A}, {B}]");
            calculationSteps.AppendLine();

            try
            {
                var d1 = FunctionExpr.Differentiate("x");
                var d2 = d1.Differentiate("x");
                var d3 = d2.Differentiate("x");
                var d4 = d3.Differentiate("x");
                calculationSteps.AppendLine($"M₄ = max|f⁽⁴⁾(x)| trên [{A}, {B}] ≈ {M4:F6}");
                calculationSteps.AppendLine();
            }
            catch { }

            if (Epsilon > 0 && M4 > 0)
            {
                calculationSteps.AppendLine("TÍNH SỐ KHOẢNG CHIA:");
                calculationSteps.AppendLine($"Sai số cho phép: ε = {Epsilon}");
                calculationSteps.AppendLine();
                calculationSteps.AppendLine($"Công thức: n = ⌈((b-a)⁵ × M₄ / (180ε))^(1/4)⌉ = {N}");
            }
            else
            {
                calculationSteps.AppendLine($"Số khoảng chia (cho trước): n = {N}");
            }

            calculationSteps.AppendLine($"Bước nhảy: h = (b-a)/n = ({B}-{A})/{N} = {H}");
            calculationSteps.AppendLine();

            calculationSteps.AppendLine("Công thức Simpson: I ≈ (h/3) × [f(a) + 4∑f(x_lẻ) + 2∑f(x_chẵn) + f(b)]");
            calculationSteps.AppendLine();

            var compiledFunc = FunctionExpr.Compile("x");

            double sum = 0;
            double oddSum = 0;   // Các điểm có chỉ số lẻ (x₁, x₃, x₅, ...)
            double evenSum = 0;  // Các điểm có chỉ số chẵn (x₂, x₄, x₆, ...)

            // Tính toán và lưu trữ dữ liệu
            for (int i = 0; i <= N; i++)
            {
                double xi = A + i * H;
                double fxi = EvaluateFunction(compiledFunc, xi);

                // Lưu vào mảng
                XData[i] = xi;
                YData[i] = fxi;

                if (i == 0 || i == N)
                {
                    sum += fxi;
                }
                else if (i % 2 == 1)
                {
                    oddSum += fxi;
                }
                else
                {
                    evenSum += fxi;
                }
            }

            calculationSteps.AppendLine($"f(a) = f({A}) = {YData[0]:F8}");
            calculationSteps.AppendLine($"f(b) = f({B}) = {YData[N]:F8}");
            calculationSteps.AppendLine();

            calculationSteps.AppendLine("Các điểm có chỉ số lẻ:");
            // In log cho các điểm lẻ (đã tính ở trên)
            for (int i = 1; i < N; i += 2)
            {
                calculationSteps.AppendLine($"  f(x{i}) = f({XData[i]:F6}) = {YData[i]:F8}");
            }
            calculationSteps.AppendLine($"  Tổng: {oddSum:F8}");
            calculationSteps.AppendLine();

            calculationSteps.AppendLine("Các điểm có chỉ số chẵn:");
            // In log cho các điểm chẵn
            for (int i = 2; i < N; i += 2)
            {
                calculationSteps.AppendLine($"  f(x{i}) = f({XData[i]:F6}) = {YData[i]:F8}");
            }
            calculationSteps.AppendLine($"  Tổng: {evenSum:F8}");
            calculationSteps.AppendLine();

            Result = (H / 3.0) * (sum + 4 * oddSum + 2 * evenSum);

            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"I ≈ ({H}/3) × [{YData[0]:F8} + 4×{oddSum:F8} + 2×{evenSum:F8} + {YData[N]:F8}]");
            calculationSteps.AppendLine($"  = ({H}/3) × {sum + 4 * oddSum + 2 * evenSum:F8}");
            calculationSteps.AppendLine($"  = {Result}");

            // Tính sai số lý thuyết
            if (M4 > 0)
            {
                CalculateTheoreticalError();
            }
        }

        private void CalculateTheoreticalError()
        {
            double theoError = (M4 / 180.0) * (B - A) * Math.Pow(H, 4);

            calculationSteps.AppendLine();
            calculationSteps.AppendLine("═══ SAI SỐ LÝ THUYẾT ═══");
            calculationSteps.AppendLine("Công thức: R = M₄/(180) × (b-a) × h⁴");
            calculationSteps.AppendLine($"         = {M4:F6}/(180) × ({B}-{A}) × {H}⁴");
            calculationSteps.AppendLine($"         = {M4:F6}/180 × {B - A} × {Math.Pow(H, 4):F10}");
            calculationSteps.AppendLine($"         ≈ {theoError:F10}");
        }

        private void EstimateErrorGeneral()
        {
            // 1. Kiểm tra điều kiện cơ bản
            // Simpson cần ít nhất 4 đoạn để có thể chia đôi lưới (mỗi lưới con phải chẵn)
            if (N < 2)
            {
                EstimatedError = 0;
                return;
            }

            // 2. Tìm p sao cho N chia hết cho p VÀ (N/p) cũng phải là số chẵn
            // Vì lưới thưa (Coarse) dùng cho Simpson cũng yêu cầu số khoảng chia là chẵn.
            int p = -1;

            // Duyệt tìm p. 
            // Ta ưu tiên p nhỏ nhất để lưới thưa gần lưới mịn nhất (thường là p=2 nếu N chẵn)
            for (int i = 2; i <= Math.Sqrt(N); i++)
            {
                // Điều kiện: N chia hết cho i VÀ số khoảng mới (N/i) phải chẵn
                if (N % i == 0 && (N / i) % 2 == 0)
                {
                    p = i;
                    break;
                }
            }

            // Nếu không tìm thấy p (ví dụ N=6, chia 2 được 3 (lẻ) -> không dùng Simpson cho lưới thưa được
            // hoặc N là số nguyên tố)
            if (p == -1)
            {
                calculationSteps.AppendLine();
                calculationSteps.AppendLine($"(!) Không tìm được tỷ lệ p sao cho lưới thưa có số khoảng chẵn (N={N}).");
                calculationSteps.AppendLine("    Không thể áp dụng đánh giá sai số lưới phủ cho Simpson.");
                EstimatedError = 0;
                return;
            }

            // 3. Thiết lập lưới thưa
            // Lưới mịn: N khoảng.
            // Lưới thưa: N' = N/p khoảng (N' chẵn). Bước nhảy h' = p*h.

            int n_coarse = N / p;
            double h_coarse = H * p;

            int indexA = Array.IndexOf(XData, A);

            // Xử lý an toàn indexA
            if (indexA == -1 && !IsFromData) indexA = 0;

            // Tính Simpson cho lưới thưa
            double sumCoarse = YData[indexA] + YData[indexA + N]; // Đầu + Cuối
            double oddSumCoarse = 0;
            double evenSumCoarse = 0;

            // Duyệt qua các điểm của lưới thưa: 1, 2, ..., n_coarse - 1
            for (int i = 1; i < n_coarse; i++)
            {
                int originalIndex = indexA + (i * p); // Chỉ số tương ứng trên mảng dữ liệu gốc

                if (i % 2 == 1) // Lẻ trên lưới thưa
                {
                    oddSumCoarse += YData[originalIndex];
                }
                else // Chẵn trên lưới thưa
                {
                    evenSumCoarse += YData[originalIndex];
                }
            }

            double I_coarse = (h_coarse / 3.0) * (sumCoarse + 4 * oddSumCoarse + 2 * evenSumCoarse);

            // 4. Tính sai số theo công thức Runge cho Simpson (O(h^4))
            // R = |I_fine - I_coarse| / (p^4 - 1)

            double denominator = Math.Pow(p, 4) - 1;
            EstimatedError = Math.Abs(Result - I_coarse) / denominator;

            // 5. Ghi log
            calculationSteps.AppendLine();
            calculationSteps.AppendLine("═══ SAI SỐ LƯỚI PHỦ (Runge) ═══");
            calculationSteps.AppendLine($"Lưới ban đầu (mịn): N = {N} khoảng (chẵn), h = {H}");
            calculationSteps.AppendLine($"Lưới thô: n = {n_coarse} khoảng (chẵn), h' = {h_coarse}");
            calculationSteps.AppendLine($"Tỉ lệ làm mịn: p = {p} (N = p × n)");
            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"I_N   (lưới mịn - {N} khoảng)    = {Result}");
            calculationSteps.AppendLine($"I_n   (lưới thô - {n_coarse} khoảng)     = {I_coarse}");
            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"Công thức Runge: R ≈ |I_N - I_n| / (p⁴ - 1)");
            calculationSteps.AppendLine($"                  = |{Result:F8} - {I_coarse:F8}| / ({p}⁴ - 1)");
            calculationSteps.AppendLine($"                  = {Math.Abs(Result - I_coarse):F10} / {denominator}");
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
    }
}