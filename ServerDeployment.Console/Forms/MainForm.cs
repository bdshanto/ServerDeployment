using ARAKDataSetup.Domains.ServerAccessDto;
using ARAKDataSetup.Domains.Utility;
using ARAKDataSetup.Domains.ViewModels;
using ARAKDataSetup.infrastructure.Contracts.IDataAccessor;
using ARAKDataSetup.infrastructure.Factories;
using OfficeOpenXml;

namespace ServerDeployment.Console.Forms
{
    public partial class MainForm : Form
    {
        private Dictionary<string, List<ExcelVm>> _allSheetData;
        List<ExcelVm> _importedDataList = new();

        private int _userId = 0;
        private int _orgId = 0;

        private delegate void UpdateCount(int count);

        private readonly IDataAccess _iDataAccess;

        public MainForm()
        {
            InitializeComponent();
            _iDataAccess = new DataAccessFactory().ServerDataAccess();
        }

        private async void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new())
            {
                // openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                openFileDialog.Filter = @"Excel Files|*.xlsx;";
                openFileDialog.FilterIndex = 2;
                // openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lblFileName.Text = openFileDialog.FileName;
                }
                else
                {
                    return;
                }

                if (AppUtility.HasNoStr(lblFileName.Text))
                {
                    MessageBox.Show(@"Please select a file to proceed", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _importedDataList.Clear();
                lblFileStatus.Text = @"Reading data...";
                _allSheetData = await Task.Run(() => ReadDataFromExcel(lblFileName.Text));
                foreach (var sheetData in _allSheetData)
                {
                    _importedDataList.AddRange(sheetData.Value);
                }
                lblFileStatus.Text = $"Total Sheet:{_allSheetData.Count}. Total data: {_importedDataList.Count} ";

            }
        }

        private static Dictionary<string, List<ExcelVm>> ReadDataFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var result = new Dictionary<string, List<ExcelVm>>();
            FileInfo fileInfo = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                var ass = package.Workbook.Worksheets;
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    var sheetData = new List<ExcelVm>();
                    int rowCount = sheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Assuming the first row contains headers
                    {
                        var data = new ExcelVm();
                        data.ClientId = AppUtility.TrimStr(sheet.Cells[row, 1].Text);
                        data.ClientName = AppUtility.TrimStr(sheet.Cells[row, 2].Text);
                        data.FirstName = data.ClientName.Split(' ')[0] ?? "";
                        // the remaining part of the last name
                        data.LastName = data.ClientName.Replace(data.FirstName, "").Trim();
                        data.SecondaryOwnerName = AppUtility.TrimStr(sheet.Cells[row, 3].Text);
                        data.PhoneNumber = AppUtility.TrimStr(sheet.Cells[row, 4].Text);
                        data.ClientId = AppUtility.TrimStr(sheet.Cells[row, 5].Text);
                        data.CustomerType = AppUtility.TrimStr(sheet.Cells[row, 6].Text);
                        data.OrganisationName = AppUtility.TrimStr(sheet.Cells[row, 7].Text);
                        data.VatNumber = AppUtility.TrimStr(sheet.Cells[row, 8].Text);
                        data.StreetAddress = AppUtility.TrimStr(sheet.Cells[row, 9].Text);
                        data.PostCode = AppUtility.TrimStr(sheet.Cells[row, 10].Text);
                        data.City = AppUtility.TrimStr(sheet.Cells[row, 11].Text);
                        data.Country = AppUtility.TrimStr(sheet.Cells[row, 12].Text);
                        data.Email = AppUtility.TrimStr(sheet.Cells[row, 13].Text);
                        data.AltEmail = AppUtility.TrimStr(sheet.Cells[row, 14].Text);
                        data.ClientCriticalNote = AppUtility.TrimStr(sheet.Cells[row, 15].Text);
                        data.ClientRemarks = AppUtility.TrimStr(sheet.Cells[row, 16].Text);
                        data.ClientTags = AppUtility.TrimStr(sheet.Cells[row, 17].Text);
                        data.PatientId = AppUtility.TrimStr(sheet.Cells[row, 18].Text);
                        data.PatientName = AppUtility.TrimStr(sheet.Cells[row, 19].Text);
                        data.Species = AppUtility.TrimStr(sheet.Cells[row, 20].Text);
                        data.Breed = AppUtility.TrimStr(sheet.Cells[row, 21].Text);
                        data.Microchip = AppUtility.TrimStr(sheet.Cells[row, 22].Text);

                        var gender = AppUtility.TrimStr(sheet.Cells[row, 23].Text);
                        // comma seperated 
                        var selectedGender = gender.Split(',');
                        data.Sex = AppUtility.TrimStr(selectedGender[0]);
                        if (selectedGender.Length > 1)
                        {
                            data.IsNeutered = true;
                        }
                        data.Dob = AppUtility.TrimStr(sheet.Cells[row, 24].Text);
                        data.Archived = AppUtility.TrimStr(sheet.Cells[row, 25].Text);
                        data.DeceasedDate = AppUtility.TrimStr(sheet.Cells[row, 26].Text);
                        data.ReasonOfDeath = AppUtility.TrimStr(sheet.Cells[row, 27].Text);
                        data.Colour = AppUtility.TrimStr(sheet.Cells[row, 28].Text);
                        data.PatientCriticalNotes = AppUtility.TrimStr(sheet.Cells[row, 29].Text);
                        data.Remarks = AppUtility.TrimStr(sheet.Cells[row, 30].Text);
                        data.InsuranceNumber = AppUtility.TrimStr(sheet.Cells[row, 31].Text);
                        data.PassportNumber = AppUtility.TrimStr(sheet.Cells[row, 32].Text);
                        data.OfficialName = AppUtility.TrimStr(sheet.Cells[row, 33].Text);
                        data.BloodGroup = AppUtility.TrimStr(sheet.Cells[row, 34].Text);
                        data.LastVisit = AppUtility.TrimStr(sheet.Cells[row, 35].Text);
                        data.LastReason = AppUtility.TrimStr(sheet.Cells[row, 36].Text);
                        data.PatientTags = AppUtility.TrimStr(sheet.Cells[row, 37].Text);
                        sheetData.Add(data);
                    }

                    result.Add(sheet.Name, sheetData);
                }
            }

            return result;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var user = txtUser.Text.Trim();
                var pass = txtPassword.Text.Trim();

                if (AppUtility.HasNoStr(user) || AppUtility.HasNoStr(pass))
                {
                    lblLogin.Text = @"Please enter username and password";
                    return;
                }

                var userDto = _iDataAccess
                    .GetAll<UserDto>($"select  * from HR_USER where [USER_NAME]='{user}' or EMAIL_OFFICIAL = '{user}'")
                    .FirstOrDefault();
                if (userDto is null)
                {
                    lblLogin.Text = @"Invalid username or password";
                    return;
                }

                var isValid = VerifyPasswordHash(pass, userDto.PWD_HASH, userDto.PWD_SALT);
                if (isValid)
                {
                    _userId = userDto.USER_ID;
                    lblUserId.Text = @"User ID: " + userDto.USER_ID.ToString();

                    lblOrgId.Text = @"ORG ID: " + userDto.ORG_ID.ToString();
                    _orgId = +userDto.ORG_ID;

                    lblLogin.Text = @"Login success";
                }
                else
                {
                    lblLogin.Text = @"Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                lblLogin.Text = ex.Message + Environment.NewLine + ex.InnerException ?? "" + Environment.NewLine + ex.Source;
                //throw;
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i]) return false;
            }

            return true;
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            lblLogin.Text = "";
            lblStatus.Text = "";
            lblCompleted.Text = @"0";

            if (_orgId <= 0 || _userId <= 0)
            {
                lblLogin.Text = @"Please login to proceed";
                return;
            }

            if (_allSheetData == null || !_allSheetData.Any() || _allSheetData.Count <= 0)
            {
                lblLogin.Text = @"Please select a file to proceed";
                return;
            }
            if (_allSheetData.Count == 0)
            {
                lblLogin.Text = @"No data found in the file";
                return;
            }

            //this.lblTotal.Text = $"Total Sheet:{_allSheetData.Count}. Total data: {importedDataList.Count} ";

            ICollection<ProvinceDto> provinceList = await _iDataAccess.GetAllAsync<ProvinceDto>("SELECT * FROM GBL_PROVINCE");
            ICollection<DistrictDto> districtList = await _iDataAccess.GetAllAsync<DistrictDto>("SELECT * FROM GBL_DISTRICT");
            ICollection<SubDistrictDto> subDistrictList = await _iDataAccess.GetAllAsync<SubDistrictDto>("SELECT * FROM GBL_SUBDISTRICT");

            ICollection<SpeciesDto> speciesList = await _iDataAccess.GetAllAsync<SpeciesDto>($"SELECT * FROM PMS_SPECIES where ORG_ID={_orgId}");
            ICollection<BreedDto> breedList = await _iDataAccess.GetAllAsync<BreedDto>($"SELECT * FROM PMS_BREED where ORG_ID={_orgId}");
            ICollection<PetColorDto> petColorList = await _iDataAccess.GetAllAsync<PetColorDto>($"SELECT * FROM PMS_PETCOLOR where ORG_ID={_orgId}");

            ICollection<ClientDto> clientList = await _iDataAccess.GetAllAsync<ClientDto>($"SELECT * FROM PMS_CLIENT where ORG_ID={_orgId}");
            ICollection<PatientDto> petList = await _iDataAccess.GetAllAsync<PatientDto>($"SELECT * FROM PMS_PATIENT where org_id={_orgId}");
            //var hn = petList.OrderByDescending(c => c.PATIENT_ID).Select(c => c.HN).FirstOrDefault();

            var organization = (await _iDataAccess.GetAllAsync<GBLEnvDto>($"SELECT TOP 1 * FROM GBL_ENV WHERE ORG_ID={_orgId}")).FirstOrDefault();

            var hn = (await _iDataAccess.GetAllAsync<PatientDto>($"SELECT TOP 1 * FROM PMS_PATIENT WHERE ORG_ID={_orgId} ORDER BY PATIENT_ID DESC")).FirstOrDefault();

            HashSet<string> uniquePhoneNumbers = new HashSet<string>();

            _importedDataList.ForEach(c => uniquePhoneNumbers.Add(c.PhoneNumber));

            int countNumber = 1;

            foreach (var data in _importedDataList)
            {

                /*var province = provinceList.FirstOrDefault(x =>
                AppUtility.TrimStr(x.PROVINCE_NAME.ToLowerInvariant()) ==
                AppUtility.TrimStr(data.Province.ToLowerInvariant()));
            if (province is null)
            {
                province = ProvinceDto.GetDto(data.Province);

                var provinceId = await _iDataAccess.InsertAsync(province, "GBL_PROVINCE");
                province.PROVINCE_ID = Convert.ToInt32(provinceId);

                provinceList.Add(province);
            }*/
                var province = provinceList.FirstOrDefault()!;

                var district = districtList.FirstOrDefault(x =>
                     AppUtility.TrimStr(x.DISTRICT_NAME.ToLowerInvariant()) ==
                     AppUtility.TrimStr(data.City.ToLowerInvariant()));

                if (district is null)
                {
                    district = DistrictDto.GetDto(data.City, province!.PROVINCE_ID);

                    var districtId = await _iDataAccess.InsertAsync(district, "GBL_DISTRICT");
                    district.DISTRICT_ID = Convert.ToInt32(districtId);

                    districtList.Add(district);
                }


                /* var subDistrict = subDistrictList.FirstOrDefault(x =>
                     AppUtility.TrimStr(x.SUBDISTRICT_NAME.ToLowerInvariant()) ==
                     AppUtility.TrimStr(data.SubDistrict.ToLowerInvariant()));

                 if (subDistrict is null)
                 {
                     subDistrict = SubDistrictDto.GetDto(data.SubDistrict, district!.DISTRICT_ID);

                     var subDistrictId = await _iDataAccess.InsertAsync(subDistrict, "GBL_SUBDISTRICT");
                     subDistrict.SUBDISTRICT_ID = Convert.ToInt32(subDistrictId);

                     subDistrictList.Add(subDistrict);
                 }*/

                var clientDto = clientList.FirstOrDefault(c =>
                    AppUtility.TrimAllSpaceStr(c.PHONE_MOBILE.Trim()).ToLowerInvariant() ==
                    AppUtility.TrimAllSpaceStr(data.PhoneNumber.Trim()).ToLower());

                if (clientDto is null)
                {
                    clientDto = new ClientDto(
                        data.AltEmail, data.Email, data.StreetAddress,
                        data.FirstName, data.LastName, data.ClientIdNumber,
                        data.PhoneNumber, data.PostCode, district.DISTRICT_ID,
                        province.PROVINCE_ID, _orgId, _userId);

                    var clientId = await _iDataAccess.InsertAsync(clientDto, "PMS_CLIENT");
                    clientDto.CLIENT_ID = Convert.ToInt32(clientId);
                    clientList.Add(clientDto);
                }
                else
                {

                    clientDto.ADDRESS1 = data.StreetAddress;
                    clientDto.FNAME_NLS = data.FirstName;
                    clientDto.LNAME_NLS = data.LastName;
                    clientDto.IDENTIFICATION_NUMBER = data.ClientIdNumber;
                    clientDto.PHONE_MOBILE = data.PhoneNumber;
                    clientDto.ZIP_CODE = data.PostCode;
                    clientDto.DISTRICT_ID = district.DISTRICT_ID;
                    clientDto.PROVINCE_ID = province.PROVINCE_ID;
                    clientDto.LAST_MODIFIED_BY = _userId;
                    clientDto.LAST_MODIFIED_ON = DateTime.UtcNow;

                    var isUpdated = await _iDataAccess.UpdateAsync(clientDto, "PMS_CLIENT") > 0;

                }

                var speciesDto = speciesList.FirstOrDefault(s =>
                    AppUtility.TrimAllSpaceStr(s.SPECIES_NAME).ToLowerInvariant() ==
                    AppUtility.TrimAllSpaceStr(data.Species).ToLower());

                if (speciesDto is null)
                {
                    speciesDto = new SpeciesDto()
                    {
                        SPECIES_NAME = data.Species,
                        ORG_ID = _orgId,
                        IS_ACTIVE = true,
                        CREATED_BY = _userId,
                        CREATED_ON = DateTime.UtcNow,
                        SPECIES_DESC = data.Species,
                        SPECIES_TEXT = data.Species,
                        SPECIES_UID = data.Species,
                        SPECIES_ID = 0,

                    };

                    var speciesId = await _iDataAccess.InsertAsync(speciesDto, "PMS_SPECIES");
                    speciesDto.SPECIES_ID = Convert.ToInt32(speciesId);
                    speciesList.Add(speciesDto);
                }

                var breedDto = breedList.FirstOrDefault(b =>
                    AppUtility.TrimAllSpaceStr(b.BREED_TEXT).ToLowerInvariant() ==
                    AppUtility.TrimAllSpaceStr(data.Breed).ToLower());

                if (breedDto is null)
                {

                    breedDto = new BreedDto()
                    {
                        BREED_TEXT = data.Breed.Length > 99 ? data.Breed.Substring(0, 99) : data.Breed,
                        ORG_ID = _orgId,
                        IS_ACTIVE = true,
                        CREATED_BY = _userId,
                        CREATED_ON = DateTime.UtcNow,
                        SPECIES_ID = speciesDto.SPECIES_ID
                    };

                    var breedId = await _iDataAccess.InsertAsync(breedDto, "PMS_BREED");
                    breedDto.BREED_ID = Convert.ToInt32(breedId);
                    breedList.Add(breedDto);
                }

                var colorDto = petColorList.FirstOrDefault(p => AppUtility.TrimAllSpaceStr(p.COLOR_TEXT).ToLowerInvariant() ==
                                                       AppUtility.TrimAllSpaceStr(data.Colour).ToLower());
                if (colorDto is null)
                {
                    colorDto = new PetColorDto()
                    {
                        COLOR_TEXT = data.Colour.Length > 99 ? data.Colour.Substring(0, 99) : data.Colour,
                        ORG_ID = _orgId,
                        CREATED_BY = _userId,
                        IS_ACTIVE = true,
                        CREATED_ON = DateTime.UtcNow,
                    };

                    var colorId = await _iDataAccess.InsertAsync(colorDto, "PMS_PETCOLOR");
                    colorDto.COLOR_ID = Convert.ToInt32(colorId);
                    petColorList.Add(colorDto);
                }


                var petDto = petList.FirstOrDefault(p => AppUtility.TrimAllSpaceStr(p.HN).ToLowerInvariant() ==
                                                         AppUtility.TrimAllSpaceStr(data.PatientId).ToLower());
                if (petDto is null)
                {
                    var remarks = data.Remarks ?? "";
                    if (data.PatientCriticalNotes.Length > 0)
                    {
                        remarks += Environment.NewLine + data.PatientCriticalNotes;
                    }

                    petDto = new PatientDto()
                    {
                        OWNER_ID = clientDto.CLIENT_ID,
                        ORG_ID = clientDto.ORG_ID,
                        CREATED_BY = _userId,
                        CREATED_ON = DateTime.UtcNow,
                        BREED_ID = breedDto.BREED_ID,
                        COLOR_ID = colorDto.COLOR_ID,
                        DOB_UNKNOWN = false,
                        IS_NEUTER = data.IsNeutered,
                        NEUTERED_DATE = data.IsNeutered ? DateTime.UtcNow : null,
                        Remarks = remarks,

                        PATIENT_ID = 0,
                        PATIENT_NAME = data.PatientName,
                        SPECIES_ID = speciesDto.SPECIES_ID
                    };
                    petDto.HN = $"{organization!.ORG_UID}-{countNumber}";

                    petDto.PATIENT_GENDER = AppUtility.GetGenderValue(data.Sex);
                    petDto.GetDoB(data.Dob);
                    var petId = await _iDataAccess.InsertAsync(petDto, "PMS_PATIENT");
                    petDto.PATIENT_ID = Convert.ToInt32(petId);
                    petList.Add(petDto);
                }
                else
                {
                    petDto.HN = data.PatientId;
                    petDto.OWNER_ID = clientDto.CLIENT_ID;
                    petDto.ORG_ID = clientDto.ORG_ID;
                    petDto.CREATED_BY = _userId;
                    petDto.CREATED_ON = DateTime.UtcNow;
                    petDto.BREED_ID = breedDto.BREED_ID;
                    petDto.COLOR_ID = colorDto.COLOR_ID;
                    petDto.DOB_UNKNOWN = false;
                    petDto.LAST_MODIFIED_BY = _userId;
                    petDto.LAST_MODIFIED_ON = DateTime.UtcNow;
                    petDto.GetDoB(data.Dob);

                    var isUpdated = await _iDataAccess.UpdateAsync(petDto, "PMS_PATIENT") > 0;
                }
                UpdateRecordNumber();

            }
            lblStatus.Text = @"Data imported successfully";
            lblStatus.ForeColor = Color.Green;
        }


        private void UpdateRecordNumber(int count = 1)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateCount(UpdateRecordNumber), new object[] { count });
            }
            else
            {
                int exTotal = int.Parse(this.lblCompleted.Text.Trim());
                exTotal += count;
                this.lblCompleted.Text = exTotal.ToString();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}