using System;

namespace Eduplos.Domain.CoreModule
{
    public class UserData
    {
        public int UserDataId { get; set; } 
        public string InstitutionName { get; set; }
        public string AffiliateInfo { get; set; }
        public string ContactPerson { get; set; }
        public string InstitutionCode { get; set; }
        public string InstitutionType { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Email1 { get; set; }
        public string AppEmail { get; set; }
        public string EmailDomain { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public bool EnableSsl { get; set; }
        public string Url { get; set; }
        public byte[] Logo { get; set; }
        public byte[] RegSign { get; set; }
        public byte[] Regbanner { get; set; }
        public byte[] RegFooter { get; set; }
        public float WataOpacity { get; set; }
        public float HWata { get; set; }
        public float VWata { get; set; }
        public float WataHeight { get; set; }
        public float WataWidth { get; set; }
        public bool EnableWata { get; set; }

        
    }
}
