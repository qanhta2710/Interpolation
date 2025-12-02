using Interpolation.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public enum IterationDirection
    {
        Forward,
        Backward
    }

    public class NewtonIteration
    {
        public IterationDirection Direction { get; private set; }
        public double YTarget { get; private set; }
        public double Result { get; private set; }
        public double?[,] DiffTable { get; private set; }
        public List<IterationStep> IterationSteps { get; private set; }
        public int Precision { get; private set; }
        public double[] XValues { get; private set; }
        public double[] YValues { get; private set; }

        public class IterationStep
        {
            public int Iteration { get; set; }
            public double TPrev { get; set; }
            public double TN { get; set; }
            public double Sum { get; set; }
            public double Error { get; set; }
            public List<StepDetail> Details { get; set; }
        }

        public class StepDetail
        {
            public int R { get; set; }
            public double DeltaR { get; set; }
            public double Factorial { get; set; }
            public double Prod { get; set; }
            public double Term { get; set; }
        }

        public NewtonIteration(double[] x, double[] y, double yTarget, IterationDirection direction, int precision)
        {
            if (x == null || y == null || x.Length == 0)
                throw new ArgumentException("Dữ liệu đầu vào không hợp lệ");

            XValues = x;
            YValues = y;
            YTarget = yTarget;
            Direction = direction;
            Precision = precision;

            Solve(x, y, yTarget, direction, precision);
        }

        private void Solve(double[] x, double[] y, double yTarget, IterationDirection direction, int precision)
        {
            DiffTable = DifferenceTable.BuildFiniteDifferenceTable(x, y, precision);

            List<IterationStep> steps;
            if (direction == IterationDirection.Forward)
            {
                Result = SolveForward(x, y, yTarget, precision, DiffTable, out steps);
            }
            else
            {
                Result = SolveBackward(x, y, yTarget, precision, DiffTable, out steps);
            }
            IterationSteps = steps;
        }

        private static double SolveForward(double[] x, double[] y, double yTarget, int precision,
            double?[,] diffTable, out List<IterationStep> iterationSteps)
        {
            iterationSteps = new List<IterationStep>();

            double h = Math.Round(x[1] - x[0], precision);
            double y0 = y[0];
            double delta1 = diffTable[1, 2] ?? 0.0;

            double t_prev, t_n = Math.Round((yTarget - y0) / delta1, precision);
            double epsilon = 1e-6;
            int maxIterations = 1000;
            int iteration = 0;

            iterationSteps.Add(new IterationStep
            {
                Iteration = 0,
                TPrev = 0,
                TN = t_n,
                Sum = 0,
                Error = 0,
                Details = new List<StepDetail>()
            });

            do
            {
                t_prev = t_n;
                double sum = 0.0;
                var stepDetails = new List<StepDetail>();

                for (int r = 2; r < x.Length; r++)
                {
                    double deltaR = diffTable[r, r + 1] ?? 0.0;
                    double factorial = Function.Factorial(r);

                    double prod = 1.0;
                    for (int i = 0; i < r; i++)
                    {
                        prod *= (t_prev - i);
                    }

                    double term = Math.Round((deltaR / factorial) * prod, precision);
                    sum += term;

                    stepDetails.Add(new StepDetail
                    {
                        R = r,
                        DeltaR = deltaR,
                        Factorial = factorial,
                        Prod = prod,
                        Term = term
                    });
                }

                t_n = Math.Round(((yTarget - y0) / delta1) - (sum / delta1), precision);
                double error = Math.Abs(t_n - t_prev);

                iterationSteps.Add(new IterationStep
                {
                    Iteration = iteration + 1,
                    TPrev = t_prev,
                    TN = t_n,
                    Sum = sum,
                    Error = error,
                    Details = stepDetails
                });

                iteration++;

                if (iteration > maxIterations) break;

            } while (Math.Abs(t_n - t_prev) >= epsilon);

            double result = x[0] + h * t_n;
            return result;
        }

        private static double SolveBackward(double[] x, double[] y, double yTarget, int precision,
            double?[,] diffTable, out List<IterationStep> iterationSteps)
        {
            iterationSteps = new List<IterationStep>();

            int n = x.Length;
            double h = Math.Round(x[1] - x[0], precision);
            double y_n_minus_1 = y[n - 1];
            double x_n_minus_1 = x[n - 1];
            double nabla1 = diffTable[n - 2, 2] ?? 0.0;

            double t_prev, t_n = Math.Round((yTarget - y_n_minus_1) / nabla1, precision);
            double epsilon = 1e-6;
            int maxIterations = 1000;
            int iteration = 0;

            iterationSteps.Add(new IterationStep
            {
                Iteration = 0,
                TPrev = 0,
                TN = t_n,
                Sum = 0,
                Error = 0,
                Details = new List<StepDetail>()
            });

            do
            {
                t_prev = t_n;
                double sum = 0.0;
                var stepDetails = new List<StepDetail>();

                for (int r = 2; r < n; r++)
                {
                    double deltaR = diffTable[n - 1 - r, r + 1] ?? 0.0;
                    double factorial = Function.Factorial(r);

                    double prod = 1.0;
                    for (int i = 0; i < r; i++)
                    {
                        prod *= (t_prev + i);
                    }

                    double term = Math.Round((deltaR / factorial) * prod, precision);
                    sum += term;

                    stepDetails.Add(new StepDetail
                    {
                        R = r,
                        DeltaR = deltaR,
                        Factorial = factorial,
                        Prod = prod,
                        Term = term
                    });
                }

                t_n = Math.Round(((yTarget - y_n_minus_1) / nabla1) - (sum / nabla1), precision);
                double error = Math.Abs(t_n - t_prev);

                iterationSteps.Add(new IterationStep
                {
                    Iteration = iteration + 1,
                    TPrev = t_prev,
                    TN = t_n,
                    Sum = sum,
                    Error = error,
                    Details = stepDetails
                });

                iteration++;

                if (iteration > maxIterations) break;

            } while (Math.Abs(t_n - t_prev) >= epsilon);

            double result = x_n_minus_1 + h * t_n;
            return result;
        }

        public void DisplayResults(DataGridView dgv, RichTextBox rtb)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            DataGridViewHelper.SetupColumns(dgv, DiffTable.GetLength(1), "Ghi chú");
            DataGridViewHelper.AddNullableTable(dgv, DiffTable, "Bảng tỷ sai phân");

            rtb.Clear();

            if (Direction == IterationDirection.Forward)
            {
                DisplayForwardIterationProcess(rtb);
            }
            else
            {
                DisplayBackwardIterationProcess(rtb);
            }
        }

        private void DisplayForwardIterationProcess(RichTextBox rtb)
        {
            double h = XValues[1] - XValues[0];
            double y0 = YValues[0];
            double delta1 = DiffTable[1, 2] ?? 0.0;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("QUÁ TRÌNH LẶP TIẾN (NEWTON FORWARD)");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            sb.AppendLine("Dữ liệu ban đầu:");
            sb.AppendLine($"  h = {h}");
            sb.AppendLine($"  y₀ = {y0}");
            sb.AppendLine($"  y_target = {YTarget}");
            sb.AppendLine($"  Δy₀ = {delta1}");
            sb.AppendLine($"  epsilon = 1e-6");
            sb.AppendLine($"  Max iterations = 1000\n");

            foreach (var step in IterationSteps)
            {
                if (step.Iteration == 0)
                {
                    sb.AppendLine("KHỞI TẠO:");
                    sb.AppendLine("───────────────────────────────────────────────────────────────");
                    sb.AppendLine($"t₀ = (y_target - y₀) / Δy₀ = ({YTarget} - {y0}) / {delta1} = {step.TN}\n");
                }
                else
                {
                    sb.Append($"t_{step.Iteration} = φ(t_{step.Iteration - 1}) = {(YTarget - y0) / delta1}");

                    if (step.Details.Count > 0)
                    {
                        sb.Append($" - (1/{delta1}) × [");

                        var terms = new List<string>();
                        foreach (var detail in step.Details)
                        {
                            var prodTerms = new List<string>();
                            for (int i = 0; i < detail.R; i++)
                            {
                                prodTerms.Add($"({step.TPrev} - {i})");
                            }

                            string termStr = $"({detail.DeltaR}/{detail.Factorial}) × {string.Join(" × ", prodTerms)}";
                            terms.Add(termStr);
                        }

                        sb.Append(string.Join(" + ", terms));
                        sb.Append("]");
                    }

                    sb.AppendLine($" = {step.TN}");
                    sb.AppendLine($"  → Sai số = |t_{step.Iteration} - t_{step.Iteration - 1}| = {step.Error}\n");
                }
            }

            var lastStep = IterationSteps[IterationSteps.Count - 1];
            if (lastStep.Iteration >= 1000)
            {
                sb.AppendLine("⚠ Đã đạt số vòng lặp tối đa: 1000\n");
            }

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ CUỐI CÙNG:");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine($"Số vòng lặp: {lastStep.Iteration}");
            sb.AppendLine($"t_final = {lastStep.TN}");
            sb.AppendLine($"\nTính giá trị x:");
            sb.AppendLine($"x = x₀ + h × t_n = {XValues[0]} + {h} × {lastStep.TN} = {Result}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");

            rtb.AppendText(sb.ToString());
        }

        private void DisplayBackwardIterationProcess(RichTextBox rtb)
        {
            int n = XValues.Length;
            double h = XValues[1] - XValues[0];
            double y_n_minus_1 = YValues[n - 1];
            double x_n_minus_1 = XValues[n - 1];
            double nabla1 = DiffTable[n - 2, 2] ?? 0.0;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("QUÁ TRÌNH LẶP LÙI (NEWTON BACKWARD)");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            sb.AppendLine("Dữ liệu ban đầu:");
            sb.AppendLine($"  h = {h}");
            sb.AppendLine($"  x_{{n-1}} = {x_n_minus_1}");
            sb.AppendLine($"  y_{{n-1}} = {y_n_minus_1}");
            sb.AppendLine($"  y_target = {YTarget}");
            sb.AppendLine($"  ∇y_{{n-1}} = {nabla1}");
            sb.AppendLine($"  epsilon = 1e-6");
            sb.AppendLine($"  Max iterations = 1000\n");

            foreach (var step in IterationSteps)
            {
                if (step.Iteration == 0)
                {
                    sb.AppendLine("KHỞI TẠO:");
                    sb.AppendLine("───────────────────────────────────────────────────────────────");
                    sb.AppendLine($"t₀ = (y_target - y_{{n-1}}) / ∇y_{{n-1}} = ({YTarget} - {y_n_minus_1}) / {nabla1} = {step.TN}\n");
                }
                else
                {
                    sb.Append($"t_{step.Iteration} = φ(t_{step.Iteration - 1}) = {(YTarget - y_n_minus_1) / nabla1}");

                    if (step.Details.Count > 0)
                    {
                        sb.Append($" - (1/{nabla1}) × [");

                        var terms = new List<string>();
                        foreach (var detail in step.Details)
                        {
                            var prodTerms = new List<string>();
                            for (int i = 0; i < detail.R; i++)
                            {
                                prodTerms.Add($"({step.TPrev} + {i})");
                            }

                            string termStr = $"({detail.DeltaR}/{detail.Factorial}) × {string.Join(" × ", prodTerms)}";
                            terms.Add(termStr);
                        }

                        sb.Append(string.Join(" + ", terms));
                        sb.Append("]");
                    }

                    sb.AppendLine($" = {step.TN}");
                    sb.AppendLine($"  → Sai số = |t_{step.Iteration} - t_{step.Iteration - 1}| = {step.Error}\n");
                }
            }

            var lastStep = IterationSteps[IterationSteps.Count - 1];
            if (lastStep.Iteration >= 1000)
            {
                sb.AppendLine("⚠ Đã đạt số vòng lặp tối đa: 1000\n");
            }

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ CUỐI CÙNG:");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine($"Số vòng lặp: {lastStep.Iteration}");
            sb.AppendLine($"t_final = {lastStep.TN}");
            sb.AppendLine($"\nTính giá trị x:");
            sb.AppendLine($"x = x_{{n-1}} + h × t_n = {x_n_minus_1} + {h} × {lastStep.TN} = {Result}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");

            rtb.AppendText(sb.ToString());
        }
    }
}