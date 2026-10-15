namespace SmartMeterAdmin.ViewModel.Login
{
    public class LoginResultData
    {
        public int UserID { get; set; }
        public string RoleType { get; set; }
        public string FullName { get; set; }
        public byte[] Password { get; set; }
        public bool Access { get; set; }    
    }
}
