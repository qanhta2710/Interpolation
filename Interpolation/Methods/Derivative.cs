using AngouriMath;
using System;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public enum DerivativeMethod { TwoPoints = 2, ThreePoints = 3, FourPoints = 4 }
    public enum DataPosition { Start, Center, End }
    
    public class Derivative
    {
        public double[] XData { get; set; }
        public double[] YData { get; set; }

        public double Result { get; set; }
        public double TheoreticalError { get; set; }
        public DerivativeMethod Method { get; set; }
        public double MaxDerivativeValue { get; private set; }
        public DataPosition Position { get; set; }

        public int k { get; set; }
        public double h { get; set; }

        public string Formula { get; set; }
        private string substitutionStep;
        private double XTarget;

        public Entity FunctionExpr { get; set; }
        
        public Derivative(double[] x, double[] y, double xTarget, DerivativeMethod method, int precision, string functionStr = null)
        {
            XData = x;
            YData = y;
            Method = method;
            XTarget = xTarget;
            
            try
            {
                FunctionExpr = (Entity)functionStr;
            }
            catch
            {
                FunctionExpr = null;
            }
            
            Solve(precision);
            
            if (FunctionExpr != null)
            {
                CalculateTheoreticalError(precision);
            }
        }
        
        private void Solve(int precision)
        {
            var y = YData;
            int n = XData.Length;
            h = XData[1] - XData[0];

            k = Array.IndexOf(XData, XTarget);
            if (k == -1) throw new ArgumentException($"Không tìm thấy {XTarget}");
            
            if (k == 0) Position = DataPosition.Start;
            else if (k == n - 1) Position = DataPosition.End;
            else Position = DataPosition.Center;

            switch (Method)
            {
                case DerivativeMethod.TwoPoints:
                    if (Position == DataPosition.Start)
                    {
                        SetFormulaInfo("(y[k+1] - y[k]) / h", $"({y[k + 1]} - {y[k]}) / {h}");
                        Result = (y[k + 1] - y[k]) / h;
                    }
                    else
                    {
                        SetFormulaInfo("(y[k] - y[k-1]) / h", $"({y[k]} - {y[k - 1]}) / {h}");
                        Result = (y[k] - y[k - 1]) / h;
                    }
                    break;
                    
                case DerivativeMethod.ThreePoints:
                    if (Position == DataPosition.Start)
                    {
                        SetFormulaInfo("(-3y[k] + 4y[k+1] - y[k+2]) / 2h",
                            $"(-3*{y[k]} + 4*{y[k + 1]} - {y[k + 2]}) / {2 * h}");
                        Result = (-3 * y[k] + 4 * y[k + 1] - y[k + 2]) / (2 * h);
                    }
                    else if (Position == DataPosition.End)
                    {
                        SetFormulaInfo("(y[k-2] - 4y[k-1] + 3y[k]) / 2h",
                            $"({y[k - 2]} - 4*{y[k - 1]} + 3*{y[k]}) / {2 * h}");
                        Result = (y[k - 2] - 4 * y[k - 1] + 3 * y[k]) / (2 * h);
                    }
                    else
                    {
                        SetFormulaInfo("(y[k+1] - y[k-1]) / 2h", $"({y[k + 1]} - {y[k - 1]}) / {2 * h}");
                        Result = (y[k + 1] - y[k - 1]) / (2 * h);
                    }
                    break;
                    
                case DerivativeMethod.FourPoints:
                    if (Position == DataPosition.Start)
                    {
                        if (k + 3 >= n) throw new ArgumentException("Không đủ dữ liệu.");
                        SetFormulaInfo("(-11y[k] + 18y[k+1] - 9y[k+2] + 2y[k+3]) / 6h",
                            $"(-11*{y[k]} + 18*{y[k + 1]} - 9*{y[k + 2]} + 2*{y[k + 3]}) / {6 * h}");
                        Result = (-11 * y[k] + 18 * y[k + 1] - 9 * y[k + 2] + 2 * y[k + 3]) / (6 * h);
                    }
                    else if (Position == DataPosition.End)
                    {
                        if (k - 3 < 0) throw new ArgumentException("Không đủ dữ liệu.");
                        SetFormulaInfo("(11y[k] - 18y[k-1] + 9y[k-2] - 2y[k-3]) / 6h",
                            $"(11*{y[k]} - 18*{y[k - 1]} + 9*{y[k - 2]} - 2*{y[k - 3]}) / {6 * h}");
                        Result = (11 * y[k] - 18 * y[k - 1] + 9 * y[k - 2] - 2 * y[k - 3]) / (6 * h);
                    }
                    else
                    {
                        if (k - 1 >= 0 && k + 2 < n)
                        {
                            SetFormulaInfo("(-2y[k-1] - 3y[k] + 6y[k+1] - y[k+2]) / 6h",
                                $"(-2*{y[k - 1]} - 3*{y[k]} + 6*{y[k + 1]} - {y[k + 2]}) / {6 * h}");
                            Result = (-2 * y[k - 1] - 3 * y[k] + 6 * y[k + 1] - y[k + 2]) / (6 * h);
                        }
                        else if (k - 2 >= 0 && k + 1 < n)
                        {
                            SetFormulaInfo("(y[k-2] - 6y[k-1] + 3y[k] + 2y[k+1]) / 6h",
                                $"({y[k - 2]} - 6*{y[k - 1]} + 3*{y[k]} + 2*{y[k + 1]}) / {6 * h}");
                            Result = (y[k - 2] - 6 * y[k - 1] + 3 * y[k] + 2 * y[k + 1]) / (6 * h);
                        }
                    }
                    break;
            }
            
            Result = Math.Round(Result, precision);
        }
        
        private void SetFormulaInfo(string expr, string subs)
        {
            Formula = expr;
            substitutionStep = subs;
        }
        private void CalculateTheoreticalError(int precision)
        {
            if (FunctionExpr == null) return;

            try
            {
                int derivativeOrder = 0;
                double errorCoefficient = 0;
                switch (Method)
                {
                    case DerivativeMethod.TwoPoints:
                        derivativeOrder = 2;
                        errorCoefficient = h / 2.0;
                        break;

                    case DerivativeMethod.ThreePoints:
                        derivativeOrder = 3;
                        if (Position == DataPosition.Center)
                        {
                            errorCoefficient = (h * h) / 6.0;
                        }
                        else
                        {
                            errorCoefficient = (h * h) / 3.0;
                        }
                        break;

                    case DerivativeMethod.FourPoints:
                        derivativeOrder = 4;
                        if (Position == DataPosition.Start || Position == DataPosition.End)
                        {
                            errorCoefficient = Math.Pow(h, 3) / 4.0;
                        }
                        else
                        {
                            errorCoefficient = Math.Pow(h, 3) / 12.0;
                        }
                        break;
                }

                Entity derivativeN = FunctionExpr;
                for (int i = 0; i < derivativeOrder; i++)
                {
                    derivativeN = derivativeN.Differentiate("x");
                }

                derivativeN = derivativeN.Simplify();

                MaxDerivativeValue = FindMaxDerivativeValue(derivativeN, derivativeOrder);

                TheoreticalError = errorCoefficient * MaxDerivativeValue;
                TheoreticalError = Math.Round(TheoreticalError, precision + 2);
            }
            catch
            {
                TheoreticalError = 0.0;
            }
        }
        private double FindMaxDerivativeValue(Entity derivativeExpr, int order)
        {
            try
            {
                var compiled = derivativeExpr.Compile("x");

                // Xác định phạm vi tìm kiếm
                int rangeMultiplier = Method == DerivativeMethod.TwoPoints ? 1 :
                                     Method == DerivativeMethod.ThreePoints ? 2 : 3;

                double left = XTarget - rangeMultiplier * h;
                double right = XTarget + rangeMultiplier * h;

                // Lấy mẫu 50 điểm
                int samples = 50;
                double step = (right - left) / samples;
                double maxValue = 0;

                for (int i = 0; i <= samples; i++)
                {
                    double x = left + i * step;
                    try
                    {
                        var result = compiled.Call(x);
                        double value = Math.Abs((double)result.Real);

                        if (!double.IsNaN(value) && !double.IsInfinity(value) && value > maxValue)
                        {
                            maxValue = value;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (maxValue < 1e-10)
                {
                    try
                    {
                        maxValue = Math.Abs((double)compiled.Call(XTarget).Real);
                    }
                    catch
                    {
                        maxValue = 1.0;
                    }
                }

                return maxValue;
            }
            catch
            {
                return 1.0;
            }
        }

        public void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("1. THÔNG TIN:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($" - Điểm cần tính: x = {XTarget} (tại chỉ số k = {k})");
            sb.AppendLine($" - Bước nhảy h:   {h}");
            sb.AppendLine($" - Vị trí:        {GetPositionName()}");
            sb.AppendLine($" - Phương pháp:   Công thức {(int)Method} điểm");
            sb.AppendLine($" - Công thức:     {Formula}\n");

            sb.AppendLine("2. CÁC BƯỚC TÍNH:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($" f'({XTarget}) ≈ {substitutionStep}");
            sb.AppendLine($"\n -> KẾT QUẢ: {Result}");

            if (FunctionExpr != null && TheoreticalError > 0)
            {
                sb.AppendLine("\n3. SAI SỐ LÝ THUYẾT:");
                sb.AppendLine("───────────────────────────────────────────────────────────────");
                sb.AppendLine($" - Hàm số: f(x) = {FunctionExpr}");
                
                try
                {
                    int order = Method == DerivativeMethod.TwoPoints ? 2 :
                               Method == DerivativeMethod.ThreePoints ? 3 : 4;
                    
                    Entity deriv = FunctionExpr;
                    for (int i = 0; i < order; i++)
                    {
                        deriv = deriv.Differentiate("x");
                    }
                    sb.AppendLine($" - Giá trị lớn nhất |f^({order})(x)| ≈ {MaxDerivativeValue}");
                }
                catch { }
                
                sb.AppendLine($" - Công thức sai số: {GetErrorFormula()}");
                sb.AppendLine($" - Sai số ước lượng: R ≈ {TheoreticalError}");
            }

            rtb.Text = sb.ToString();
        }
        
        private string GetPositionName()
        {
            switch (Position)
            {
                case DataPosition.Start: return "Đầu (Tiến)";
                case DataPosition.End: return "Cuối (Lùi)";
                case DataPosition.Center: return "Giữa (Trung tâm)";
                default: return "Không xác định";
            }
        }
        
        private string GetErrorFormula()
        {
            switch (Method)
            {
                case DerivativeMethod.TwoPoints:
                    return "R ≈ (h/2) × |f''(ξ)|";
                    
                case DerivativeMethod.ThreePoints:
                    if (Position == DataPosition.Center)
                        return "R ≈ (h²/6) × |f'''(ξ)|";
                    else
                        return "R ≈ (h²/3) × |f'''(ξ)|";
                        
                case DerivativeMethod.FourPoints:
                    if (Position == DataPosition.Start || Position == DataPosition.End)
                        return "R ≈ (h³/4) × |f⁽⁴⁾(ξ)|";
                    else
                        return "R ≈ (h³/12) × |f⁽⁴⁾(ξ)|";
                        
                default:
                    return "";
            }
        }
    }
}
