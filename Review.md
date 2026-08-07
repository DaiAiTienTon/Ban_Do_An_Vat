1. Trang chủ, danh mục sản phẩm, chi tiết sản phẩm

Trang chủ

Banner/slider khuyến mãi (ảnh + link đến sản phẩm/danh mục)
Danh sách danh mục nổi bật (dạng icon/card: bánh kẹo, snack, đồ khô, mứt...)
Sản phẩm bán chạy / mới nhất (dạng lưới sản phẩm, ảnh + tên + giá + nút "Thêm vào giỏ")
Thanh tìm kiếm ở header

Trang danh mục sản phẩm

Breadcrumb (Trang chủ > Danh mục)
Sidebar/bộ lọc: theo danh mục con, khoảng giá, sắp xếp (giá tăng/giảm, mới nhất, bán chạy)
Lưới sản phẩm có phân trang hoặc load thêm
Mỗi item: ảnh, tên, giá (giá gốc + giá giảm nếu có), nút thêm giỏ nhanh

Trang chi tiết sản phẩm

Ảnh sản phẩm (có thể nhiều ảnh/thumbnail)
Tên, giá, mô tả, thông tin (trọng lượng, hạn sử dụng, xuất xứ...)
Chọn số lượng + nút "Thêm vào giỏ" / "Mua ngay"
Sản phẩm liên quan (cùng danh mục)
2. Đặt hàng (giỏ hàng, đặt hàng, theo dõi đơn)

Giỏ hàng

Danh sách sản phẩm đã thêm: ảnh, tên, đơn giá, số lượng (tăng/giảm/xoá), thành tiền
Tổng tiền tạm tính
Nút "Tiến hành đặt hàng"

Đặt hàng (checkout)

Form thông tin giao hàng: họ tên, SĐT, địa chỉ, ghi chú
Chọn phương thức thanh toán (COD / chuyển khoản / ví điện tử)
Xem lại đơn hàng trước khi xác nhận
Xác nhận đặt hàng → tạo đơn, hiển thị mã đơn hàng

Theo dõi trạng thái đơn

Trang "Đơn hàng của tôi" (yêu cầu đăng nhập)
Danh sách đơn hàng với trạng thái: Chờ xác nhận → Đang xử lý → Đang giao → Đã giao → Đã huỷ
Xem chi tiết từng đơn (sản phẩm, tổng tiền, địa chỉ, trạng thái)
3. Thanh toán cơ bản

COD (thanh toán khi nhận hàng)

Chọn khi checkout, không cần xử lý gì thêm ngoài ghi nhận vào đơn hàng

Chuyển khoản / ví điện tử đơn giản

Hiển thị thông tin chuyển khoản (số tài khoản/ tên ngân hàng) hoặc QR code tĩnh
Khách tự chuyển khoản, nhập/upload mã giao dịch hoặc ảnh chụp biên lai (tuỳ bạn quyết định có cần bước này không)
Đơn hàng ở trạng thái "Chờ xác nhận thanh toán" cho đến khi admin xác nhận thủ công

(Lưu ý: đây là hình thức xác nhận thủ công, không phải tích hợp cổng thanh toán tự động như VNPay/Momo API — nếu bạn cần cổng thanh toán thật, đó là việc khác cần tích hợp riêng)

4. Trang quản trị (Admin)

Quản lý sản phẩm

Danh sách sản phẩm (tìm kiếm, lọc theo danh mục)
Thêm/sửa/xoá sản phẩm: tên, giá, mô tả, ảnh, danh mục, tồn kho
Quản lý danh mục sản phẩm (thêm/sửa/xoá danh mục)

Quản lý đơn hàng

Danh sách đơn hàng (lọc theo trạng thái, tìm theo mã đơn/SĐT khách)
Xem chi tiết đơn, cập nhật trạng thái đơn
Xác nhận thanh toán (với đơn chuyển khoản/ví điện tử)

Quản lý khách hàng

Danh sách tài khoản khách hàng
Xem thông tin, lịch sử đơn hàng của từng khách
Khoá/mở khoá tài khoản (nếu cần)
5. Đăng ký / đăng nhập
Đăng ký: họ tên, email hoặc SĐT, mật khẩu
Đăng nhập: email/SĐT + mật khẩu
Quên mật khẩu (đặt lại qua email/SĐT)
Trang thông tin tài khoản: sửa thông tin cá nhân, đổi mật khẩu, xem địa chỉ giao hàng đã lưu
6. Tìm kiếm, lọc sản phẩm
Ô tìm kiếm theo tên sản phẩm (header, có thể có gợi ý khi gõ)
Lọc theo: danh mục, khoảng giá (min-max), có thể thêm sắp xếp (giá, mới nhất, bán chạy)
Kết hợp tìm kiếm + lọc trên cùng trang danh mục/kết quả tìm kiếm
7. Giao diện responsive
Bố cục dạng lưới co giãn (grid/flexbox) theo breakpoint: mobile, tablet, desktop
Trên mobile: menu dạng hamburger, giỏ hàng dạng icon nổi hoặc trang riêng, lưới sản phẩm 2 cột
Ảnh sản phẩm tối ưu kích thước để tải nhanh trên di động

A. Điều kiện & luồng để ĐẶT HÀNG được

1. Có bắt buộc đăng nhập mới được đặt hàng không?
Đây là quyết định thiết kế bạn cần chốt trước — có 2 hướng phổ biến:

Hướng 1: Bắt buộc đăng nhập mới cho đặt hàng
→ Khách bấm "Đặt hàng" mà chưa đăng nhập → hệ thống chuyển sang trang đăng nhập/đăng ký → sau khi login thành công quay lại giỏ hàng để tiếp tục checkout
Hướng 2: Cho phép đặt hàng dạng khách (guest checkout)
→ Không cần tài khoản, chỉ cần nhập họ tên + SĐT + địa chỉ ở bước checkout
→ Nhưng sẽ không xem được "Đơn hàng của tôi" trừ khi đăng ký sau, hoặc tra cứu đơn bằng mã đơn + SĐT

Tôi cần bạn chọn 1 trong 2 hướng này để tôi cụ thể hoá tiếp — vì nó quyết định toàn bộ luồng checkout và cấu trúc database (đơn hàng có gắn user_id bắt buộc hay để null được).

2. Điều kiện để nút "Đặt hàng" được phép bấm (submit thành công)

Dù chọn hướng nào ở trên, đơn hàng chỉ được tạo khi tất cả điều kiện sau đúng:

Điều kiện	Mô tả
Giỏ hàng không rỗng	Có ít nhất 1 sản phẩm trong giỏ
Sản phẩm còn tồn kho	Số lượng đặt ≤ số lượng tồn kho tại thời điểm đặt (kiểm tra lại ở server, không chỉ tin phía client)
Thông tin giao hàng hợp lệ	Họ tên, SĐT (đúng định dạng), địa chỉ không được để trống
Đã chọn phương thức thanh toán	Bắt buộc chọn COD hoặc chuyển khoản/ví trước khi xác nhận
(Nếu bắt buộc đăng nhập) Có phiên đăng nhập hợp lệ	Token/session còn hiệu lực

3. Sau khi bấm "Xác nhận đặt hàng" — hệ thống làm gì (theo thứ tự)

Server kiểm tra lại tồn kho từng sản phẩm trong giỏ (tránh trường hợp 2 người đặt cùng lúc hết hàng)
Nếu đủ hàng → tạo bản ghi đơn hàng (mã đơn, danh sách sản phẩm, tổng tiền, thông tin giao hàng, trạng thái = "Chờ xác nhận")
Trừ tồn kho tương ứng
Xoá giỏ hàng của khách
Trả về trang xác nhận + mã đơn hàng
(Nếu chọn chuyển khoản/ví) → chuyển sang trạng thái "Chờ xác nhận thanh toán" thay vì "Chờ xác nhận"

Nếu bước 1 phát hiện hết hàng → báo lỗi ngay, không tạo đơn, yêu cầu khách sửa số lượng.

B. Điều kiện & luồng để VÀO TRANG ADMIN

Trang admin không phải là một trang công khai — nó phải được kiểm soát bằng phân quyền tài khoản (role-based access). Cụ thể:

1. Phân biệt loại tài khoản

Trong bảng người dùng (users), cần có 1 trường xác định vai trò, ví dụ:

role = "customer" (mặc định khi khách tự đăng ký)
role = "admin"    (chỉ được gán thủ công, không ai tự đăng ký thành admin được)

2. Tài khoản admin được tạo ra bằng cách nào

Không có form đăng ký admin công khai trên website — đây là nguyên tắc bắt buộc để tránh ai cũng tạo được tài khoản admin
Tài khoản admin được tạo theo 1 trong các cách:
Tạo sẵn thủ công trong database khi triển khai hệ thống (seed data)
Hoặc admin hiện tại vào trang quản trị → mục "Quản lý nhân viên/tài khoản" → tạo tài khoản admin mới cho người khác

3. Luồng kiểm soát truy cập khi vào /admin

Khách truy cập URL /admin (hoặc bất kỳ route nào thuộc khu vực quản trị)
Hệ thống kiểm tra: đã đăng nhập chưa?
Chưa đăng nhập → chuyển hướng về trang đăng nhập admin (có thể dùng chung hoặc tách riêng trang login admin)
Nếu đã đăng nhập → kiểm tra: role của tài khoản có phải admin không?
Nếu role = "customer" → từ chối truy cập (trả về lỗi 403 / chuyển hướng về trang chủ), dù họ đã đăng nhập thành công với vai trò khách hàng
Nếu role = "admin" → cho vào trang quản trị
Việc kiểm tra này phải thực hiện ở backend/server (middleware bảo vệ route), không chỉ ẩn menu ở giao diện — vì nếu chỉ ẩn ở frontend, khách hàng vẫn có thể gõ thẳng URL để vào được nếu server không chặn

4. Tóm tắt bảng phân quyền

Vai trò	Đặt hàng	Xem đơn hàng của mình	Vào trang admin
Khách chưa đăng nhập	Tuỳ theo hướng bạn chọn ở mục A	Không	Không
Khách hàng (customer)	Có	Có (đơn của chính họ)	Không
Admin	Có thể có hoặc không cần (tuỳ bạn)	—	Có, và thấy toàn bộ đơn của mọi khách hàng

Bạn cho tôi biết: bạn chọn bắt buộc đăng nhập mới đặt hàng được hay cho đặt hàng kiểu khách (guest)? Tôi sẽ cụ thể hoá tiếp phần database schema và luồng API dựa trên lựa chọn đó.