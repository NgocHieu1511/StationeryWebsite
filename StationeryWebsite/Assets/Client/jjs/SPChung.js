const ITEM_PER_PAGE = 16;
let currentPage = 1;
let dsDangHienThi = null; // null là chưa lọc
function layDSSanPham() {
    let ds = JSON.parse(localStorage.getItem("dsSanPham")) || [];
    return ds;
}
function hienThiSanPham() {
    const box = document.getElementById("listSanPham");

    // nếu đã lọc thigdùng dsDangHienThi kể cả nó rỗng
    let ds = dsDangHienThi !== null ? dsDangHienThi : layDSSanPham();

    // KHÔNG CÓ SẢN PHẨM
    if (ds.length === 0) {
        box.innerHTML = `
            <div class="col-12 text-center mt-4">
                <p style="font-size:18px;color:#777">
                    Không có sản phẩm phù hợp với mức giá đã chọn
                </p>
            </div>
        `;
        renderPagination(0);
        return;
    }

    let start = (currentPage - 1) * ITEM_PER_PAGE;
    let end = start + ITEM_PER_PAGE;
    let dsTrang = ds.slice(start, end);

    box.innerHTML = "";

    dsTrang.forEach((sp) => {
        box.innerHTML += `
            <div class="col-6 col-md-3 san-pham" data-id="${sp.id}">
                <div class="card-sanpham">
                    <i class="fa-solid fa-star favorite-icon" data-id="${sp.id}"></i>
                    <a href="TrangChiTietSanPham.html?id=${sp.id}">
                        <img src="${sp.hinh}" class="anh-sp" alt="${sp.ten}" />
                    </a>
                    <p class="ten-sp">${sp.ten}</p>
                    <h4 class="gia-sp">Giá: ${Number(sp.gia).toLocaleString()} đ</h4>
                    <a href="#" class="nut-them" onclick="themGioHang(${sp.id})">
                        Thêm giỏ hàng
                    </a>
                </div>
            </div>
        `;
    });

    renderPagination(ds.length);
}

function renderPagination(totalItem) {
    const pagination = document.getElementById("pagination");
    let totalPage = Math.ceil(totalItem / ITEM_PER_PAGE);

    pagination.innerHTML = "";

    // <
    let prev = document.createElement("a");
    prev.href = "#";
    prev.innerHTML = "&lt;";
    prev.className = "nut";
    if (currentPage === 1) prev.classList.add("disabled");
    prev.onclick = () => changePage(currentPage - 1);
    pagination.appendChild(prev);

    // số trang
    for (let i = 1; i <= totalPage; i++) {
        let btn = document.createElement("a");
        btn.href = "#";
        btn.innerText = i;
        btn.className = "nut";
        if (i === currentPage) btn.classList.add("active");
        btn.onclick = () => changePage(i);
        pagination.appendChild(btn);
    }

    // >
    let next = document.createElement("a");
    next.href = "#";
    next.innerHTML = "&gt;";
    next.className = "nut";
    if (currentPage === totalPage) next.classList.add("disabled");
    next.onclick = () => changePage(currentPage + 1);
    pagination.appendChild(next);
}
function changePage(page) {
    let ds = dsDangHienThi !== null ? dsDangHienThi : layDSSanPham();
    const totalPage = Math.ceil(ds.length / ITEM_PER_PAGE);

    if (page < 1 || page > totalPage) return;

    currentPage = page;
    hienThiSanPham();
}
// hàm lọc theo giá
document.querySelectorAll(".loc-gia").forEach((cb) => {
    cb.addEventListener("change", () => {
        let dsGoc = layDSSanPham();
        let checked = document.querySelectorAll(".loc-gia:checked");

        if (checked.length === 0) {
            dsDangHienThi = null; //  chưa lọc
        } else {
            dsDangHienThi = dsGoc.filter((sp) =>
                [...checked].some((c) => {
                    let min = Number(c.dataset.min);
                    let max = Number(c.dataset.max);
                    return Number(sp.gia) >= min && Number(sp.gia) <= max;
                })
            );
        }

        currentPage = 1;
        hienThiSanPham();
    });
});
// render sản phẩm nổi bậc
function renderSanPhamNoiBat() {
    const box = document.getElementById("sanPhamNoiBat");
    if (!box) return;

    let ds = JSON.parse(localStorage.getItem("dsSanPham")) || [];

    // lọc danh mục "Sản phẩm nổi bật" và lấy 3 sản phẩm đầu tiên
    let dsNoiBat = ds.filter((sp) => sp.danhMuc === "Sản phẩm Nổi Bậc").slice(0, 3);

    box.innerHTML = "";

    dsNoiBat.forEach((sp) => {
        box.innerHTML += `
            <li class="product-item">
                <a href="TrangChiTietSanPham.html?id=${sp.id}" style="display:flex; gap:10px; text-decoration:none; color:inherit">
                    <img src="${sp.hinh}" alt="${sp.ten}" />
                    <div class="info">
                        <p class="p-name">${sp.ten}</p>
                        <p class="p-price">${sp.gia.toLocaleString()}₫</p>
                    </div>
                </a>
            </li>
        `;
    });
}
// 3. Thêm sản phẩm vào giỏ hàng
function themGioHang(id) {
    let gioh = JSON.parse(localStorage.getItem("gioHang")) || [];
    let ds = layDSSanPham();
    let sp = ds.find((x) => x.id === id);

    let index = gioh.findIndex((x) => x.id === id);

    if (index === -1) {
        gioh.push({ id: sp.id, ten: sp.ten, gia: sp.gia, hinh: sp.hinh, sl: 1 });
    } else {
        gioh[index].sl++;
    }

    localStorage.setItem("gioHang", JSON.stringify(gioh));
    alert("Đã thêm vào giỏ hàng!");
}

// xử lý yêu thích 2
document.addEventListener("click", function (e) {
    if (e.target.classList.contains("favorite-icon")) {
        const id = Number(e.target.dataset.id);
        let ds = JSON.parse(localStorage.getItem("dsSanPham")) || [];
        let yeu = JSON.parse(localStorage.getItem("yeuThich")) || [];

        let sp = ds.find((sp) => sp.id === id);
        if (!sp) return;

        let index = yeu.findIndex((x) => x.id === id);

        if (index === -1) {
            yeu.push({
                id: sp.id,
                ten: sp.ten,
                gia: sp.gia,
                hinh: sp.hinh,
            });
            e.target.classList.add("active");
        } else {
            yeu.splice(index, 1);
            e.target.classList.remove("active");
        }

        localStorage.setItem("yeuThich", JSON.stringify(yeu));
    }
});
function activeYeuThichIcon() {
    let yeu = JSON.parse(localStorage.getItem("yeuThich")) || [];

    document.querySelectorAll(".favorite-icon").forEach((icon) => {
        let id = Number(icon.dataset.id);
        if (yeu.some((x) => x.id === id)) {
            icon.classList.add("active");
        }
    });
}

document.addEventListener("DOMContentLoaded", activeYeuThichIcon);
document.addEventListener("DOMContentLoaded", () => {
    hienThiSanPham();
    renderSanPhamNoiBat();
    activeYeuThichIcon();
});
//
document.addEventListener("DOMContentLoaded", () => {
    let tuKhoa = localStorage.getItem("tuKhoaTimKiem");

    if (tuKhoa) {
        let ds = layDSSanPham();

        let tuKhoaKhongDau = removeDiacritics(tuKhoa.toLowerCase());

        dsDangHienThi = ds.filter((sp) => removeDiacritics(sp.ten.toLowerCase()).includes(tuKhoaKhongDau));

        currentPage = 1;

        // Xóa từ khóa sau khi dùng tránh ảnh hưởng lọc khác
        localStorage.removeItem("tuKhoaTimKiem");
    }

    hienThiSanPham();
    renderSanPhamNoiBat();
    activeYeuThichIcon();
});

function removeDiacritics(str) {
    return str
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .replace(/đ/g, "d")
        .replace(/Đ/g, "D");
}
