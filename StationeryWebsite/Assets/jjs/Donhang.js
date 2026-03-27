document.addEventListener("DOMContentLoaded", function () {
    const tbody = document.getElementById("dsdonhang");

    //Lấy người dùng hiện tại
    const currentUser = JSON.parse(localStorage.getItem("currentUser"));

    if (!currentUser) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5" class="text-danger">
                    Vui lòng đăng nhập để xem đơn hàng
                </td>
            </tr>
        `;
        return;
    }

    //Lấy đơn hàng theo email người dùng
    const keyDonHang = `donHang_${currentUser.email}`;
    const dsDonHang = JSON.parse(localStorage.getItem(keyDonHang)) || [];

    //Nếu chưa có đơn hàng
    if (dsDonHang.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5">Bạn chưa có đơn hàng nào</td>
            </tr>
        `;
        return;
    }
    // lấy mẫu trạng thái để tạm thời khi chưa có amdin
    function layMauTrangThai(trangThai) {
        switch (trangThai) {
            case "Đã xác nhận":
                return "bg-info";
            case "Đang giao":
                return "bg-primary";
            case "Hoàn thành":
                return "bg-success";
            case "Đã hủy":
                return "bg-danger";
            default:
                return "bg-warning text-dark";
        }
    }

    // R ender đơn hàng ra bảng
    tbody.innerHTML = dsDonHang
        .map(
            (dh) => `
        <tr>
            <td>${dh.maDonHang}</td>
            <td>${dh.ngayMua}</td>
            <td>${dh.tongGia.toLocaleString()} ₫</td>
            <td>
                <span class="badge ${layMauTrangThai(dh.trangThai)}">
                    ${dh.trangThai}
                </span>
            </td>
            <td>
                <button class="btn btn-sm btnxemdonhang"
                        onclick="xemChiTiet('${dh.maDonHang}')">
                    Xem
                </button>
            </td>
        </tr>
    `
        )
        .join("");
});
function xemChiTiet(maDonHang) {
    localStorage.setItem("maDonHangXem", maDonHang);
    window.location.href = "Chitietdonhang.html";
}
