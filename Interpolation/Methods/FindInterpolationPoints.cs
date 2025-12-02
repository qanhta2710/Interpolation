using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public enum InterpolationPointsType
    {
        EvenlySpaced,
        ReverseInterpolation
    }

    public class FindInterpolationPoints
    {
        public double[] XData { get; private set; }
        public double[] YData { get; private set; }
        public double TargetValue { get; private set; }
        public InterpolationPointsType Type { get; private set; }
        public int K { get; private set; }
        public int CentralPointIndex { get; private set; }
        public double[] SelectedX { get; private set; }
        public double[] SelectedY { get; private set; }
        public List<IsolationInterval> IsolationIntervals { get; private set; }
        public List<MonotonicInterval> MonotonicIntervals { get; private set; }

        public class IsolationInterval
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
        }

        public class MonotonicInterval
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public bool IsIncreasing { get; set; }
            public double[] SelectedX { get; set; }
            public double[] SelectedY { get; set; }
        }

        public FindInterpolationPoints(double[] xData, double[] yData)
        {
            if (xData == null || yData == null || xData.Length == 0 || xData.Length != yData.Length)
                throw new ArgumentException("Dữ liệu đầu vào không hợp lệ");

            XData = xData;
            YData = yData;
        }

        public void FindEvenlySpacedPoints(double xTarget, int k)
        {
            Type = InterpolationPointsType.EvenlySpaced;
            TargetValue = xTarget;
            K = k;

            if (k > XData.Length)
                throw new ArgumentException($"Số mốc nội suy k = {k} lớn hơn số điểm dữ liệu {XData.Length}");

            int n = XData.Length;
            int xs = -1;
            bool isRightPoint = false;

            for (int i = 0; i < n - 1; i++)
            {
                if (xTarget >= XData[i] && xTarget <= XData[i + 1])
                {
                    double distToLeft = Math.Abs(xTarget - XData[i]);
                    double distToRight = Math.Abs(xTarget - XData[i + 1]);

                    if (distToLeft <= distToRight)
                    {
                        xs = i;
                        isRightPoint = false;
                    }
                    else
                    {
                        xs = i + 1;
                        isRightPoint = true;
                    }
                    break;
                }
            }

            if (xs == -1)
            {
                if (xTarget < XData[0] || xTarget > XData[n - 1])
                {
                    CentralPointIndex = -1;
                    SelectedX = null;
                    SelectedY = null;
                    return;
                }
            }

            CentralPointIndex = xs;
            var selectedIndices = new List<int>();
            selectedIndices.Add(xs);
            int left = xs - 1;
            int right = xs + 1;

            if (isRightPoint)
            {
                while (selectedIndices.Count < k)
                {
                    if (left >= 0 && selectedIndices.Count < k)
                    {
                        selectedIndices.Add(left);
                        left--;
                    }
                    if (right < n && selectedIndices.Count < k)
                    {
                        selectedIndices.Add(right);
                        right++;
                    }
                    if (left < 0 && right >= n)
                    {
                        break;
                    }
                }
            }
            else
            {
                while (selectedIndices.Count < k)
                {
                    if (right < n && selectedIndices.Count < k)
                    {
                        selectedIndices.Add(right);
                        right++;
                    }
                    if (left >= 0 && selectedIndices.Count < k)
                    {
                        selectedIndices.Add(left);
                        left--;
                    }
                    if (left < 0 && right >= n)
                    {
                        break;
                    }
                }
            }

            SelectedX = selectedIndices.Select(i => XData[i]).ToArray();
            SelectedY = selectedIndices.Select(i => YData[i]).ToArray();
        }

        public void FindReverseInterpolationPoints(double yTarget)
        {
            Type = InterpolationPointsType.ReverseInterpolation;
            TargetValue = yTarget;

            IsolationIntervals = FindAllIsolationIntervals(yTarget, YData);
            MonotonicIntervals = FindMonotonicIntervals(IsolationIntervals, XData, YData);
        }

        private List<IsolationInterval> FindAllIsolationIntervals(double y, double[] dataY)
        {
            int n = dataY.Length;
            var isolationIntervals = new List<IsolationInterval>();

            for (int i = 0; i < n - 1; i++)
            {
                double minY = Math.Min(dataY[i], dataY[i + 1]);
                double maxY = Math.Max(dataY[i], dataY[i + 1]);

                if (y >= minY && y <= maxY)
                {
                    isolationIntervals.Add(new IsolationInterval
                    {
                        StartIndex = i,
                        EndIndex = i + 1
                    });
                }
            }

            return isolationIntervals;
        }

        private List<MonotonicInterval> FindMonotonicIntervals(List<IsolationInterval> isolationIntervals, double[] dataX, double[] dataY)
        {
            var monotonicIntervals = new List<MonotonicInterval>();
            int n = dataY.Length;

            foreach (var isolation in isolationIntervals)
            {
                int isoStart = isolation.StartIndex;
                int isoEnd = isolation.EndIndex;

                bool isIncreasing = dataY[isoEnd] > dataY[isoStart];

                int monotonicStart = isoStart;
                while (monotonicStart > 0)
                {
                    bool prevIncreasing = dataY[monotonicStart] > dataY[monotonicStart - 1];
                    if (prevIncreasing == isIncreasing)
                    {
                        monotonicStart--;
                    }
                    else
                    {
                        break;
                    }
                }

                int monotonicEnd = isoEnd;
                while (monotonicEnd < n - 1)
                {
                    bool nextIncreasing = dataY[monotonicEnd + 1] > dataY[monotonicEnd];
                    if (nextIncreasing == isIncreasing)
                    {
                        monotonicEnd++;
                    }
                    else
                    {
                        break;
                    }
                }

                var selectedIndices = new List<int>();
                for (int idx = monotonicStart; idx <= monotonicEnd; idx++)
                {
                    selectedIndices.Add(idx);
                }

                double[] selectedX = selectedIndices.Select(idx => dataX[idx]).ToArray();
                double[] selectedY = selectedIndices.Select(idx => dataY[idx]).ToArray();

                monotonicIntervals.Add(new MonotonicInterval
                {
                    StartIndex = monotonicStart,
                    EndIndex = monotonicEnd,
                    IsIncreasing = isIncreasing,
                    SelectedX = selectedX,
                    SelectedY = selectedY
                });
            }

            return monotonicIntervals;
        }

        public void DisplayEvenlySpacedResult(RichTextBox rtb)
        {
            if (Type != InterpolationPointsType.EvenlySpaced)
                throw new InvalidOperationException("Chưa thực hiện tìm mốc nội suy cách đều");

            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════");
            sb.AppendLine($"KẾT QUẢ TÌM MỐC NỘI SUY CHO x = {TargetValue}");
            sb.AppendLine("═══════════════════════════════════════════════\n");

            sb.AppendLine($"Số mốc nội suy tìm được: {SelectedX.Length}\n");

            sb.AppendLine("Bộ điểm (x, y) được chọn:");
            sb.AppendLine("───────────────────────────────────────────────");
            sb.AppendLine(String.Format("{0,-5} {1,15} {2,15}", "STT", "x", "y"));
            sb.AppendLine("───────────────────────────────────────────────");

            for (int i = 0; i < SelectedX.Length; i++)
            {
                sb.AppendLine(String.Format("{0,-5} {1,15:G10} {2,15:G10}",
                    i + 1, SelectedX[i], SelectedY[i]));
            }

            sb.AppendLine("───────────────────────────────────────────────");
            sb.AppendLine($"\nĐiểm trung tâm xs = {XData[CentralPointIndex]:G10}");
            sb.AppendLine("═══════════════════════════════════════════════");

            rtb.Clear();
            rtb.AppendText(sb.ToString());
        }

        public void DisplayReverseInterpolationResult(RichTextBox rtb)
        {
            if (Type != InterpolationPointsType.ReverseInterpolation)
                throw new InvalidOperationException("Chưa thực hiện tìm mốc nội suy ngược");

            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine($"TÌM KHOẢNG ĐƠN ĐIỆU CHỨA CÁC KHOẢNG CÁCH LY y = {TargetValue}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            if (IsolationIntervals.Count == 0)
            {
                sb.AppendLine($"Không tìm thấy khoảng nào chứa y = {TargetValue}");
                sb.AppendLine($"Phạm vi dữ liệu: [{YData.Min():F6}, {YData.Max():F6}]");
            }
            else
            {
                sb.AppendLine($"Tìm thấy {IsolationIntervals.Count} khoảng cách ly chứa y = {TargetValue}");
                sb.AppendLine($"Tìm thấy {MonotonicIntervals.Count} khoảng đơn điệu tương ứng\n");

                for (int idx = 0; idx < MonotonicIntervals.Count; idx++)
                {
                    var isolation = IsolationIntervals[idx];
                    var monotonic = MonotonicIntervals[idx];

                    sb.AppendLine("╔═══════════════════════════════════════════════════════════════╗");
                    sb.AppendLine($"KHOẢNG {idx + 1}");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");

                    sb.AppendLine("KHOẢNG CÁCH LY:");
                    sb.AppendLine($"f(x) ∈ [{YData[isolation.StartIndex]}, {YData[isolation.EndIndex]}]");
                    sb.AppendLine($"x    ∈ [{XData[isolation.StartIndex]}, {XData[isolation.EndIndex]}]");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");

                    string monotonicType = monotonic.IsIncreasing ? "TĂNG" : "GIẢM";
                    sb.AppendLine($"KHOẢNG ĐƠN ĐIỆU ({monotonicType}):");
                    sb.AppendLine($"f(x) ∈ [{monotonic.SelectedY.Min()}, {monotonic.SelectedY.Max()}]");
                    sb.AppendLine($"x    ∈ [{monotonic.SelectedX.Min()}, {monotonic.SelectedX.Max()}]");
                    sb.AppendLine($"Số điểm: {monotonic.SelectedX.Length}");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");
                    sb.AppendLine("CÁC MỐC NỘI SUY:");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");
                    sb.AppendLine("           x                           f(x)                         ");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");

                    for (int i = 0; i < monotonic.SelectedX.Length; i++)
                    {
                        sb.AppendLine(String.Format("  {0,18:G10}      {1,18:G10}       ",
                            monotonic.SelectedX[i], monotonic.SelectedY[i]));
                    }

                    sb.AppendLine("╚═══════════════════════════════════════════════════════════════╝\n");
                }
                sb.AppendLine("- Nếu sử dụng phương pháp hàm ngược để tìm đa thức nội suy thì hãy tự chọn ra các mốc nội suy sao cho khoảng cách ly nằm giữa khoảng đơn điệu");
                sb.AppendLine("- Nếu sử dụng phương pháp lặp để tìm đa thức nội suy, hãy tự chọn ra các mốc sao cho khoảng cách ly nằm ở đầu (Nếu sử dụng Newton tiến) hoặc khoảng cách ly nằm ở cuối (Nếu sử dụng Newton lùi) của khoảng đơn điệu");
            }
            rtb.Clear();
            rtb.AppendText(sb.ToString());
        }

        public void ExportEvenlySpacedToExcel()
        {
            if (Type != InterpolationPointsType.EvenlySpaced)
                throw new InvalidOperationException("Chưa thực hiện tìm mốc nội suy cách đều");

            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");

            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.Title = "Xuất dữ liệu ra Excel";
                    saveFileDialog.FileName = $"InterpolationPoints_x_{TargetValue}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;

                        using (ExcelPackage package = new ExcelPackage())
                        {
                            string worksheetName = "Moc_Noi_Suy";
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

                            int row = 1;
                            for (int i = 0; i < SelectedX.Length; i++)
                            {
                                worksheet.Cells[row, 1].Value = SelectedX[i];
                                worksheet.Cells[row, 2].Value = SelectedY[i];
                                row++;
                            }

                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            FileInfo fileInfo = new FileInfo(filePath);
                            package.SaveAs(fileInfo);

                            MessageBox.Show(
                                $"Đã xuất thành công {SelectedX.Length} mốc nội suy ra file:\n{filePath}",
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void ExportReverseInterpolationToExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");

            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.Title = "Xuất dữ liệu ra Excel";
                    saveFileDialog.FileName = $"ReverseInterpolation_y_{TargetValue}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;

                        using (ExcelPackage package = new ExcelPackage())
                        {
                            for (int idx = 0; idx < MonotonicIntervals.Count; idx++)
                            {
                                var monotonic = MonotonicIntervals[idx];

                                string worksheetName = $"Khoang_{idx + 1}";
                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

                                int row = 1;
                                for (int i = 0; i < monotonic.SelectedX.Length; i++)
                                {
                                    worksheet.Cells[row, 1].Value = monotonic.SelectedX[i];
                                    worksheet.Cells[row, 2].Value = monotonic.SelectedY[i];
                                    row++;
                                }
                                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            }

                            FileInfo fileInfo = new FileInfo(filePath);
                            package.SaveAs(fileInfo);

                            MessageBox.Show(
                                $"Đã xuất thành công {MonotonicIntervals.Count} khoảng đơn điệu ra file:\n{filePath}",
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}