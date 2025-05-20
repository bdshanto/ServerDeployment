using System.ComponentModel.DataAnnotations;

namespace ARAKDataSetup.Domains.ServerAccessDto;

public class EncounterTypeDto
{
    [Key]
    public int ENCOUNTER_ID { get; set; }

    [MaxLength(50)]
    public string? ENCOUNTER_CODE { get; set; }

    public int? PARENT_ENCOUNTER_ID { get; set; }

    [MaxLength(250)]
    public string? ENCOUNTER_TYPE_EN { get; set; }

    [MaxLength(250)]
    public string? ENCOUNTER_TYPE_TH { get; set; }

    public bool IS_ACTIVE { get; set; }

    public int? ORG_ID { get; set; }

    public int? CREATED_BY { get; set; }

    public DateTime? CREATED_ON { get; set; }

    public int? LAST_MODIFIED_BY { get; set; }

    public DateTime? LAST_MODIFIED_ON { get; set; }
}