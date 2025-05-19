using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ARAKDataSetup.Domains.ServerAccessDto;

public class PmrReasonTypeDto
{
    [Key]
    public int ID { get; set; }

    public int ENCOUNTER_ID { get; set; }

    public int HR_UNIT_ID { get; set; }

    [MaxLength(300)]
    public string REASON_TYPE { get; set; } = string.Empty;

    public int ORG_ID { get; set; }

    public int CREATED_BY_ID { get; set; }

    public DateTime CREATED_ON { get; set; } = DateTime.UtcNow;

    public int? LAST_MODIFIED_BY_ID { get; set; }

    public DateTime? LAST_MODIFIED_ON { get; set; }

}