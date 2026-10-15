namespace SmartMeterAdmin.ViewModel.Login
{
    public class ElectricianLoginResponse
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string ProfileImage { get; set; }
        public string AccessToken { get; set; }
        public string CompanyName { get; set; }
        public string Roletype { get; set; }
        public byte[] Password { get; set; }
        public int UserID { get; set; }
    }
}
