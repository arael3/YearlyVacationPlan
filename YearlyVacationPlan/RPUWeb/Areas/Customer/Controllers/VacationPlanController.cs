using RPU.DataAccess.Repository.IRepository;
using RPU.Models;
using RPU.Models.Enums;
using RPU.Models.ViewModels;
using RPU.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace RPUWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class VacationPlanController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<VacationPlanController> _logger;

    public VacationPlanController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, IEmailSender emailSender, ILogger<VacationPlanController> logger)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
        _emailSender = emailSender;
        _logger = logger;
    }


    public IActionResult Index()
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

        bool allowForCreateVacationPlan = false;

        List<int> listOfUserVacationPlans = _unitOfWork.VacationPlan.GetAll().Where(vp => vp.ApplicationUserId == nameIdentifier).Select(vp => vp.Year).ToList();

        if (!listOfUserVacationPlans.Any())
        {
            allowForCreateVacationPlan = true;
        }
        else
        {
            List<int> listOfYearlyHolidaySchedules = _unitOfWork.YearlyHolidaySchedule.GetAll().Select(yhs => yhs.Year).ToList();

            foreach (int year in listOfYearlyHolidaySchedules)
            {
                if (!listOfUserVacationPlans.Contains(year))
                {
                    allowForCreateVacationPlan = true;
                }
            }
        }

        ViewBag.AllowForCreateVacationPlan = allowForCreateVacationPlan;

        return View();
    }

    public IActionResult Create()
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

        List<int> availableYears = _unitOfWork.YearlyHolidaySchedule
            .GetAll()
            .Where(i => !_unitOfWork.VacationPlan
                            .GetAll()
                            .Where(vp => vp.ApplicationUserId == nameIdentifier)
                            .Select(vp => vp.Year)
                            .Contains(i.Year))
            .Select(i => i.Year).ToList();

        if (availableYears.Count() == 0)
        {
            return Json(new { success = false });
        }
        else
        {
            return Json(new { success = true, availableYearsList = availableYears });
        }
    }

    [HttpPost]
    public IActionResult Create(int year)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

        VacationPlan vacationPlan = new()
        {
            ApplicationUserId = nameIdentifier,
            Year = year
        };

        _unitOfWork.VacationPlan.Add(vacationPlan);
        _unitOfWork.Save();
        TempData["success"] = "Create - success";
        return RedirectToAction("Update", "VacationPlan", new { id = vacationPlan.Id });
    }

    public IActionResult Update(int? id)
    {
        VacationPlanVM vacationPlanVM = new()
        {
            VacationPlan = new(),
            HolidaySchedule = "",
            Username = "",
            ApplicationUsersList = new(),
            Deputy1VacationDays = "",
            Deputy2VacationDays = "",
            SentSharedVacationPlansVM = new(),
            ReceivedSharedVacationPlansVM = new(),
            AcceptedSharedVacationPlansVM = new()
        };

        vacationPlanVM = UpdateLogic(id, vacationPlanVM);

        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        // Jeżeli zalogowany użytkownik nie jest właścicielem planu oraz nie jest administratorem, zwróć błąd
        if (nameIdentifier != vacationPlanVM.VacationPlan.ApplicationUserId && role != SD.Role_Admin)
        {
            TempData["error"] = "Operacja niedozwolona";
            return RedirectToAction(nameof(Index));
        }

        if (vacationPlanVM.VacationPlan.Year != 0)
        {
            vacationPlanVM.HolidaySchedule = _unitOfWork.YearlyHolidaySchedule.GetFirstOrDefault(u => u.Year == vacationPlanVM.VacationPlan.Year).HolidaySchedule;

            if (vacationPlanVM.HolidaySchedule is null)
            {
                vacationPlanVM.HolidaySchedule = "";
            }

            IEnumerable<SharedVacationPlan> sentSharedVacationPlans = _unitOfWork.SharedVacationPlan.GetAll(svp => svp.SenderApplicationUserId == vacationPlanVM.VacationPlan.ApplicationUserId).Where(svp => svp.SharedVacationPlanYear == vacationPlanVM.VacationPlan.Year).ToList();

            if (sentSharedVacationPlans is not null)
            {
                foreach (var sharedVacationPlan in sentSharedVacationPlans)
                {
                    if (sharedVacationPlan.Status == SharedVacationPlanStatuses.Pending)
                    {
                        vacationPlanVM.SentSharedVacationPlansVM.Add(new()
                        {
                            SharedVacationPlan = sharedVacationPlan,
                            ReceiverApplicationUsername = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.Id == sharedVacationPlan.ReceiverApplicationUserId).UserName
                        });
                    }
                }
            }

            IEnumerable<SharedVacationPlan> receivedSharedVacationPlans = _unitOfWork.SharedVacationPlan.GetAll(vp => vp.ReceiverApplicationUserId == vacationPlanVM.VacationPlan.ApplicationUserId).Where(svp => svp.SharedVacationPlanYear == vacationPlanVM.VacationPlan.Year).ToList();

            if (receivedSharedVacationPlans is not null)
            {
                foreach (var sharedVacationPlan in receivedSharedVacationPlans)
                {
                    if (sharedVacationPlan.Status == SharedVacationPlanStatuses.Pending)
                    {
                        vacationPlanVM.ReceivedSharedVacationPlansVM.Add(new()
                        {
                            SharedVacationPlan = sharedVacationPlan,
                            SenderApplicationUsername = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.Id == sharedVacationPlan.SenderApplicationUserId).UserName
                        });
                    }
                }
            }



            IEnumerable<SharedVacationPlan> acceptedSharedVacationPlansSent = _unitOfWork.SharedVacationPlan.GetAll(vp => vp.SenderApplicationUserId == vacationPlanVM.VacationPlan.ApplicationUserId).Where(svp => svp.SharedVacationPlanYear == vacationPlanVM.VacationPlan.Year).ToList();

            IEnumerable<SharedVacationPlan> acceptedSharedVacationPlansReceived = _unitOfWork.SharedVacationPlan.GetAll(vp => vp.ReceiverApplicationUserId == vacationPlanVM.VacationPlan.ApplicationUserId).Where(svp => svp.SharedVacationPlanYear == vacationPlanVM.VacationPlan.Year).ToList();

            if (acceptedSharedVacationPlansSent is not null)
            {
                foreach (var sharedVacationPlan in acceptedSharedVacationPlansSent)
                {
                    if (sharedVacationPlan.Status == SharedVacationPlanStatuses.Accepted)
                    {
                        string username = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.Id == sharedVacationPlan.ReceiverApplicationUserId).UserName;

                        vacationPlanVM.AcceptedSharedVacationPlansVM.Add(new()
                        {
                            SharedVacationPlan = sharedVacationPlan,
                            AcceptedShareApplicationUsername = username
                        });

                        vacationPlanVM.ApplicationUsersList.Add(new()
                        {
                            Text = username,
                            Value = sharedVacationPlan.ReceiverApplicationUserId
                        });
                    }
                }
            }

            if (acceptedSharedVacationPlansReceived is not null)
            {
                foreach (var sharedVacationPlan in acceptedSharedVacationPlansReceived)
                {
                    string username = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.Id == sharedVacationPlan.SenderApplicationUserId).UserName;

                    if (sharedVacationPlan.Status == SharedVacationPlanStatuses.Accepted)
                    {
                        vacationPlanVM.AcceptedSharedVacationPlansVM.Add(new()
                        {
                            SharedVacationPlan = sharedVacationPlan,
                            AcceptedShareApplicationUsername = username
                        });

                        vacationPlanVM.ApplicationUsersList.Add(new()
                        {
                            Text = username,
                            Value = sharedVacationPlan.SenderApplicationUserId
                        });
                    }
                }
            }
        }

        return View(vacationPlanVM);
    }

    private VacationPlanVM UpdateLogic(int? id, VacationPlanVM vacationPlanVM)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // W tym kodzie najpierw pobieramy wszystkie obiekty z YearlyHolidaySchedule, a następnie filtrujemy je metodą Where. W ramach tego filtra tworzymy zagnieżdżone zapytanie, które pobiera lata z VacationPlan tylko dla danego użytkownika (ApplicationUserId == claim.Value) i konwertuje je na listę. Następnie za pomocą metody Contains sprawdzamy, czy dany rok występuje w zbiorze VacationPlan i negujemy ten warunek za pomocą operatora !. Dzięki temu otrzymujemy tylko te elementy z YearlyHolidaySchedule, których rok nie znajduje się w zbiorze VacationPlan. Na końcu mapujemy te obiekty na listę SelectListItem zawierającą rok i ID.
        vacationPlanVM.AvailableYears = _unitOfWork.YearlyHolidaySchedule
            .GetAll()
            .Where(i => !_unitOfWork.VacationPlan
                            .GetAll()
                            .Where(vp => vp.ApplicationUserId == nameIdentifier)
                            .Select(vp => vp.Year)
                            .Contains(i.Year))
            .Select(i => new SelectListItem
            {
                Text = i.Year.ToString(),
                Value = i.Id.ToString(),
            });

        //vacationPlanVM.ApplicationUsersList = _unitOfWork.ApplicationUser.GetAll().Select(i => new SelectListItem
        //{
        //    Text = i.Name,
        //    Value = i.Id.ToString(),
        //});

        if (id == null || id == 0)
        {
            vacationPlanVM.VacationPlan.ApplicationUserId = nameIdentifier;
            vacationPlanVM.Username = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.Id == nameIdentifier).UserName;

            vacationPlanVM = UpdateOperations(id, vacationPlanVM);

            return vacationPlanVM;
        }
        else
        {
            // Update 
            vacationPlanVM.VacationPlan = _unitOfWork.VacationPlan.GetFirstOrDefault(u => u.Id == id);
            vacationPlanVM.Username = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.Id == vacationPlanVM.VacationPlan.ApplicationUserId).UserName;

            vacationPlanVM = UpdateOperations(id, vacationPlanVM);

            return vacationPlanVM;
        }
    }

    private VacationPlanVM UpdateOperations(int? id, VacationPlanVM vacationPlanVM)
    {
        string format = "dd.MM.yyyy HH:mm:ss";

        if (vacationPlanVM.VacationPlan.VacationDays is null)
        {
            vacationPlanVM.VacationPlan.VacationDays = "";
        }

        VacationPlan deputy1VacationPlan = _unitOfWork.VacationPlan.GetFirstOrDefault(u => u.ApplicationUserId == vacationPlanVM.VacationPlan.Deputy1ApplicationUserId);

        if (deputy1VacationPlan is not null)
        {
            if (deputy1VacationPlan.VacationDays is null)
            {
                vacationPlanVM.Deputy1VacationDays = " ";
            }
            else
            {
                vacationPlanVM.Deputy1VacationDays = deputy1VacationPlan.VacationDays;
            }
        }
        else
        {
            deputy1VacationPlan = new();
            vacationPlanVM.Deputy1VacationDays = " ";
        }

        VacationPlan deputy2VacationPlan = _unitOfWork.VacationPlan.GetFirstOrDefault(u => u.ApplicationUserId == vacationPlanVM.VacationPlan.Deputy2ApplicationUserId);

        if (deputy2VacationPlan is not null)
        {
            if (deputy2VacationPlan.VacationDays is null)
            {
                vacationPlanVM.Deputy2VacationDays = " ";
            }
            else
            {
                vacationPlanVM.Deputy2VacationDays = deputy2VacationPlan.VacationDays;
            }
        }
        else
        {
            deputy2VacationPlan = new();
            vacationPlanVM.Deputy2VacationDays = " ";
        }

        return vacationPlanVM;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(VacationPlanVM obj)
    {
        if (ModelState.IsValid)
        {
            string tempData = "";

            if (obj.VacationPlan.Id == 0)
            {
                _unitOfWork.VacationPlan.Add(obj.VacationPlan);
                tempData = "Create - success";
            }
            else
            {
                _unitOfWork.VacationPlan.Update(obj.VacationPlan);
                tempData = "Update - success";
            }

            _unitOfWork.Save();
            TempData["success"] = tempData;
            return RedirectToAction("Update", "VacationPlan", new { id = obj.VacationPlan.Id });
        }

        obj = UpdateLogic(obj.VacationPlan.Id, obj);

        return View(obj);
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        var obj = _unitOfWork.VacationPlan.GetFirstOrDefault(c => c.Id == id);

        // Jeżeli nie znaleziono planu urlopowego lub zalogowany użytkownik nie jest właścicielem planu oraz nie jest administratorem, zwróć błąd
        if (obj is null || (nameIdentifier != obj.ApplicationUserId && role != SD.Role_Admin))
        {
            TempData["error"] = "Operacja niedozwolona.";
            return RedirectToAction(nameof(Index));
        }


        // Wyszukanie wszystkich SharedVacationPlan. Usunięcie SharedVacationPlans, które mają SenderApplicationUserId lub ReceiverApplicationUserId równe ApplicationUserId usuwanego VacationPlan

        IEnumerable<SharedVacationPlan> sharedVacationPlans = _unitOfWork.SharedVacationPlan.GetAll(svp => svp.SenderApplicationUserId == obj.ApplicationUserId).ToList();

        foreach (var sharedVacationPlan in sharedVacationPlans)
        {
            _unitOfWork.SharedVacationPlan.Remove(sharedVacationPlan);
        }

        sharedVacationPlans = _unitOfWork.SharedVacationPlan.GetAll(svp => svp.ReceiverApplicationUserId == obj.ApplicationUserId).ToList();

        foreach (var sharedVacationPlan in sharedVacationPlans)
        {
            _unitOfWork.SharedVacationPlan.Remove(sharedVacationPlan);
        }

        _unitOfWork.VacationPlan.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] = "Plan urlopowy został usunięty.";
        return RedirectToAction(nameof(Index));
    }


    #region API CALLS
    [HttpGet]
    public IActionResult GetAllAPI()
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (role == SD.Role_Admin)
        {
            var vacationPlanList = _unitOfWork.VacationPlan.GetAll(includeProperties: "ApplicationUser");
            return Json(new { data = vacationPlanList });
        }
        else
        {
            var vacationPlanList = _unitOfWork.VacationPlan.GetAll(vp => vp.ApplicationUserId == nameIdentifier, includeProperties: "ApplicationUser");
            return Json(new { data = vacationPlanList });
        }
    }

    [HttpDelete]
    public IActionResult DeleteAPI(int? id)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        var obj = _unitOfWork.VacationPlan.GetFirstOrDefault(c => c.Id == id);

        // Jeżeli nie znaleziono planu urlopowego lub zalogowany użytkownik nie jest właścicielem planu oraz nie jest administratorem, zwróć błąd
        if (obj is null || (nameIdentifier != obj.ApplicationUserId && role != SD.Role_Admin))
        {
            return Json(new { success = false, message = "Operacja niedozwolona." });
        }

        _unitOfWork.VacationPlan.Remove(obj);
        _unitOfWork.Save();
        //TempData["success"] = "Delete - success";
        //return RedirectToAction("Index");
        return Json(new { success = true, message = "Plan urlopowy został usunięty.", redirectToUrl = Url.Action(nameof(Index)) });
    }
    #endregion
}
