document.addEventListener("DOMContentLoaded", function () {
    const maDonHang = localStorage.getItem("maDonHangXem");
    const emailUser = localStorage.getItem("emailDonHangXem");

    if (!maDonHang || !emailUser) {
        alert("Không tìm thấy đơn hàng");
        return;
    }

    const key = `donHang_${emailUser}`;
    const dsDonHang = JSON.parse(localStorage.getItem(key)) || [];

    const donHang = dsDonHang.find((dh) => dh.maDonHang === maDonHang);

    if (!donHang) {
        alert("Đơn hàng không tồn tại");
        return;
    }

    //trạng thái
    const trangThai = donHang.trangThai || "Đang xử lý";

    document.getElementById("thongTinDonHang").innerHTML = `
        Đơn hàng <b class="text-danger">${donHang.maDonHang}</b>
        được đặt lúc <span class="text-danger">${donHang.ngayMua}</span>
        với trạng thái <span class="text-primary fw-bold">${trangThai}</span>
    `;

    // danh sách sp
    const tbody = document.getElementById("dsSanPhamAdmin");

    tbody.innerHTML = donHang.danhSachSanPham
        .map(
            (sp, index) => `
        <tr>
            <td>${index + 1}</td>
            <td>${sp.ten}</td>
            <td>${sp.gia.toLocaleString()} ₫</td>
            <td>${sp.sl}</td>
            <td class="text-danger">
                ${(sp.gia * sp.sl).toLocaleString()} ₫
            </td>
        </tr>
    `
        )
        .join("");

    // tong tien
    document.getElementById("tongThanhToan").innerText = donHang.tongGia.toLocaleString() + " ₫";

    // thong tin khach hang
    document.getElementById("emailKH").innerText = donHang.email;
    document.getElementById("hoTenKH").innerText = donHang.hoTen;
    document.getElementById("soDienThoaiKH").innerText = donHang.soDienThoai;
    document.getElementById("diaChiKH").innerText = donHang.diaChi;
    document.getElementById("ghiChuKH").innerText = donHang.ghiChu || "Không có";
});
