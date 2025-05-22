using System.ComponentModel.DataAnnotations;

namespace ServerDeployment.Domains.ServerAccessDto;

/// <summary>
/// GBL_SUBDISTRICT
/// </summary>
public class SubDistrictDto
{
    [Key]
    public int SUBDISTRICT_ID { get; set; }
    public int DISTRICT_ID { get; set; }
    public string SUBDISTRICT_CODE { get; set; }
    public string SUBDISTRICT_NAME { get; set; }

    public static SubDistrictDto GetDto(string dataSubDistrict, int districtId)
    {
        return new SubDistrictDto()
        {
            SUBDISTRICT_NAME = dataSubDistrict,
            SUBDISTRICT_CODE = "00",
            SUBDISTRICT_ID = 0,
            DISTRICT_ID = districtId,
        };
    }
}