document.addEventListener("DOMContentLoaded", function () {
    const maDonHang = localStorage.getItem("maDonHangXem");
    const currentUser = JSON.parse(localStorage.getItem("currentUser"));

    if (!maDonHang || !currentUser) {
        alert("Không tìm thấy đơn hàng");
        return;
    }

    // Lấy danh sách đơn hàng của user
    const key = `donHang_${currentUser.email}`;
    const dsDonHang = JSON.parse(localStorage.getItem(key)) || [];

    // Tìm đơn hàng theo mã
    const donHang = dsDonHang.find((dh) => dh.maDonHang === maDonHang);

    if (!donHang) {
        alert("Đơn hàng không tồn tại");
        return;
    }

    //RENDER DANH SÁCH SẢN PHẨM
    // Lấy trạng thái
    const trangThai = donHang.trangThai || "Đang xử lý";

    // Render dòng thông tin đơn hàng
    document.getElementById("thongTinDonHang").innerHTML = `
    Đơn hàng <b class="text-danger">${donHang.maDonHang}</b>
    đã được đặt lúc
    <span class="text-danger">${donHang.ngayMua}</span>
    với trạng thái
    <span class="text-danger">${trangThai}</span>
`;

    const tbody = document.getElementById("dsSanPham");

    tbody.innerHTML = donHang.danhSachSanPham
        .map(
            (sp) => `
        <tr>
            <td>${sp.ten}</td>
            <td>${sp.sl}</td>
            <td class="text-danger">
                ${(sp.gia * sp.sl).toLocaleString()} đ
            </td>
        </tr>
    `
        )
        .join("");

    //THÔNG TIN THANH TOÁN
    document.getElementById("phuongThuc").innerText = donHang.phuongThucThanhToan || "Chuyển khoản ngân hàng";

    document.getElementById("tongThanhToan").innerText = donHang.tongGia.toLocaleString() + " đ";

    // ĐỊA CHỈ
    document.getElementById("diaChi").innerText = donHang.diaChi;
    document.getElementById("soDienThoai").innerText = donHang.soDienThoai;
});
