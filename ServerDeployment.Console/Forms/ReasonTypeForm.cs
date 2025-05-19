using ARAKDataSetup.Domains.ServerAccessDto;
using ARAKDataSetup.Domains.Utility;
using ARAKDataSetup.Domains.ViewModels;
using ARAKDataSetup.infrastructure.Contracts.IDataAccessor;
using ARAKDataSetup.infrastructure.Factories;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace ServerDeployment.Console.Forms
{
    public partial class ReasonTypeForm : Form
    {
        Dictionary<string, List<ReasonTypeExcelVm>> _allSheetData;
        List<ReasonTypeExcelVm> _importedDataList = new();

        private int _userId = 0;
        private int _orgId = 0;

        private delegate void UpdateCount(int count);

        private readonly IDataAccess _iDataAccess;

        public ReasonTypeForm()
        {
            InitializeComponent();
            _iDataAccess = new DataAccessFactory().ServerDataAccess();
        }

        #region Button Login
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
        #endregion

        #region Button brows

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
                lblFileStatus.Text = $@"Total Sheet:{_allSheetData.Count}. Total data: {_importedDataList.Count} ";

            }
        }

        private static Dictionary<string, List<ReasonTypeExcelVm>> ReadDataFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var result = new Dictionary<string, List<ReasonTypeExcelVm>>();
            FileInfo fileInfo = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                var ass = package.Workbook.Worksheets;
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    var sheetData = new List<ReasonTypeExcelVm>();
                    int rowCount = sheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Assuming the first row contains headers
                    {
                        var data = new ReasonTypeExcelVm();
                        data.Type = AppUtility.TrimStr(sheet.Cells[row, 1].Text);
                        data.Department = AppUtility.TrimStr(sheet.Cells[row, 2].Text);
                        data.ReasonType = AppUtility.TrimStr(sheet.Cells[row, 3].Text);
                        if (AppUtility.HasAnyStr(data.Type) && AppUtility.HasAnyStr(data.Department) && AppUtility.HasAnyStr(data.ReasonType))
                        {
                            sheetData.Add(data);
                        }

                    }
                    if (sheetData.Count > 0)
                    {
                        result.Add(sheet.Name, sheetData);
                    }
                }
            }

            return result;
        }


        #endregion

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
            ICollection<EncounterTypeDto> types = await _iDataAccess.GetAllAsync<EncounterTypeDto>($"select * from PMS_ENCOUNTERTYPE WHERE PARENT_ENCOUNTER_ID IS NOT NULL AND ORG_ID = {_orgId}");
            ICollection<HrUnit> departments = await _iDataAccess.GetAllAsync<HrUnit>($"SELECT * FROM HR_UNIT WHERE ORG_ID = {_orgId}");
            ICollection<PmrReasonTypeDto> reasonTypes = await _iDataAccess.GetAllAsync<PmrReasonTypeDto>($"SELECT * FROM PMR_REASON_TYPE WHERE ORG_ID = {_orgId}");

            foreach (var pair in _allSheetData)
            {
                foreach (var row in _allSheetData[pair.Key])
                {
                    row.Type = row.Type.ToUpperInvariant();
                    row.Department = row.Department.ToUpperInvariant();

                    if (row.Type == "OPD")
                        row.Type = "CONSULTATION";


                    if (row.Type == "IPD")
                        row.Type = "ADMISSION";

                    var type = types.FirstOrDefault(c => c.ENCOUNTER_CODE?.ToUpperInvariant() == row.Type.ToUpperInvariant());

                    if (type is null && AppUtility.HasAnyStr(row.Type))
                    {
                        type = new EncounterTypeDto()
                        {
                            ENCOUNTER_CODE = row.Type,
                            ENCOUNTER_TYPE_EN = row.Type,
                            IS_ACTIVE = true,
                            ORG_ID = _orgId,
                            CREATED_BY = _userId,
                            CREATED_ON = DateTime.UtcNow
                        };
                        var setAbleParentType = types.FirstOrDefault(c => c.ENCOUNTER_CODE?.ToUpperInvariant() == "CONSULTATION");
                        if (setAbleParentType is not null)
                        {
                            type.PARENT_ENCOUNTER_ID = setAbleParentType.PARENT_ENCOUNTER_ID;
                        }

                        setAbleParentType = types.FirstOrDefault(c => c.ENCOUNTER_CODE?.ToUpperInvariant() == "ADMISSION");
                        if (setAbleParentType is not null)
                        {
                            type.PARENT_ENCOUNTER_ID = setAbleParentType.PARENT_ENCOUNTER_ID;
                        }

                        setAbleParentType = types.FirstOrDefault(c => c.ENCOUNTER_CODE?.ToUpperInvariant()  == "BOARDING");
                        if ( setAbleParentType is not null)
                        {
                            type.PARENT_ENCOUNTER_ID = setAbleParentType.PARENT_ENCOUNTER_ID;
                        }

                        setAbleParentType = types.FirstOrDefault(c => c.ENCOUNTER_CODE?.ToUpperInvariant()  == "GROOMING");
                        if (setAbleParentType is not null)
                        {
                            type.PARENT_ENCOUNTER_ID = setAbleParentType.PARENT_ENCOUNTER_ID;
                        }

                        setAbleParentType = types.FirstOrDefault(c => c.ENCOUNTER_CODE?.ToUpperInvariant() == "SPECIAL-1");
                        if (setAbleParentType is not null)
                        {
                            type.PARENT_ENCOUNTER_ID = setAbleParentType.PARENT_ENCOUNTER_ID;
                        }

                        setAbleParentType = types.FirstOrDefault(c => c.ENCOUNTER_CODE?.ToUpperInvariant() == "SURGERY");
                        if (setAbleParentType is not null)
                        {
                            type.PARENT_ENCOUNTER_ID = setAbleParentType.PARENT_ENCOUNTER_ID;
                        }

                        setAbleParentType = types.FirstOrDefault(c => c.ENCOUNTER_CODE?.ToUpperInvariant() == "VACCINE");
                        if (setAbleParentType is not null)
                        {
                            type.PARENT_ENCOUNTER_ID = setAbleParentType.PARENT_ENCOUNTER_ID;
                        }

                        var id = await _iDataAccess.InsertAsync(type, "PMS_ENCOUNTERTYPE");
                        type.ENCOUNTER_ID = Convert.ToInt32(id);
                        types.Add(type);
                    }

                    var department = departments.FirstOrDefault(c => c.UNIT_NAME.ToUpperInvariant() == row.Department.ToUpperInvariant());
                    if (department is null && AppUtility.HasAnyStr(row.Department))
                    {
                        department = new HrUnit()
                        {
                            UNIT_UID = row.Department,
                            CREATED_ON = DateTime.UtcNow,
                            ORG_ID = _orgId,
                            UNIT_NAME = row.Department,
                            CREATED_BY = _userId
                        };
                        var id = await _iDataAccess.InsertAsync(department, "HR_UNIT");
                        department.UNIT_ID = Convert.ToInt32(id);
                        departments.Add(department);
                    }

                    var reasonType = reasonTypes.FirstOrDefault(c => c.ENCOUNTER_ID == type.ENCOUNTER_ID
                                                                     && c.HR_UNIT_ID == department.UNIT_ID
                                                                     && c.REASON_TYPE == row.ReasonType);
                    if (reasonType is null && AppUtility.HasAnyStr(row.ReasonType))
                    {
                        reasonType = new PmrReasonTypeDto()
                        {
                            ENCOUNTER_ID = type.ENCOUNTER_ID,
                            REASON_TYPE = row.ReasonType,
                            CREATED_ON = DateTime.UtcNow,
                            ORG_ID = _orgId,
                            CREATED_BY_ID = _userId,
                            HR_UNIT_ID = department.UNIT_ID,
                        };

                        var id = await _iDataAccess.InsertAsync(reasonType, "PMR_REASON_TYPE");
                    }
                }
            }

            lblStatus.Text = @"Data imported successfully";
            lblStatus.ForeColor = Color.Green;

        }
    }
}
