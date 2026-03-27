document.addEventListener("DOMContentLoaded", () => {
    const dsSanPham = JSON.parse(localStorage.getItem("dsSanPham")) || [];

    function taoTheSanPham(sp) {
        return `
            <div class="card-sanpham">
                <img src="${sp.hinh}" class="anh-sp" alt="${sp.ten}" />
                <p class="ten-sp">${sp.ten}</p>
                <h4 class="gia-sp">Giá: ${sp.gia.toLocaleString()} đ</h4>
                <a href="../Html/TrangChiTietSanPham.html?id=${sp.id}" class="nut-xem">
                    <i class="fa-solid fa-eye"></i>
                    Xem sản phẩm
                </a>
            </div>
        `;
    }

    function hienThiTheoDanhMuc(danhMuc, idHTML) {
        let list = dsSanPham.filter((sp) => sp.danhMuc === danhMuc); // lọc sản phẩm theo danh mục
        list = list.slice(0, 4); // lấy đúng 4 sản phẩm

        const box = document.getElementById(idHTML); // tìm vị trí đưa sản p vào
        box.innerHTML = list.map(taoTheSanPham).join("");
    }
    //  hàm hiện sản phẩm nổi bậc
    function hienThiNoiBatTheoSlide(danhMuc, idHTML, slideIndex) {
        let list = dsSanPham.filter((sp) => sp.danhMuc === danhMuc);

        let batDau = slideIndex * 4;
        let ketThuc = batDau + 4;

        let slideSanPham = list.slice(batDau, ketThuc);

        const box = document.getElementById(idHTML);
        box.innerHTML = slideSanPham.map(taoTheSanPham).join("");
    }

    // Render từng danh mục
    hienThiTheoDanhMuc("Bút học sinh", "spDungCuViet");
    hienThiTheoDanhMuc("Vở học sinh", "spVoHocSinh");
    hienThiTheoDanhMuc("Dụng cụ đo - tính toán", "spDoTinhToan");
    hienThiTheoDanhMuc("Họa Phẩm", "spHoaPham");
    hienThiTheoDanhMuc("Balo", "spBalo");
    hienThiTheoDanhMuc("Sản phẩm khác", "spKhac");
    // Sản phẩm nổi bật
    hienThiNoiBatTheoSlide("Sản phẩm Nổi Bậc", "spNoiBac1", 0);
    hienThiNoiBatTheoSlide("Sản phẩm Nổi Bậc", "spNoiBac2", 1);
    hienThiNoiBatTheoSlide("Sản phẩm Nổi Bậc", "spNoiBac3", 2);
});
// đăng xuất đăng nhập
document.addEventListener("DOMContentLoaded", () => {
    const userArea = document.getElementById("userArea");
    const currentUser = JSON.parse(localStorage.getItem("currentUser"));

    // Nếu CHƯA đăng nhập  giữ nguyên Đăng ký Đăng nhập
    if (!currentUser) {
        userArea.innerHTML = `
            <a href="../Html/Dangki.html">Đăng kí</a>
            <span>|</span>
            <a href="../Html/Dangnhap.html">Đăng nhập</a>
        `;
        return;
    }

    // Nếu ĐÃ đăng nhập hiện tên dropdown đăng xuất
    userArea.innerHTML = `
        <div class="dropdown">
            <button class="btn dropdown-toggle text-capitalize tendangnhap" data-bs-toggle="dropdown">
                ${currentUser.hoten}
            </button>
            <ul class="dropdown-menu tendangxuat">
                <li><a class="dropdown-item" id="btnTaikhoan" href="../Html/Thongtinnguoidung.html">Tài khoản</a></li>
                <li><a class="dropdown-item" id="btnDonhang" href="../Html/donhang.html">Đơn hàng</a></li>
                <li><a class="dropdown-item" id="btnLogout">Đăng xuất</a></li>
            </ul>
        </div>
    `;

    // Xử lý đăng xuất
    document.getElementById("btnLogout").addEventListener("click", () => {
        localStorage.removeItem("currentUser");
        window.location.reload(); // load lại trang để hiện lại Đăng nhập | Đăng ký
    });
});
// Xử lý nút cuộn lên đầu trang
const backToTop = document.getElementById("backToTop");

// Khi cuộn xuống
window.addEventListener("scroll", () => {
    if (window.scrollY > 300) {
        backToTop.style.display = "block";
    } else {
        backToTop.style.display = "none";
    }
});

// Khi bấm nút
backToTop.addEventListener("click", () => {
    window.scrollTo({
        top: 0,
        behavior: "smooth",
    });
});
