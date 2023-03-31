using RPU.DataAccess.Repository.IRepository;
using RPU.Models;
using RPU.Models.Enums;
using RPU.Models.ViewModels;
using RPU.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RPUWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class SharedVacationPlanController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    public SharedVacationPlanController(IUnitOfWork unitOfWork, IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

        IEnumerable<SharedVacationPlan> sharedVacationPlanList = _unitOfWork.SharedVacationPlan.GetAll(svp => svp.ReceiverApplicationUserId == nameIdentifier &&
            svp.Status == SharedVacationPlanStatuses.Pending);

        List<SharedVacationPlanVM> sharedVacationPlanVMList = new();

        foreach (var sharedVacationPlan in sharedVacationPlanList)
        {
            sharedVacationPlanVMList.Add(new()
            {
                SharedVacationPlan = sharedVacationPlan,
                SenderApplicationUsername = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.Id == sharedVacationPlan.SenderApplicationUserId).UserName
            });
        }

        return View(sharedVacationPlanVMList);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(string invitedUserNameOrEmail, int vacationPlanId)
    {
        var invitedUser = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.UserName == invitedUserNameOrEmail || u.Email == invitedUserNameOrEmail);
        if (invitedUser == null)
        {
            TempData["error"] = "Użytkownik o podanej nazwie nie istnieje.";
            return RedirectToAction("Update", "VacationPlan", new { id = vacationPlanId });
        }

        VacationPlan vacationPlan = _unitOfWork.VacationPlan.GetFirstOrDefault(vp => vp.Id == vacationPlanId);

        if (invitedUser.Id == vacationPlan.ApplicationUserId)
        {
            TempData["error"] = "Nie możesz wysłać zaproszenia do siebie.";
            return RedirectToAction("Update", "VacationPlan", new { id = vacationPlanId });
        }

        if (invitedUser is not null)
        {
            VacationPlan vacationPlanOfInvitedUser = _unitOfWork.VacationPlan.GetAll(vp => vp.ApplicationUserId == invitedUser.Id).FirstOrDefault(vp => vp.Year == vacationPlan.Year);

            if (vacationPlanOfInvitedUser is null)
            {
                TempData["error"] = "Użytkownik o podanej nazwie nie posiada planu urlopowego na wybrany rok.";
                return RedirectToAction("Update", "VacationPlan", new { id = vacationPlan.Id });
            }
        }

        SharedVacationPlan sharedVacationPlanFromDb = _unitOfWork.SharedVacationPlan.GetFirstOrDefault(svp =>
        svp.SenderApplicationUserId == vacationPlan.ApplicationUserId &&
        svp.ReceiverApplicationUserId == invitedUser.Id &&
        svp.SharedVacationPlanYear == vacationPlan.Year &&
        svp.Status == SharedVacationPlanStatuses.Pending);

        if (sharedVacationPlanFromDb is not null)
        {
            TempData["error"] = "Istnieje już oczekujące zaproszenie dla wskazanego użytkownika.";
            return RedirectToAction("Update", "VacationPlan", new { id = vacationPlan.Id });
        }

        sharedVacationPlanFromDb = _unitOfWork.SharedVacationPlan.GetFirstOrDefault(svp =>
        svp.SenderApplicationUserId == invitedUser.Id &&
        svp.ReceiverApplicationUserId == vacationPlan.ApplicationUserId &&
        svp.SharedVacationPlanYear == vacationPlan.Year &&
        svp.Status == SharedVacationPlanStatuses.Pending);

        if (sharedVacationPlanFromDb is not null)
        {
            TempData["error"] = "Posiadasz oczekujące zaproszenie od wskazanego użytkownika.";
            return RedirectToAction("Update", "VacationPlan", new { id = vacationPlan.Id });
        }

        sharedVacationPlanFromDb = _unitOfWork.SharedVacationPlan.GetFirstOrDefault(svp =>
        svp.SenderApplicationUserId == vacationPlan.ApplicationUserId &&
        svp.ReceiverApplicationUserId == invitedUser.Id &&
        svp.SharedVacationPlanYear == vacationPlan.Year &&
        svp.Status == SharedVacationPlanStatuses.Accepted);

        if (sharedVacationPlanFromDb is not null)
        {
            TempData["error"] = "Współdzielisz już plan urlopowy na wybrany rok ze wskazanym użytkownikiem.";
            return RedirectToAction("Update", "VacationPlan", new { id = vacationPlan.Id });
        }

        sharedVacationPlanFromDb = _unitOfWork.SharedVacationPlan.GetFirstOrDefault(svp =>
        svp.SenderApplicationUserId == invitedUser.Id &&
        svp.ReceiverApplicationUserId == vacationPlan.ApplicationUserId &&
        svp.SharedVacationPlanYear == vacationPlan.Year &&
        svp.Status == SharedVacationPlanStatuses.Accepted);

        if (sharedVacationPlanFromDb is not null)
        {
            TempData["error"] = "Współdzielisz już plan urlopowy na wybrany rok ze wskazanym użytkownikiem.";
            return RedirectToAction("Update", "VacationPlan", new { id = vacationPlan.Id });
        }

        SharedVacationPlan sharedVacationPlan = new SharedVacationPlan()
        {
            SenderApplicationUserId = vacationPlan.ApplicationUserId,
            ReceiverApplicationUserId = invitedUser.Id,
            SharedVacationPlanYear = vacationPlan.Year,
            Status = SharedVacationPlanStatuses.Pending
        };

        _unitOfWork.SharedVacationPlan.Add(sharedVacationPlan);
        _unitOfWork.Save();
        //HttpContext.Session.Clear();
        TempData["success"] = "Pomyślnie wysłano zaproszenie";

        _emailSender.SendEmailAsync(invitedUser.Email, "Otrzymałeś/-aś zaproszenie do wspódzielenia planu urlopowego na portalu Mój Plan Urlopowy",
                    $"<a href='https://localhost:7064/Identity/Account/Login'>Zaloguj się na portalu</a>, aby zaakceptować lub odrzucić zaproszenie");

        // $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        return RedirectToAction("Update", "VacationPlan", new { id = vacationPlan.Id });
    }

    #region API CALLS
    [HttpPost]
    public IActionResult StatusUpdateAPI(int id)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        SharedVacationPlan sharedVacationPlanFromDb = _unitOfWork.SharedVacationPlan.GetFirstOrDefault(svp => svp.Id == id);

        var vacationPlanId = _unitOfWork.VacationPlan.GetAll(vp => vp.ApplicationUserId == nameIdentifier)
                .FirstOrDefault(vp => vp.Year == sharedVacationPlanFromDb.SharedVacationPlanYear).Id;

        // Jeżeli nie znaleziono planu urlopowego lub zalogowany użytkownik nie jest właścicielem planu oraz nie jest administratorem, zwróć błąd
        if (sharedVacationPlanFromDb is null ||
            (nameIdentifier != sharedVacationPlanFromDb.ReceiverApplicationUserId &&
             role != SD.Role_Admin))
        {
            return Json(new { success = false, message = "Operacja niedozwolona." });
        }

        _unitOfWork.SharedVacationPlan.UpdateStatus(id, SharedVacationPlanStatuses.Accepted);
        _unitOfWork.Save();
        //HttpContext.Session.Clear();

        string note = "Zaakceptowano współdzielenie planu urlopowego";

        if (vacationPlanId == 0 || role == "Admin")
        {
            return Json(new { success = true, message = note, redirectToUrl = Url.Action("Index", "VacationPlan") });
        }

        return Json(new { success = true, message = note, redirectToUrl = Url.Action("Update", "VacationPlan", new { id = vacationPlanId }) });
    }


    [HttpDelete]
    public IActionResult DeleteAPI(int id)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        SharedVacationPlan sharedVacationPlanFromDb = _unitOfWork.SharedVacationPlan.GetFirstOrDefault(svp => svp.Id == id);

        var vacationPlanId = _unitOfWork.VacationPlan.GetAll(vp => vp.ApplicationUserId == nameIdentifier)
                .FirstOrDefault(vp => vp.Year == sharedVacationPlanFromDb.SharedVacationPlanYear).Id;

        // Jeżeli nie znaleziono planu urlopowego lub zalogowany użytkownik nie jest właścicielem planu oraz nie jest administratorem, zwróć błąd
        if (sharedVacationPlanFromDb is null ||
            (nameIdentifier != sharedVacationPlanFromDb.SenderApplicationUserId &&
             nameIdentifier != sharedVacationPlanFromDb.ReceiverApplicationUserId &&
             role != SD.Role_Admin))
        {
            return Json(new { success = false, message = "Operacja niedozwolona." });
        }

        _unitOfWork.SharedVacationPlan.Remove(sharedVacationPlanFromDb);
        _unitOfWork.Save();
        //HttpContext.Session.Clear();

        if (sharedVacationPlanFromDb.Status == SharedVacationPlanStatuses.Accepted)
        {
            VacationPlan senderVacationPlanFromDb = _unitOfWork.VacationPlan.GetAll(vp => vp.ApplicationUserId == sharedVacationPlanFromDb.SenderApplicationUserId)
                .FirstOrDefault(vp => vp.Year == sharedVacationPlanFromDb.SharedVacationPlanYear);

            if (senderVacationPlanFromDb is not null)
            {
                if (senderVacationPlanFromDb.Deputy1ApplicationUserId == sharedVacationPlanFromDb.ReceiverApplicationUserId)
                {
                    senderVacationPlanFromDb.Deputy1ApplicationUserId = null;
                }
                if (senderVacationPlanFromDb.Deputy2ApplicationUserId == sharedVacationPlanFromDb.ReceiverApplicationUserId)
                {
                    senderVacationPlanFromDb.Deputy2ApplicationUserId = null;
                }

                _unitOfWork.VacationPlan.Update(senderVacationPlanFromDb);
                _unitOfWork.Save();
            }

            VacationPlan receiverVacationPlanFromDb = _unitOfWork.VacationPlan.GetAll(vp => vp.ApplicationUserId == sharedVacationPlanFromDb.ReceiverApplicationUserId)
                .FirstOrDefault(vp => vp.Year == sharedVacationPlanFromDb.SharedVacationPlanYear);

            if (receiverVacationPlanFromDb is not null)
            {
                if (receiverVacationPlanFromDb.Deputy1ApplicationUserId == sharedVacationPlanFromDb.SenderApplicationUserId)
                {
                    receiverVacationPlanFromDb.Deputy1ApplicationUserId = null;
                }
                if (receiverVacationPlanFromDb.Deputy2ApplicationUserId == sharedVacationPlanFromDb.SenderApplicationUserId)
                {
                    receiverVacationPlanFromDb.Deputy2ApplicationUserId = null;
                }

                _unitOfWork.VacationPlan.Update(receiverVacationPlanFromDb);
                _unitOfWork.Save();
            }
        }

        string note = "Anulowano współdzielenie planu urlopowego";

        if (vacationPlanId == 0 || role == "Admin")
        {
            return Json(new { success = true, message = note, redirectToUrl = Url.Action("Index", "VacationPlan") });
        }

        return Json(new { success = true, message = note, redirectToUrl = Url.Action("Update", "VacationPlan", new { id = vacationPlanId }) });
    }
    #endregion
}
