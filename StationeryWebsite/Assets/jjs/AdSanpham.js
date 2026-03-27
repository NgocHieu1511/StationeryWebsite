let dsSanPham = JSON.parse(localStorage.getItem("dsSanPham")) || [];
let currentPage = 1;
const itemPerPage = 16;

function renderSanPham() {
    const tbody = document.getElementById("tableBody");
    tbody.innerHTML = "";

    let start = (currentPage - 1) * itemPerPage;
    let end = start + itemPerPage;

    let dsHienThi = dsSanPham.slice(start, end);

    dsHienThi.forEach((sp, i) => {
        const tr = document.createElement("tr");

        tr.innerHTML = `
            <td>${sp.id}</td>
            <td><img src="${sp.hinh}" class="prod-img" /></td>
            <td>${sp.ten}</td>
            <td>${Number(sp.gia).toLocaleString()} đ</td>
            <td>
                <button class="btnXoa" data-index="${start + i}">
                    <i class="fa fa-trash"></i>
                </button>
            </td>
        `;

        tbody.appendChild(tr);
    });

    // Gán sự kiện XÓA
    document.querySelectorAll(".btnXoa").forEach((btn) => {
        btn.addEventListener("click", function () {
            let index = this.getAttribute("data-index");

            dsSanPham.splice(index, 1); // xóa khỏi ds
            localStorage.setItem("dsSanPham", JSON.stringify(dsSanPham));

            // Nếu trang hiện tại trống tự lùi về trang trước
            let totalPage = Math.ceil(dsSanPham.length / itemPerPage);
            if (currentPage > totalPage) currentPage = totalPage;

            renderSanPham();
        });
    });

    renderPagination();
}

function renderPagination() {
    const pagination = document.getElementById("pagination");
    const totalPage = Math.ceil(dsSanPham.length / itemPerPage);

    pagination.innerHTML = "";

    // Nút <
    let prev = document.createElement("a");
    prev.href = "#";
    prev.innerHTML = "&lt;";
    prev.classList.add("nut");
    if (currentPage === 1) prev.classList.add("disabled");
    prev.onclick = () => changePage(currentPage - 1);
    pagination.appendChild(prev);

    // Nút số trang
    for (let i = 1; i <= totalPage; i++) {
        let btn = document.createElement("a");
        btn.href = "#";
        btn.textContent = i;
        btn.classList.add("nut");
        if (i === currentPage) btn.classList.add("active");
        btn.onclick = () => changePage(i);
        pagination.appendChild(btn);
    }

    // Nút >
    let next = document.createElement("a");
    next.href = "#";
    next.innerHTML = "&gt;";
    next.classList.add("nut");
    if (currentPage === totalPage) next.classList.add("disabled");
    next.onclick = () => changePage(currentPage + 1);
    pagination.appendChild(next);
}

function changePage(page) {
    const totalPage = Math.ceil(dsSanPham.length / itemPerPage);

    if (page >= 1 && page <= totalPage) {
        currentPage = page;
        renderSanPham();
    }
}

document.addEventListener("DOMContentLoaded", renderSanPham);
