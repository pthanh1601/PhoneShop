# Cách chạy Code 
  ## 1 Cài 3 Thư Viện này
     ![image](https://github.com/<username>/<repository>/blob/main/<path_to_image>?raw=true)

  ## 2 Kết Nối SQL 
    ![image](https://github.com/user-attachments/assets/da6688b4-be36-469f-90e9-5ff715e5265c)
  
    ![image](https://github.com/user-attachments/assets/e3944548-fed0-4126-9e43-88f317f52f87)

  ## 3 Chạy Câu lệnh Package Manager Console
    ![image](https://github.com/user-attachments/assets/ecc2fda9-80df-49cf-89fe-67fd188741c9)

      Scaffold-DbContext "Data Source=LAPTOP-B8CQH803\ALINSBTC;Initial Catalog=Hshop2023;Integrated Security=True;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data -f
 
  ## 4 Sữa lại file appsettings.json
    ![image](https://github.com/user-attachments/assets/1b5f363f-60ac-4d48-9c0e-75ca21bd3fd2)



Quy tắc đặt tên nhánh
  #  Không Push Code Trực Tiếp Vào Nhánh Developer AND master:
-      Tất cả các thành viên trong nhóm không được phép push code trực tiếp vào nhánh developer.

  #  Sử Dụng Nhánh Riêng Để Phát Tri
    - Mỗi thành viên trong nhóm nên tạo một nhánh riêng cho các tính năng hoặc sửa lỗi mà họ đang làm việc. 
    -    Tên nhánh nên có định dạng như sau:
    +    feature/<tên-tính-năng> cho các tính năng mới.
    +    bugfix/<tên-sửa-lỗi> cho các sửa lỗi.
    +    hotfix/<tên-sửa-lỗi-nghiêm-trọng> cho các sửa lỗi khẩn cấp.
PHÂN CÔNG
* Trang admin
  - Thoại: Quản lý hóa đơn, chi tiết hóa đơn
  - Ngọc Thành: Quản lý nhân viên, phân quyền
  - Lộc: Login, logout
  - Quang: Quản lý khách hàng
  - Anh Cọp: Quản lý nhà cung cấp

DEADLINE: 00:00 4/11 
