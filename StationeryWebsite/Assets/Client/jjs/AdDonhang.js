document.addEventListener("DOMContentLoaded", function () {
    const tbody = document.getElementById("dsDonHangAdmin");

    let tatCaDonHang = [];

    // lấy tất cả đơn hàng
    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);

        if (key.startsWith("donHang_")) {
            const email = key.replace("donHang_", "");
            const ds = JSON.parse(localStorage.getItem(key)) || [];

            ds.forEach((dh) => {
                tatCaDonHang.unshift({
                    ...dh,
                    emailUser: email,
                });
            });
        }
    }

    if (tatCaDonHang.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="8">Chưa có đơn hàng nào</td>
            </tr>
        `;
        return;
    }

    // RENDER BẢNG
    tbody.innerHTML = tatCaDonHang
        .map((dh, index) => {
            const trangThai = dh.trangThai || "Đang xử lý";

            return `
            <tr>
                <td>${index + 1}</td>
                <td>${dh.maDonHang}</td>
                <td>${dh.email}</td>
                <td>${dh.hoTen}</td>
                <td>${dh.ngayMua}</td>
                <td>${dh.tongGia.toLocaleString()} ₫</td>
                <td>
                    <select class="form-select form-select-sm"
                        onchange="doiTrangThai(
                            '${dh.maDonHang}',
                            '${dh.email}',
                            this.value
                        )">
                        ${renderTrangThai(trangThai)}
                    </select>
                </td>
                <td>
                    <button class="btn btn-sm btnxem"
                        onclick="xemChiTietAdmin(
                            '${dh.maDonHang}',
                            '${dh.email}'
                        )">
                        Xem
                    </button>
                </td>
            </tr>
        `;
        })
        .join("");
});

//RENDER OPTION TRẠNG THÁI
function renderTrangThai(trangThaiHienTai) {
    const dsTrangThai = ["Đang xử lý", "Đã xác nhận", "Đang giao", "Hoàn thành", "Đã hủy"];

    return dsTrangThai
        .map(
            (tt) => `
        <option value="${tt}" ${tt === trangThaiHienTai ? "selected" : ""}>
            ${tt}
        </option>
    `
        )
        .join("");
}

//ĐỔI TRẠNG THÁI
function doiTrangThai(maDonHang, emailUser, trangThaiMoi) {
    const key = `donHang_${emailUser}`;
    let ds = JSON.parse(localStorage.getItem(key)) || [];

    const index = ds.findIndex((dh) => dh.maDonHang === maDonHang);
    if (index === -1) return;

    ds[index].trangThai = trangThaiMoi;
    localStorage.setItem(key, JSON.stringify(ds));
}

//XEM CHI TIẾT
function xemChiTietAdmin(maDonHang, emailUser) {
    localStorage.setItem("maDonHangXem", maDonHang);
    localStorage.setItem("emailDonHangXem", emailUser);
    window.location.href = "AdChitietdonhang.html";
}
