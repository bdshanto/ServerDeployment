using System.ComponentModel.DataAnnotations;

namespace ARAKDataSetup.Domains.ServerAccessDto;


/// <summary>
/// GBL_PROVINCE
/// </summary>
public class ProvinceDto
{
    [Key]
    public int PROVINCE_ID { get; set; } = 0;
    public int COUNTRY_ID { get; set; }
    public string PROVINCE_CODE { get; set; }
    public string PROVINCE_NAME { get; set; }
    public bool IS_ACTIVE { get; set; } = true;

    public static ProvinceDto GetDto(string province)
    {
        return new ProvinceDto()
        {
            PROVINCE_NAME = province,
            PROVINCE_CODE = "00",
            PROVINCE_ID = 0,
            COUNTRY_ID = 4,
            IS_ACTIVE = true,
        };
    }
}