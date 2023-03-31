using RPU.DataAccess.Repository.IRepository;
using RPU.Models;
using RPU.Models.ViewModels;
using RPU.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace RPUWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class YearlyHolidayScheduleController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public YearlyHolidayScheduleController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<YearlyHolidaySchedule> objYearlyHolidayScheduleList = _unitOfWork.YearlyHolidaySchedule.GetAll();

        return View(objYearlyHolidayScheduleList);
    }


    public IActionResult Upsert(int? id)
    {
        YearlyHolidayScheduleVM yearlyHolidayScheduleVM = new()
        {
            YearlyHolidaySchedule = new(),
            YearlyHolidayScheduleDaysList = new List<DateTime>()
        };

        if (id == null || id == 0)
        {
            return View(yearlyHolidayScheduleVM);
        }
        else
        {
            // Update 
            yearlyHolidayScheduleVM.YearlyHolidaySchedule = _unitOfWork.YearlyHolidaySchedule.GetFirstOrDefault(u => u.Id == id);
            if (yearlyHolidayScheduleVM.YearlyHolidaySchedule.HolidaySchedule is not null)
            {
                List<string> YearlyHolidayScheduleDaysListString = new List<string>(yearlyHolidayScheduleVM.YearlyHolidaySchedule.HolidaySchedule.Split(','));
                string format = "dd.MM.yyyy HH:mm:ss";
                foreach (var dateString in YearlyHolidayScheduleDaysListString)
                {
                    DateTime date = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
                    yearlyHolidayScheduleVM.YearlyHolidayScheduleDaysList.Add(date);
                }
            }

            return View(yearlyHolidayScheduleVM);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(YearlyHolidayScheduleVM obj)
    {
        if (ModelState.IsValid)
        {
            string tempData = "";

            if (obj.YearlyHolidaySchedule.Id == 0)
            {
                if (_unitOfWork.YearlyHolidaySchedule.GetFirstOrDefault(u => u.Year == obj.YearlyHolidaySchedule.Year) is not null)
                {
                    TempData["error"] = $"Istnieje już roczny harmonogram dla {obj.YearlyHolidaySchedule.Year} roku";
                    return RedirectToAction("Index");
                }

                _unitOfWork.YearlyHolidaySchedule.Add(obj.YearlyHolidaySchedule);
                tempData = "Create - success";
            }
            else
            {
                _unitOfWork.YearlyHolidaySchedule.Update(obj.YearlyHolidaySchedule);
                tempData = "Update - success";
            }

            _unitOfWork.Save();
            TempData["success"] = tempData;
            return RedirectToAction("Upsert", "YearlyHolidaySchedule", new { id = obj.YearlyHolidaySchedule.Id });
        }

        return View(obj);
    }

    public IActionResult Delete(YearlyHolidayScheduleVM obj)
    {
        if (obj.YearlyHolidaySchedule.Id == null || obj.YearlyHolidaySchedule.Id == 0)
        {
            return NotFound();
        }

        var YearlyHolidayScheduleFromDb = _unitOfWork.YearlyHolidaySchedule.GetFirstOrDefault(c => c.Id == obj.YearlyHolidaySchedule.Id);
        if (YearlyHolidayScheduleFromDb == null) return NotFound();
        return View(YearlyHolidayScheduleFromDb);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(YearlyHolidayScheduleVM obj)
    {
        if (obj.YearlyHolidaySchedule is null)
        {
            return NotFound();
        }

        _unitOfWork.YearlyHolidaySchedule.Remove(obj.YearlyHolidaySchedule);
        _unitOfWork.Save();
        TempData["success"] = "Delete - success";
        return RedirectToAction("Index");
    }
}
