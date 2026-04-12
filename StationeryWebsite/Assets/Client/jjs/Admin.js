// đếm số tài khoản đăng kí của người dùng với admin
document.addEventListener("DOMContentLoaded", () => {
    // Lấy dữ liệu accounts
    let accounts = JSON.parse(localStorage.getItem("accounts")) || [];

    // Tổng tài khoản (bao gồm admin + user)
    let totalAccount = accounts.length;

    // Ghi vào giao diện
    document.querySelector(".stat-card .number").textContent = totalAccount;
});

//  đếm số liên hệ
document.addEventListener("DOMContentLoaded", () => {
    // Lấy danh sách liên hệ từ LocalStorage
    let dsLienHe = JSON.parse(localStorage.getItem("dsLienHe")) || [];

    // Đếm số liên hệ
    let lienHeCount = dsLienHe.length;

    // Hiển thị vào ô liên hệ
    document.getElementById("soluonglienhe").textContent = lienHeCount;
});
// phần hiện sản phẩm
function loadSanPhamAdmin() {
    // Lấy danh sách sản phẩm từ LocalStorage
    let ds = JSON.parse(localStorage.getItem("dsSanPham")) || [];

    // Lấy 5 sản phẩm đầu tiên
    let top5 = ds.slice(0, 5);

    const tbody = document.getElementById("tableBody");
    tbody.innerHTML = ""; // xoá sạch trước

    top5.forEach((sp) => {
        const tr = document.createElement("tr");

        tr.innerHTML = `
            <td><img src="${sp.hinh}" class="prod-img" /></td>
            <td>${sp.ten}</td>
            <td>${Number(sp.gia).toLocaleString()} đ</td>
        `;

        tbody.appendChild(tr);
    });
}
// thong ke luot mua san pham

// LẤY TẤT CẢ ĐƠN HÀNG
function layTatCaDonHang() {
    let tatCaDon = [];

    for (let key in localStorage) {
        if (key.startsWith("donHang_")) {
            let don = JSON.parse(localStorage.getItem(key)) || [];
            tatCaDon = tatCaDon.concat(don);
        }
    }
    return tatCaDon;
}

// THỐNG KÊ LƯỢT MUA
function thongKeLuotMua() {
    let donHangs = layTatCaDonHang();
    let thongKe = {};

    donHangs.forEach((don) => {
        don.danhSachSanPham.forEach((sp) => {
            if (!thongKe[sp.id]) {
                thongKe[sp.id] = {
                    id: sp.id,
                    ten: sp.ten,
                    luotMua: 0,
                };
            }
            thongKe[sp.id].luotMua += sp.sl;
        });
    });

    return Object.values(thongKe);
}

// LẤY TOP 12 SẢN PHẨM
function layTop12SanPham() {
    return thongKeLuotMua()
        .sort((a, b) => b.luotMua - a.luotMua)
        .slice(0, 12);
}

// VẼ BIỂU ĐỒ
function veBieuDoLuotMua() {
    let top12 = layTop12SanPham();

    let labels = top12.map((sp) => sp.ten);
    let data = top12.map((sp) => sp.luotMua);
    let ids = top12.map((sp) => sp.id);

    const ctx = document.getElementById("bieuDoLuotMua").getContext("2d");

    new Chart(ctx, {
        type: "bar",
        data: {
            labels: labels,
            datasets: [
                {
                    label: "Lượt mua",
                    data: data,
                    backgroundColor: "#1ca45c",
                },
            ],
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            layout: {
                padding: {
                    bottom: 20,
                },
            },
            scales: {
                x: {
                    ticks: {
                        maxRotation: 0,
                        minRotation: 0,
                    },
                },
                y: {
                    beginAtZero: true,
                },
            },
        },
    });
}

veBieuDoLuotMua();

loadSanPhamAdmin();
