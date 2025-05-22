using System.ComponentModel.DataAnnotations;

namespace ServerDeployment.Domains.ServerAccessDto;

public class HrUnit
{
    [Key]
    public int UNIT_ID { get; set; }

    [MaxLength(30)]
    public string? UNIT_UID { get; set; }

    public int? UNIT_ID_PARENT { get; set; }

    [MaxLength(100)]
    public string? UNIT_NAME { get; set; }

    [MaxLength(100)]
    public string? UNIT_ALIAS { get; set; }

    [MaxLength(50)]
    public string? PHONE_NO { get; set; }

    [MaxLength(500)]
    public string? DESCR { get; set; }

    [MaxLength(1)]
    public string? UNIT_TYPE { get; set; }

    [MaxLength(1)]
    public string? IS_EXTERNAL { get; set; }

    [MaxLength(100)]
    public string? LOCATION { get; set; }

    public int? ORG_ID { get; set; }

    public int? CREATED_BY { get; set; }

    public DateTime? CREATED_ON { get; set; }

    public int? LAST_MODIFIED_BY { get; set; }

    public DateTime? LAST_MODIFIED_ON { get; set; }
}