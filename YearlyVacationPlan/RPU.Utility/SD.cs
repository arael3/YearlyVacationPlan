namespace RPU.Utility;

public static class SD  // Static Details
{
    public const string Role_User_Indi = "Individual";
    public const string Role_User_Comp = "Company";
    public const string Role_Admin = "Admin";
    public const string Role_Employee = "Employee";

    // order statuses
    public const string StatusPending = "Pending";
    public const string StatusApproved = "Approved";
    public const string StatusInProcess = "Processing";
    public const string StatusShipped = "Shipped";
    public const string StatusCancelled = "Cancelled";
    public const string StatusRefunded = "Refunded";

    // payment statuses
    public const string PaymentStatusPending = "Pending";
    public const string PaymentStatusApproved = "Approved";
    public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
    public const string PaymentStatusRejected = "Rejected";

    public const string SessionCart = "SessionShoppingCart";
    public const string SessionPendingSharedVacationPlan = "SessionPendingSharedVacationPlan";

    public const double StripeMinimumPayment = 15.00;

    // roczny plan urlopu

    public const string VacationDayDateFormat = "dd.MM.yyyy HH:mm:ss";

    public const int NumberOfDaysBeforeVacation = 7;

    public static List<int> Years = new List<int> { 2023, 2024 };

    public static List<string> Status = new List<string> { "Aktywny", "Nieaktywny" };


    // Notifications
    public const string GoogleCaptchaMessage = "Ups... reCAPTCHA nie pozwolił na wykonanie operacji.";

}
