namespace SmartMeterAdmin.ViewModel.Login
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AccountType { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Access { get; set; }   

    }
}
