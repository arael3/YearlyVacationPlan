var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Customer/VacationPlan/GetAll"
        },
        "columns": [
            { "data":"applicationUser.name", "width" : "60%" },
            { "data":"year", "width" : "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/Customer/VacationPlan/Update?id=${data}" class="btn btn-primary"><i class="bi bi-pencil-square"></i> &nbsp; Edit</a>
                            <a onClick=Delete('/Customer/VacationPlan/Delete/${data}') class="btn btn-danger"><i class="bi bi-trash3"></i> &nbsp; Delete</a>
                        </div>
                        `
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {
    // Swal - SweetAlert2
    Swal.fire({
        title: 'Jesteś pewien?',
        text: "Nie będziesz mógł cofnąć tej operacji.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    // Dane dla data.success pochodzą z
                    // ProductController.cs >> public IActionResult Delete(int ? id) >> return Json(new { success = true, message = "Delete - success" });
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}