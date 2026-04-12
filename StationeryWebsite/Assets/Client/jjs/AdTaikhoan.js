//  LOAD DANH SÁCH TÀI KHOẢN
function loadAccounts() {
    return JSON.parse(localStorage.getItem("accounts")) || [];
}

//  LƯU SAU KHI SỬA / XÓA
function saveAccounts(list) {
    localStorage.setItem("accounts", JSON.stringify(list));
}

//  HIỂN THỊ BẢNG TÀI KHOẢN
function renderTable() {
    const danhSach = loadAccounts();
    const table = document.getElementById("tableBody");
    table.innerHTML = "";

    danhSach.forEach((acc, index) => {
        // Kiểm tra ADMIN GỐC
        const isMainAdmin = acc.email === "admin@gmail.com";

        table.innerHTML += `
            <tr>
                <td>
                    <input class="form-control text-center"
                        value="${acc.hoten}"
                        id="name_${index}"
                        ${isMainAdmin ? "readonly" : ""}>
                </td>

                <td>${acc.email}</td>

                <td>
                    <input class="form-control text-center"
                        value="${acc.password}"
                        id="pw_${index}"
                        ${isMainAdmin ? "readonly" : ""}>
                </td>

                <td>
                    <select class="form-control" id="role_${index}" ${isMainAdmin ? "disabled" : ""}>
                        <option value="user" ${acc.role === "user" ? "selected" : ""}>User</option>
                        <option value="admin" ${acc.role === "admin" ? "selected" : ""}>Admin</option>
                    </select>
                </td>

                <td>
                    ${
                        isMainAdmin
                            ? `<button class="btn btn-sm btn-secondary" disabled>X</button>`
                            : `<button class="btn btn-sm btn-success" onclick="updateAccount(${index})">
                                <i class="fa-solid fa-pen"></i>
                           </button>`
                    }
                </td>
 
                <td>
                    ${
                        isMainAdmin
                            ? `<button class="btn btn-sm btn-secondary" disabled>X</button>`
                            : `<button class="btn btn-sm btn-danger" onclick="deleteAccount(${index})">
                                <i class="fa-solid fa-trash"></i>
                           </button>`
                    }
                </td>
            </tr>
        `;
    });
}

//      CẬP NHẬT TÀI KHOẢN
function updateAccount(i) {
    let list = loadAccounts();

    // KHÓA ADMIN GỐC
    if (list[i].email === "admin@gmail.com") {
        alert("Không thể sửa tài khoản Admin mặc định!");
        return;
    }

    list[i].hoten = document.getElementById(`name_${i}`).value.trim();
    list[i].password = document.getElementById(`pw_${i}`).value.trim();
    list[i].role = document.getElementById(`role_${i}`).value.trim();

    saveAccounts(list);
    alert("Cập nhật thành công!");
    renderTable();
}

//         XÓA TÀI KHOẢN
function deleteAccount(i) {
    let list = loadAccounts();

    if (list[i].email === "admin@gmail.com") {
        alert("Không thể xoá tài khoản Admin mặc định!");
        return;
    }

    if (confirm("Bạn có chắc muốn xóa tài khoản này?")) {
        list.splice(i, 1);
        saveAccounts(list);
        renderTable();
    }
}

//   GỌI RENDER LẦN ĐẦU
renderTable();
