using RPU.DataAccess.Repository.IRepository;
using RPU.Models.Enums;
using RPU.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RPUWeb.ViewComponents;

public class SharedVacationPlanViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public SharedVacationPlanViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is not null)
        {
            //Ustawienie w sesji wartości oczekujących zaproszeń
            HttpContext.Session.SetInt32(SD.SessionPendingSharedVacationPlan, _unitOfWork.SharedVacationPlan.GetAll(svp => svp.ReceiverApplicationUserId == claim.Value &&
                        svp.Status == SharedVacationPlanStatuses.Pending).ToList().Count);

            return View(HttpContext.Session.GetInt32(SD.SessionPendingSharedVacationPlan));
        }
        else
        {
            HttpContext.Session.Clear();
            return View(0);
        }
    }

}
