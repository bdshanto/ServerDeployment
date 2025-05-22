using System.ComponentModel.DataAnnotations;

namespace ServerDeployment.Domains.ServerAccessDto;

public class BreedDto
{
    [Key]
    public int BREED_ID { get; set; }
    public string BREED_TEXT { get; set; }
    public string BREED_DESC { get; set; }
    public int? SPECIES_ID { get; set; }
    public bool IS_ACTIVE { get; set; } = true;
    public int ORG_ID { get; set; }
    public int CREATED_BY { get; set; }
    public DateTime CREATED_ON { get; set; }
}