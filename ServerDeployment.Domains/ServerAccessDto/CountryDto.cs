using System.ComponentModel.DataAnnotations;

namespace ARAKDataSetup.Domains.ServerAccessDto;

/// <summary>
/// GBL_COUNTRY
/// </summary>
public class CountryDto
{
    [Key]
    public int COUNTRY_ID { get; set; }
    public string Country_CODE { get; set; }
    public string COUNTRY_NAME { get; set; }public bool IS_ACTIVE { get; set; } = true;
}