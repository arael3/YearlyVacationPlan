// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    if (localStorage.getItem('success')) {
        toastr.success(localStorage.getItem('message'));
        localStorage.removeItem('success');
        localStorage.removeItem('message');
    }
});

function createVacationPlan() {
    const years = [];

    $.ajax({
        type: 'GET',
        url: '/Customer/VacationPlan/Create',
        success: function (response) {
            if (response.success) {

                response.availableYearsList.forEach(function (year) {
                    years.push(year); 
                });

                Swal.fire({
                    title: 'Utwórz nowy plan urlopowy',
                    input: 'select',
                    inputOptions: years.reduce((options, year) => ({ ...options, [year]: year }), {}),
                    inputLabel: 'Rok',
                    inputPlaceholder: 'Wybierz rok',
                    showCancelButton: true,
                    confirmButtonText: 'Utwórz',
                    cancelButtonText: 'Anuluj',
                    showLoaderOnConfirm: true,
                    allowOutsideClick: () => !Swal.isLoading(),
                    preConfirm: (invitedUserNameOrEmail) => {
                        if (!invitedUserNameOrEmail) {
                            Swal.showValidationMessage('Wybierz rok')
                        } else {
                            return invitedUserNameOrEmail;
                        }
                    },
                    didOpen: () => {
                        Swal.getInput().focus()
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        $(document).ready(function () {
                            const form = $("#create-vacation-plan");
                            $('#vacation-plan-year').val(result.value);
                            form.submit();
                        });
                    }
                })
            }
        }
    });
}

function createSharedVacationPlan() {
    Swal.fire({
        title: 'Wyślij zaproszenie do współdzielenia planu urlopowego',
        input: 'text',
        inputLabel: 'Nazwa użytkownika / e-mail',
        inputPlaceholder: 'Wpisz nazwę użytkownika lub e-mail...',
        showCancelButton: true,
        confirmButtonText: 'Wyślij',
        cancelButtonText: 'Anuluj',
        showLoaderOnConfirm: true,
        allowOutsideClick: () => !Swal.isLoading(),
        preConfirm: (invitedUserNameOrEmail) => {
            if (!invitedUserNameOrEmail) {
                Swal.showValidationMessage('Wpisz nazwę użytkownika lub e-mail.')
            } else {
                return invitedUserNameOrEmail;
            }
        },
        didOpen: () => {
            Swal.getInput().focus()
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $(document).ready(function () {
                const form = $("#create-shared-vacation-plan");
                $('#invitedUserNameOrEmail').val(result.value);
                form.submit();
            });
        }
    })
}


function updateSharedVacationPlan(url) {
    const vpId = document.getElementById("vacationPlanId");

    Swal.fire({
        title: 'Jesteś pewny?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Akceptuj',
        cancelButtonText: 'Anuluj'
    })
        .then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: 'POST',
                    url: url,
                    success: function (response) {
                        // Dane dla data.success pochodzą z
                        // VacationPlanController.cs >> public IActionResult DeleteAPI(int ? id) >> return Json(new { success = true, message = "Delete - success" });
                        if (response.success) {
                            // Zapisanie informacji o powodzeniu w localStorage
                            localStorage.setItem('success', response.success);
                            localStorage.setItem('message', response.message);
                            window.location.href = response.redirectToUrl;
                        }
                        else {
                            toastr.error(response.message);
                        }
                    }
                })

            }
        });
}


function deleteItem(url) {
    Swal.fire({
        title: 'Jesteś pewny?',
        text: "Nie będziesz mógł tego cofnąć.",
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Usuń',
        cancelButtonText: 'Anuluj'
    })
    .then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (response) {
                    // Dane dla data.success pochodzą z
                    // VacationPlanController.cs >> public IActionResult DeleteAPI(int ? id) >> return Json(new { success = true, message = "Delete - success" });
                    if (response.success) {
                        // Zapisanie informacji o powodzeniu w localStorage
                        localStorage.setItem('success', response.success);
                        localStorage.setItem('message', response.message);
                        window.location.href = response.redirectToUrl;
                    }
                    else {
                        toastr.error(response.message);
                    }
                }
            })

        }
    });
}
