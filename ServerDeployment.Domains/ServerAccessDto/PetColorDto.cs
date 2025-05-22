using System.ComponentModel.DataAnnotations;

namespace ServerDeployment.Domains.ServerAccessDto;

public class PetColorDto
{
    [Key]
    public int COLOR_ID { get; set; }
    public string COLOR_TEXT { get; set; }
    public string COLOR_DESC { get; set; }
    public bool IS_ACTIVE { get; set; }

    public int ORG_ID { get; set; }
    public int CREATED_BY { get; set; }
    public DateTime CREATED_ON { get; set; }
}