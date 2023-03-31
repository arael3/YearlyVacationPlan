var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    const isAdmin = document.getElementById('is-admin').dataset.isAdmin;
    // Jeżeli zalogowany użytkownik jest adminem, pokaż kolumnę applicationUser.name
    if (isAdmin == 1) {
        dataTable = $('#tblData').DataTable({
            "ajax": {
                "url": "/Customer/VacationPlan/GetAllAPI"
            },
            "columns": [
                { "data": "applicationUser.userName", "width": "30%" },
                { "data": "year", "width": "50%" },
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                        <div class="btn-group my-2" role="group">
                            <a href="/Customer/VacationPlan/Update?id=${data}" class="btn btn-primary"><i class="bi bi-pencil-square"></i> &nbsp; Edytuj</a>
                            <a onclick=deleteItem('/Customer/VacationPlan/DeleteAPI/${data}') class="btn btn-danger"><i class="bi bi-trash3"></i> &nbsp; Usuń</a>
                        </div>
                        `
                    },
                    "width": "20%"
                }
            ],
            "order": [[1, "desc"]]
        });
    }
    else {
        dataTable = $('#tblData').DataTable({
            "ajax": {
                "url": "/Customer/VacationPlan/GetAllAPI"
            },
            "columns": [
                { "data": "year", "width": "80%"},
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                        <div class="w-75 btn-group my-2" role="group">
                            <a href="/Customer/VacationPlan/Update?id=${data}" class="btn btn-primary"><i class="bi bi-pencil-square"></i> &nbsp; Edytuj</a>
                            <a onclick=deleteItem('/Customer/VacationPlan/DeleteAPI/${data}') class="btn btn-danger"><i class="bi bi-trash3"></i> &nbsp; Usuń</a>
                        </div>
                        `
                    },
                    "width": "20%"
                }
            ],
            "order": [[0, "desc"]]
        });
    }
}