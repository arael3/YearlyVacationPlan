$(document).ready(function () {

    const checkboxesMaxVD = document.querySelectorAll('input[name="day"]');
    const maxVacationDaysNumber = document.getElementById("VacationPlan_MaxVacationDaysNumber");
    const maxVacationDaysNumberAlert = document.getElementById("max-vacation-days-number-alert");
    const countDisplay = document.getElementById("number-of-selected-days");

    const maxVacationDaysNumberAlertText = "Liczba zaznaczonych dni roboczych jest większa od wymiaru urlopu.";

    var hasAlerdBeenDisplayed = false;

    function updateSelectedDaysCount() {
        // Count the number of selected checkboxes
        const selectedCount = document.querySelectorAll('input[name="day"]:checked').length;
        // Update the display with the new count
        countDisplay.value = selectedCount;

        if (maxVacationDaysNumber.value < selectedCount) {
            if (!hasAlerdBeenDisplayed) {
                maxVacationDaysNumberAlert.innerHTML = maxVacationDaysNumberAlertText;
                toastr.warning(maxVacationDaysNumberAlertText);
                hasAlerdBeenDisplayed = true;
            }
        }
        else {
            maxVacationDaysNumberAlert.innerHTML = "";
            hasAlerdBeenDisplayed = false;
        }
    }

    // Call the updateSelectedDaysCount function once to update the count on page load
    updateSelectedDaysCount();

    // Add an event listener to each checkbox to update the count when the selection changes
    checkboxesMaxVD.forEach(function (checkbox) {
        checkbox.addEventListener('change', updateSelectedDaysCount);
    });

    $(document).ready(function () {
        const submitButton = $("#update-button");
        const form = $("#form");
        submitButton.on("click", function (event) {
            event.preventDefault();

            var checkboxes = $("input[type=checkbox]:checked");
            var values = [];

            checkboxes.each(function (index) {
                values.push($(this).val());
            });

            $('#vacation-days').val(values);

            form.submit();
        });
    });


    const planMenu = document.getElementById('plan-menu');
    const overlay = document.getElementById('overlay');
    const planMenuButtonOn = document.getElementById('plan-menu-button-on');
    const planMenuButtonOff = document.getElementById('plan-menu-button-off');
    const planMenuButtonBox = document.getElementById('plan-menu-button-box');

    function showPlanMenu() {
        setTimeout(() => {
            if (window.innerWidth < 413) {
                planMenu.style.width = '100vw';
            } else {
                planMenu.style.width = '393px';
            }
            planMenu.style.left = '0px';
            planMenuButtonBox.style.left = '0px';
            planMenu.style.overflow = 'auto';
            overlay.style.backgroundColor = 'rgba(0,0,0,0.25)';
            overlay.style.zIndex = '1030';
            overlay.style.pointerEvents = 'auto';
            planMenuButton.innerHTML = '<i class="bi bi-chevron-double-left"></i>';
        }, 50);
    }

    function hidePlanMenu() {
        setTimeout(() => {
            planMenu.style.width = '393px';
            planMenu.style.left = '-403px';
            planMenu.style.overflow = 'hidden';
            planMenuButtonBox.style.left = '-403px';
            overlay.style.backgroundColor = 'rgba(0,0,0,0)';
            overlay.style.zIndex = '0';
            overlay.style.pointerEvents = 'none';
            planMenuButton.innerHTML = '<i class="bi bi-chevron-double-right"></i>';
        }, 60);
    }

    overlay.addEventListener('click', hidePlanMenu);
    planMenu.addEventListener('click', showPlanMenu);

    planMenuButtonOn.addEventListener('click', function (event) {
        event.preventDefault();
        showPlanMenu();
    });

    planMenuButtonOff.addEventListener('click', function (event) {
        event.preventDefault();
        hidePlanMenu();
    });
});