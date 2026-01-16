# Phương pháp số
## Mục đích:
- Giúp đỡ mọi người qua học phần Phương pháp số
- Source code hoàn toàn được public nên mọi người có thể dựa vào đó để viết lại thuật toán 
- Nếu có góp ý sửa lỗi hãy tạo Issues mới hoặc nếu có ý tưởng cải tiến hãy tạo Pull requests

## Tính năng:
- Tìm đa thức nội suy bằng Nội suy mốc bất kì _(Lagrange, Newton)_, Nội suy mốc cách đều _(Newton)_, Nội suy trung tâm _(Stirling, Bessel, Gauss I, Gauss II)_, Nội suy ngược _(Phương pháp lặp)_, Hàm ghép trơn _(Spline)_
- Tìm hàm thực nghiệm bằng phương pháp bình phương tối thiểu _(Dạng tuyến tính và Dạng phi tuyến có thể đưa về tuyến tính)_, tự động tịnh tiến trục tung để toàn bộ dữ liệu cùng dấu
- Tìm các mốc nội suy cách đều phù hợp cho bài toán **Nội suy trung tâm** từ tập dữ liệu, Tìm các mốc nội suy phù hợp cho bài toán **Nội suy ngược** từ tập dữ liệu
- Tính giá trị đa thức tại 1 điểm _(Sử dụng lược đồ Hoocne)_, Tính đạo hàm các cấp của đa thức tại 1 điểm, Tự động đổi biến khi sử dụng phương pháp nội suy cần đổi biến _(Newton mốc cách đều, Bessel, Stirling, ...)_
- Khi phát hiện điểm nội suy trùng nhau, tự động giữ lại điểm đầu tiên và loại bỏ tất cả các điểm còn lại
- Tự động nhập dữ liệu từ File Excel
- Tính gần đúng đạo hàm tại 1 điểm bằng **Công thức p + 1 điểm**
- Tính gần đúng tích phân tại 1 điểm bằng **Công thức hình thang**, **Công thức Simpson**, **Công thức Newton-Cotez**, **Công thức điểm giữa**
- Đánh giá sai số tính gần đúng theo công thức lưới phủ (Runge) và sai số lý thuyết
- Tìm nghiệm số, vẽ đồ thị của phương trình vi phân Cauchy bằng **Phương pháp Euler hiện**, **Euler ẩn**, **Phương pháp hình thang**, **Runge-Kutta 2, 3 ,4**, **ABs-AMs (s = 2 - 5)** 
- Giải bài toán biên, vẽ đồ thị 
- Giải bài toán trị riêng (Biên Dirichlet thuần nhất), vẽ đồ thị hàm riêng

## Hướng dẫn sử dụng - ĐÃ CŨ
- **Video đã cũ, khuyến khích mọi người tự tìm hiểu cách dùng**
[Video hướng dẫn](https://youtu.be/W5qSPlaAW-c)

## Hướng dẫn clone Source code

https://github.com/user-attachments/assets/3e09bbde-6182-4d59-b7ec-0133be25dbe1


## **Lưu ý khi sử dụng**
- Nếu báo **lỗi định dạng** thì kiểm tra lại dữ liệu nhập vào
- Khi nhập lưu ý dùng dấu **.** để biểu diễn phần thập phân
- **LUÔN LUÔN** để trống 1 dòng khi nhập giá trị
- Khi nhập hàm cơ sở trong chức năng _Phương pháp bình phương tối thiểu_, ngăn cách các hàm bằng phím Enter xuống dòng

## Tải
- Tải ở Releases hoặc ấn vào [đây](https://github.com/qanhta2710/Interpolation/releases) 

## Contributors
- Cảm ơn bác [TontonYuta](https://github.com/TontonYuta) đã giúp em có ý tưởng thực hiện làm ra App này.
- Cảm ơn bác [Tuan Anh Nguyen](https://www.facebook.com/11hnatgn) đã giúp thêm tính năng Update

## 💖 Donate

Nếu mọi người muốn ủng hộ dự án có thể donate qua STK **190127102005 (Ngân hàng MB Bank)** hoặc qua **MoMo**.

<p align="center">
  <img src="./images/MB.jpg" alt="QR MB Bank" width="220" style="border-radius:10px; margin:10px;"/>
  <img src="./images/momo.jpg" alt="QR MoMo" width="220" style="border-radius:10px; margin:10px;"/>
</p>
