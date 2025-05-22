using System.ComponentModel.DataAnnotations;

namespace ServerDeployment.Domains.ServerAccessDto;

/// <summary>
/// GBL_DISTRICT
/// </summary>
public class DistrictDto
{
    [Key]
    public int DISTRICT_ID { get; set; }
    public string DISTRICT_CODE { get; set; }
    public string DISTRICT_NAME { get; set; }
    public int? PROVINCE_ID { get; set; }
    public bool IS_ACTIVE { get; set; } = true;

    public static DistrictDto GetDto(string dataDistrict, int provinceId)
    {
        return new DistrictDto()
        {
            DISTRICT_NAME = dataDistrict,
            DISTRICT_CODE = "00",
            DISTRICT_ID = 0,
            PROVINCE_ID = provinceId
        };
    }
}