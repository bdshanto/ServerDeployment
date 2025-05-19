using System.ComponentModel.DataAnnotations;

namespace ARAKDataSetup.Domains.ServerAccessDto;

/// <summary>
/// SPECIES_ID	SPECIES_UID	SPECIES_TEXT	SPECIES_NAME	SPECIES_DESC	WEIGHT	MEASURE	INCREMENT	IS_CAT	IS_DOG	IS_ACTIVE	ORG_ID	CREATED_BY	CREATED_ON	LAST_MODIFIED_BY	LAST_MODIFIED_ON
/// </summary>
public class SpeciesDto
{
    [Key]
    public int SPECIES_ID { get; set; }
    public string SPECIES_UID { get; set; }
    public string SPECIES_TEXT { get; set; }
    public string SPECIES_NAME { get; set; }
    public string SPECIES_DESC { get; set; }
    public bool IS_ACTIVE { get; set; } = true;
    public int ORG_ID { get; set; }
    public int CREATED_BY { get; set; }
    public DateTime CREATED_ON { get; set; }
}