using AngouriMath;
using System;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public class MidpointIntegration
    {
        // Properties
        public double[] XData { get; set; }
        public double[] YData { get; set; }
        public Entity FunctionExpr { get; set; }

        public double A { get; set; }  // Cận dưới
        public double B { get; set; }  // Cận trên
        public double H { get; set; }  // Bước nhảy
        public int N { get; set; }     // Số khoảng chia
        public double Epsilon { get; set; }  // Sai số cho phép
        public double M2 { get; set; }  // |f''(x)| max trên [a,b]

        public double Result { get; set; }
        public double EstimatedError { get; set; }
        public bool IsFromData { get; private set; }

        private StringBuilder calculationSteps;

        // Constructor 1: Dữ liệu rời rạc
        public MidpointIntegration(double[] x, double[] y, double a, double b)
        {
            XData = x;
            YData = y;
            A = a;
            B = b;
            IsFromData = true;
            Solve();
            EstimateErrorRunge();
        }

        // Constructor 2: Hàm F(x) với sai số cho trước (Tính N)
        public MidpointIntegration(string functionStr, double a, double b, double epsilon)
        {
            try { FunctionExpr = functionStr; }
            catch (Exception ex) { throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}"); }

            A = a;
            B = b;
            Epsilon = epsilon;
            IsFromData = false;

            CalculateM2(); // Tự quét tìm M2
            CalculateOptimalN();
            Solve();
        }

        // Constructor 3: Hàm F(x) với N cho trước
        public MidpointIntegration(string functionStr, double a, double b, int n)
        {
            try { FunctionExpr = functionStr; }
            catch (Exception ex) { throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}"); }

            A = a;
            B = b;
            N = n;
            IsFromData = false;

            CalculateM2();
            Solve();
            CalculateTheoreticalError();
            EstimateErrorRunge();
        }

        private void Solve()
        {
            calculationSteps = new StringBuilder();
            if (IsFromData) SolveFromData();
            else SolveFromFunction();
        }

        private void SolveFromData()
        {
            int indexA = Array.IndexOf(XData, A);
            int indexB = Array.IndexOf(XData, B);
            if (indexA == -1 || indexB == -1 || indexA >= indexB)
                throw new ArgumentException("Cận a, b không hợp lệ hoặc không tìm thấy trong dữ liệu.");
            if (XData.Length < 3 || XData.Length % 2 == 0)
                throw new ArgumentException("Dữ liệu điểm giữa rời rạc cần số điểm lẻ (x0, x1, x2...) để x1 là trung điểm.");

            N = (indexB - indexA) / 2;
            H = (B - A) / N;

            calculationSteps.AppendLine("═══ CÔNG THỨC ĐIỂM GIỮA - DỮ LIỆU RỜI RẠC ═══");
            calculationSteps.AppendLine($"Cận: [{A}, {B}]. Số khoảng chia thực tế: N = {N}");
            calculationSteps.AppendLine($"Bước nhảy mỗi khoảng: H = {H}");
            calculationSteps.AppendLine("Sử dụng các điểm x lẻ làm trung điểm của mỗi khoảng.");

            double sum = 0;
            for (int i = 0; i < N; i++)
            {
                double midY = YData[2 * i + 1];
                sum += midY;
                calculationSteps.AppendLine($"Khoảng {i + 1}: Trung điểm x = {XData[2 * i + 1]}, y = {midY}");
            }

            Result = sum * H;
            calculationSteps.AppendLine($"Kết quả: I ≈ {H} * Σy_mid = {H} * {sum} = {Result}");
        }

        private void SolveFromFunction()
        {
            H = (B - A) / N;
            calculationSteps.AppendLine("═══ CÔNG THỨC ĐIỂM GIỮA - HÀM LIÊN TỤC ═══");
            calculationSteps.AppendLine($"Hàm số: f(x) = {FunctionExpr}");
            calculationSteps.AppendLine($"Cận: [{A}, {B}], n = {N}, h = {H}");

            var compiledFunc = FunctionExpr.Compile("x");
            double sum = 0;

            calculationSteps.AppendLine("\nTính giá trị tại các trung điểm:");
            for (int i = 0; i < N; i++)
            {
                double xMid = A + (i + 0.5) * H;
                double yMid = EvaluateFunction(compiledFunc, xMid);
                sum += yMid;
                calculationSteps.AppendLine($"  x_mid_{i} = {xMid:F6} => f(x_mid) = {yMid:F8}");
            }

            Result = sum * H;
            calculationSteps.AppendLine($"\nCông thức: I ≈ h * Σ f(x_mid)");
            calculationSteps.AppendLine($"I ≈ {H} * {sum:F8} = {Result}");
        }

        private void CalculateM2()
        {
            try
            {
                var d2 = FunctionExpr.Differentiate("x").Differentiate("x");
                var compiledD2 = d2.Compile("x");

                double maxVal = 0;
                int scanSteps = 1000;
                double step = (B - A) / scanSteps;

                for (int i = 0; i <= scanSteps; i++)
                {
                    double currX = A + i * step;
                    double val = Math.Abs((double)compiledD2.Call(currX).Real);
                    if (val > maxVal) maxVal = val;
                }
                M2 = maxVal;
                if (M2 < 1e-10) M2 = 0; // Hàm bậc nhất
            }
            catch { M2 = 0; }
        }

        private void CalculateOptimalN()
        {
            if (M2 == 0) { N = 2; return; }

            double nFloat = Math.Sqrt((M2 * Math.Pow(B - A, 3)) / (24 * Epsilon));
            N = (int)Math.Ceiling(nFloat);
            if (N < 1) N = 1;
        }

        private void CalculateTheoreticalError()
        {
            double error = (M2 / 24.0) * (B - A) * Math.Pow(H, 2);
            calculationSteps.AppendLine("\n═══ SAI SỐ LÝ THUYẾT ═══");
            calculationSteps.AppendLine($"M2 (max|f''|): {M2:F6}");
            calculationSteps.AppendLine($"R = (M2/24) * (b-a) * h² ≈ {error:F10}");
        }

        private void EstimateErrorRunge()
        {
            // Điều kiện tiên quyết: Số khoảng chia N phải chẵn để có thể chia đôi lưới
            if (N % 2 != 0)
            {
                EstimatedError = 0;
                calculationSteps.AppendLine("\n(!) Không thể tính sai số Runge vì N là số lẻ (không tạo được lưới thô lồng nhau).");
                return;
            }

            double I_coarse = 0;
            int n_coarse = N / 2;
            double h_coarse = H * 2;

            calculationSteps.AppendLine("\n═══ SAI SỐ LƯỚI PHỦ (RUNGE) ═══");
            calculationSteps.AppendLine($"Lưới mịn (hiện tại): N = {N}, H = {H}");
            calculationSteps.AppendLine($"Lưới thô (kiểm chứng): N = {n_coarse}, H' = {h_coarse}");

            if (IsFromData)
            {
                double sumCoarse = 0;
                calculationSteps.AppendLine("Các điểm được chọn cho lưới thô (index = 4j + 2):");

                // Kiểm tra biên dữ liệu
                int maxIndexNeeded = 4 * (n_coarse - 1) + 2;
                if (maxIndexNeeded >= YData.Length)
                {
                    calculationSteps.AppendLine("(!) Lỗi: Không đủ dữ liệu tại các điểm mốc cần thiết để tính lưới thô.");
                    EstimatedError = 0;
                    return;
                }

                for (int j = 0; j < n_coarse; j++)
                {
                    int index = 4 * j + 2;
                    double yVal = YData[index];
                    sumCoarse += yVal;
                }

                I_coarse = sumCoarse * h_coarse;
            }
            else
            {
                // --- XỬ LÝ CHO HÀM SỐ ---
                var compiledFunc = FunctionExpr.Compile("x");
                double sumCoarse = 0;

                for (int i = 0; i < n_coarse; i++)
                {
                    double xMid = A + (i + 0.5) * h_coarse;
                    sumCoarse += EvaluateFunction(compiledFunc, xMid);
                }
                I_coarse = sumCoarse * h_coarse;
            }

            calculationSteps.AppendLine($"I_mịn = {Result}");
            calculationSteps.AppendLine($"I_thô = {I_coarse}");

            // Công thức Runge cho phương pháp bậc 2 (Điểm giữa, Hình thang): chia cho (2^2 - 1) = 3
            EstimatedError = Math.Abs(Result - I_coarse) / 3.0;

            calculationSteps.AppendLine($"Công thức: R ≈ |I_mịn - I_thô| / 3");
            calculationSteps.AppendLine($"R ≈ |{Result} - {I_coarse}| / 3 = {EstimatedError}");
        }

        private double EvaluateFunction(AngouriMath.Core.FastExpression compiledFunc, double x)
        {
            return (double)compiledFunc.Call(x).Real;
        }

        public void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            rtb.Text = calculationSteps.ToString() +
                       $"\n════════════════════════════════════\nKẾT QUẢ CUỐI CÙNG: {Result}";
        }
    }
}