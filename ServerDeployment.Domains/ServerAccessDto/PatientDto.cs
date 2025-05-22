using System.ComponentModel.DataAnnotations;

namespace ServerDeployment.Domains.ServerAccessDto;
public class PatientDto
{
    public PatientDto()
    {

    }

    public PatientDto(
        string patientName,
        string hn,
        string patientGender,
        DateTime patientDob,
        bool dobUnknown,
        int speciesId,
        int breedId,
        bool breedIsMixed,
        int colorId,
        string patientStatus,
        int orgId

        ) : this()
    {
        PATIENT_NAME = patientName;
        HN = hn;



    }


    [Key]
    public int PATIENT_ID { get; set; }
    public string HN { get; set; }
    public string PATIENT_NAME { get; set; }
    public string PATIENT_GENDER { get; set; }
    public DateTime? PATIENT_DOB { get; set; }
    public bool DOB_UNKNOWN { get; set; }
    public bool IS_NEUTER { get; set; }
    public DateTime? NEUTERED_DATE { get; set; }
    public int SPECIES_ID { get; set; }
    public int BREED_ID { get; set; }
    public int COLOR_ID { get; set; }
    public int OWNER_ID { get; set; }
    public int ORG_ID { get; set; }
    public int CREATED_BY { get; set; }
    public DateTime CREATED_ON { get; set; }

    public int? LAST_MODIFIED_BY { get; set; }
    public DateTime? LAST_MODIFIED_ON { get; set; }
    public string Remarks { get; set; }

    public void GetDoB(string petDob)
    {
        var dob = string.Empty;
        /* try
         {
             if (AppUtility.HasStrValue(petDob))
             {
                 PATIENT_DOB = Convert.ToDateTime(petDob);
                 return;
             }
         }
         catch (Exception ex)
         {*/
        dob = petDob;

        /* }*/
        dob = dob.Trim();

        if (dob.ToLowerInvariant() == "-".ToLowerInvariant())
        {
            PATIENT_DOB = DateTime.UtcNow;
            return;
        }

        if (string.IsNullOrEmpty(dob))
        {
            PATIENT_DOB = DateTime.UtcNow;
            return;
        }

        if (dob.Length == 4)
        {
            PATIENT_DOB = new DateTime(Convert.ToInt32(dob), 1, 1);
            return;
        }

        if (dob.Length < 2)
        {
            PATIENT_DOB = DateTime.UtcNow;
            return;
        }

        // split  by '/' 
        if (dob.ToLowerInvariant().Contains("/".ToLowerInvariant()))
        {
            if (dob.ToLowerInvariant().Contains("//".ToLowerInvariant()))
            {
                PATIENT_DOB = DateTime.UtcNow;
                return;
            }
            var split = dob.Split('/');
            if (split.Length == 3)
            {

                if (split[2].Length == 4)
                {
                    PATIENT_DOB = new DateTime(Convert.ToInt32(split[2]), Convert.ToInt32(split[1]), Convert.ToInt32(split[0]));
                }
                else
                {
                    PATIENT_DOB = DateTime.UtcNow;
                }
                return;
            }

            if (split.Length == 2)
            {
                if (split[1].Length == 4 && (split[0].Length == 2 || split[0].Length == 1))
                {
                    if (Convert.ToInt32(split[0]) > 12)
                    {
                        PATIENT_DOB = DateTime.UtcNow;
                    }
                    else
                    {
                        PATIENT_DOB = new DateTime(Convert.ToInt32(split[1]), Convert.ToInt32(split[0]), 1);
                    }
                }
                else if (split[0].Length == 4 && split[1].Length <= 2)
                {
                    PATIENT_DOB = new DateTime(Convert.ToInt32(split[0]), Convert.ToInt32(split[1]), 1);
                }
                else
                {
                    PATIENT_DOB = DateTime.UtcNow;
                }
                return;
            }

            if (split.Length == 1 && split[0].Length > 3)
            {
                if (split[0].Length == 4)
                {
                    PATIENT_DOB = new DateTime(Convert.ToInt32(split[0]), 1, 1);
                }
                else
                {
                    PATIENT_DOB = DateTime.UtcNow;
                }
                return;
            }
            PATIENT_DOB = DateTime.UtcNow;
            return;
        }

        if (!dob.ToLowerInvariant().Contains("-".ToLowerInvariant())) return;
        {
            var split = dob.Split('/');
            if (split.Length == 3)
            {

                if (split[2].Length == 4)
                {
                    PATIENT_DOB = new DateTime(Convert.ToInt32(split[2]), Convert.ToInt32(split[1]), Convert.ToInt32(split[0]));
                }
                else
                {
                    PATIENT_DOB = DateTime.UtcNow;
                }
                return;
            }

            if (split.Length == 2)
            {
                if (split[1].Length == 4 && (split[0].Length == 2 || split[0].Length == 1))
                {
                    PATIENT_DOB = new DateTime(Convert.ToInt32(split[1]), Convert.ToInt32(split[0]), 1);
                }
                else if (split[0].Length == 4 && split[1].Length <= 2)
                {
                    PATIENT_DOB = new DateTime(Convert.ToInt32(split[0]), Convert.ToInt32(split[1]), 1);
                }
                else
                {
                    PATIENT_DOB = DateTime.UtcNow;
                }
                return;
            }

            if (split.Length == 1 && split[0].Length > 3)
            {
                if (split[0].Length == 4)
                {
                    PATIENT_DOB = new DateTime(Convert.ToInt32(split[0]), 1, 1);
                }
                else
                {
                    PATIENT_DOB = DateTime.UtcNow;
                }
                return;
            }
            PATIENT_DOB = DateTime.UtcNow;
        }


    }
}