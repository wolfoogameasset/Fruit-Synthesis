Hướng dẫn cài đặt IAP

Chi tiết Doc: https://drive.google.com/drive/folders/10y93rZGs8h4Jy1jZ40NZxmGnBPKCNROg?usp=share_link

Note: Lưu ý là bên App store connect cần setup các thông tin trên store trước khi submit bản build, và trạng thái IAP là ready to Submit trước thì khi submit bản build, IAP mới lên cùng được, nếu không thì cần phải build lại sau khi IAP đã ready
Setup trên Unity Dashboard
-	Dùng mail công ty đăng nhập vào https://dashboard.unity3d.com/
-	Trong trường hợp đăng nhâp vào nhưng không thấy là đang ở trong organization Sconnect thì có thể các bạn đang không dùng tài khoản của công ty, và chưa được add vào project. Nếu bạn đã sử dụng tải khoản công ty nhưng giao diện vẫn khác trên thì liên hệ với các trưởng nhóm sx
-	Chọn tab Project =>  Create Project
-	Điền tên Projetct, đồng ý game dưới 13 tuổi
-	Có thể nhấn bánh răng bên tên project để Kiểm tra setting 
 
Setup Unity Service
-	Tại tab Window=> general, chọn services  
-	Khi đã hiện được màn hình Services, bật In-App Purchasing
-	Đăng nhập Unity bằng mail công ty, nếu đã đăng nhập thì chỉ cần chờ 1 chút, chọn sử dụng 1 Unity project đã có sẵn
- 	Chọn Organizations Sconnect -> project đã tạo ở trên => Link Project ID 
-	Bật IAP lên và chờ Unity tải package về

Setup IAP Catalog
-	Trên chọn tab service -> IAP-> IAP Catalog
-	Lưu Ý: các thông tin trong IAP Catalog phải giống với trên Store và IAP Manager 
-	Id đặt theo mục đích của IAP product nhưng tốt nhất nên đặt theo package name + tác dụng product
-	Loại phụ thuộc vào loại IAP
o	Consumable (vật phẩm): hàng mua về có thể sử dụng và biến mất : item, tiền, lượt chơi, nhân vật,…
o	Non – consumable (dịch vụ): Remove Ads, đăng ký tháng, mở khóa chức năng
o	Subcription: đăng ký dịch vụ trong 1 khoảng thời gian
-	Điền title, description, giá tiền trên 2 store (lưu ý là phải điền giống giữa cả store và IAP), nếu ko điền giá dạng 2.99 đc thì cứ điền 2,99, cái này do unity, chú ý ko để quên thành 299

Setup IapManager
-	Import SCN IAP 
-	Click chuột phải vào Hierachy, chọn SCN =>IAP => tạo 1 IAP Manager ở ngoài Scene đầu và 1 Remove Ads Button ở bên trong Canvas
-	Chọn màu chủ đề phù hợp với game
-	Điền Sorting Layer của Dialog vào
-	Điền điền Product ID trong IAP Catalog vào ô ID Remove Ads
-	Kéo ảnh Iap Banner vào (1000x600)
 
Setup trên Store:
-	Google Play Console
	Để sử dụng được các setting trên Google Play Store thì cần phải up game lên với cài đặt IAP sẵn trong bản build lên trước. Chỉ cần điền các thông tin giống với những gì đã điền trong catalog
-	App Store Connect
	Khi chọn In app purchases, điền Tên và Product ID
	Nhấn vào IAP vừa tạo, chỉnh giá, Thêm app store localization (phải giống trong Catalog)
	Thêm Review information: 1 ảnh screenshot 2778x1284, viết review note
	Save
	Khi nào trạng thái chuyển thành ready to submit thì có thể submit bản build mới kèm với IAP
	Nếu khi chọn bản Build mà ko thấy có chọn IAP thì chưa nên up luôn, khi nào IAP ready thì sẽ có thể up kèm với bản build được

