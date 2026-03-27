// xử lý yêu thích
function hienThiYeuThich() {
    const ds = document.getElementById("dsYeuThich");
    let yeu = JSON.parse(localStorage.getItem("yeuThich")) || [];

    ds.innerHTML = "";

    if (yeu.length === 0) {
        ds.innerHTML = `<tr><td colspan="5">Bạn chưa có sản phẩm yêu thích nào.</td></tr>`;
        return;
    }

    yeu.forEach((sp, index) => {
        ds.innerHTML += `
            <tr>
                <td><img src="${sp.hinh}" class="anhSanPham"></td>
                <td>${sp.ten}</td>
                <td>${sp.gia.toLocaleString()} đ</td>
                <td>
                    <button class="btnThem" data-id="${sp.id}">
                        <i class="fa fa-shopping-cart"></i>
                    </button>
                </td>
                <td>
                    <button class="btnXoa" data-index="${index}">
                        <i class="fa fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
    });

    // Xóa
    document.querySelectorAll(".btnXoa").forEach((btn) => {
        btn.addEventListener("click", (e) => {
            const i = e.target.closest("button").dataset.index;
            yeu.splice(i, 1);
            localStorage.setItem("yeuThich", JSON.stringify(yeu));
            hienThiYeuThich();
        });
    });

    // Thêm giỏ hàng
    document.querySelectorAll(".btnThem").forEach((btn) => {
        btn.addEventListener("click", (e) => {
            const id = Number(e.target.closest("button").dataset.id);
            themGioHang(id);
        });
    });
}

document.addEventListener("DOMContentLoaded", hienThiYeuThich);
