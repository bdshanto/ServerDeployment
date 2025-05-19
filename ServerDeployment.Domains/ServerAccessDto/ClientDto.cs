using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARAKDataSetup.Domains.Utility;
using ARAKDataSetup.Domains.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ARAKDataSetup.Domains.ServerAccessDto
{
    /*
    <FNAME_NLS, nvarchar(100),>
    ,<LNAME_NLS, nvarchar(100),>
    ,<GENDER, nvarchar(1),>
    ,<PHONE_MOBILE, nvarchar(200),>
    ,<ADDRESS1, nvarchar(100),>
    ,<DISTRICT_ID, int,>
    ,<COUNTY_ID, int,>
    ,<PROVINCE_ID, int,>
    ,<IS_CLINIC, bit,>
    ,<IDENTIFICATION_NUMBER, nvarchar(30),>
    ,<ORG_ID, int,>
    ,<CREATED_BY, int,>
    ,<CREATED_ON, datetime,>
    ,<COUNTRY_ID, int,>
    ,<ISACTIVE, bit,>
    ,<IS_OUTSOURCE, bit,>
    ,<SUBDISTRICT_ID, int,> 
     *
     */
    public class ClientDto
    {
        [Key]
        public int CLIENT_ID { get; set; }
        public string EMAIL_OFFICIAL { get; set; }
        public string EMAIL_PERSONAL { get; set; }
        public string ZIP_CODE { get; set; }
        public string FNAME_NLS { get; set; }
        public string LNAME_NLS { get; set; }
        public string GENDER { get; set; }
        public string PHONE_MOBILE { get; set; }
        public string ADDRESS1 { get; set; }
        public int? DISTRICT_ID { get; set; }

        public int PROVINCE_ID { get; set; }
        public string IDENTIFICATION_NUMBER { get; set; }

        public bool IS_OUTSOURCE { get; set; }
        public int? SUBDISTRICT_ID { get; set; }

        public bool ISACTIVE { get; set; } = true;
        public int ORG_ID { get; set; }
        public int CREATED_BY { get; set; }
        public DateTime CREATED_ON { get; set; }

        public int? LAST_MODIFIED_BY { get; set; }
        public DateTime? LAST_MODIFIED_ON { get; set; }





        public ClientDto()
        {

        }
        public ClientDto(string altEmail, string email,
            string address, string firstName, string lastName, string idCard, string phone, string zipCode,
            int districtId, int provinceId, int orgId, int userId, int clientId = 0, int? subDistrictId = null) : this()
        {
            ADDRESS1 = AppUtility.TrimStr(address);
            FNAME_NLS = AppUtility.TrimStr(firstName);
            LNAME_NLS = AppUtility.TrimStr(lastName);
            IDENTIFICATION_NUMBER = AppUtility.TrimStr(idCard);

            CREATED_BY = userId;
            CLIENT_ID = 0;
            GENDER = ""; //AppUtility.GetGenderValue(data.Sex);
            ISACTIVE = true;
            IS_OUTSOURCE = false;
            ORG_ID = orgId;

            PHONE_MOBILE = AppUtility.TrimStr(phone);
            CREATED_ON = DateTime.UtcNow;
            DISTRICT_ID = districtId != null ? districtId : 0;
            PROVINCE_ID = provinceId != null ? provinceId : 0;
            SUBDISTRICT_ID = subDistrictId;
            ZIP_CODE = zipCode;
            CLIENT_ID = clientId;
            EMAIL_OFFICIAL = email;
            EMAIL_PERSONAL = altEmail;

        }


    }
}
