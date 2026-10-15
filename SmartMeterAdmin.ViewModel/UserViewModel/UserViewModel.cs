using System;

namespace SmartMeterAdmin.ViewModel.UserViewModel
{
    public class UserViewModel
    {
        public int UserID { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string RoleType { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public string AlternetMobileNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
        public DateTime? Dob { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNo { get; set; }
        public string LastQualification { get; set; }       
        public string DocumentUrl { get; set; }
        public string Password { get; set; }
        public string ProfileImage { get; set; }
        public string WhatsappNo { get; set; }
        public string AreaType { get; set; }
        public string AreaValue { get; set; }
        public string ZoneName { get; set; }

        // Audit access (Add New Admin User form se aata hai; null = proc default 1/1)
        // CanDelete hata diya - sirf 2 permissions: View | Edit & Delete (CanEdit = combined)
        public bool? CanView { get; set; }
        public bool? CanEdit { get; set; }
        public string CircleName { get; set; }
        public string DivisionName { get; set; }
        public string SubstationName { get; set; }
        public string FeederName { get; set; }
        public string DTName { get; set; }
        public string CompanyName { get; set; }
        public string CreatedDate { get; set; }

    }
}
