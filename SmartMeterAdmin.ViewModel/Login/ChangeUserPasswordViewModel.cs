namespace SmartMeterAdmin.ViewModel.Login
{
    public class ChangeUserPasswordViewModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public int? userId
        {
            get; set;
        }
    }
}
