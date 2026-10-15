using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartMeterAdmin.Data.BaseClass;
namespace SmartMeterAdmin.Data.User
{
    [Table("Users")]
    public class Users : BaseEntity
    {
        [Key]

        public int UserID { get; set; }
        public string RoleType { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
        public DateTime Dob { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNo { get; set; }
        public string LastQualification { get; set; }       
        public string DocumentUrl { get; set; }
        public string ProfileImage { get; set; }
        public string CompanyName { get; set; }
        public byte[] Password { get; set; }
      
        public int? SubDivisionID { get; set; }
        public int? SubStationID { get; set; }
        public int? FeederID { get; set; }
        public int? DTID { get; set; }
        public string WhatsappNo { get; set; }
        public string AreaType { get; set; }
        public string AreaValue { get; set; }

    }
}
