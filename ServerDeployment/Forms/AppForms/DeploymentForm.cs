using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.Json;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Microsoft.Web.Administration;
using ServerDeployment.Helpers;

namespace ServerDeployment.Forms.AppForms
{
    public partial class DeploymentForm : Form
    {
        private DataTable _sitesDataTable;

        private string _backendPath = "";
        private string _frontendPath = "";
        private string _reportPath = "";
        private string _backupPath = "";

        private readonly Dictionary<string, string> _siteBackupDirectory = new();

        public delegate void ProgressUpdateHandler(string message);
        // public event ProgressUpdateHandler? ProgressUpdated;

        public event EventHandler<ProgressEventArgs>? ProgressUpdated;


        public DeploymentForm()
        {
            InitializeComponent(); 
            ProgressUpdated += MainForm_ProgressUpdated;

            ButtonsSwitch(false);
            InitializeUltraGrid();
            LoadSitesFromIIS();

            lblMsg.Text = @"Please set both Site Root and Backup Path before publishing.";
            lblMsg.ForeColor = Color.Red;

        }

        private void InitializeUltraGrid()
        {
            

            // Create schema once
            _sitesDataTable = CreateSitesDataTable();

            // Bind empty schema
            ultraGrid.DataSource = _sitesDataTable;
            ultraGrid.UseAppStyling = true;
            ultraGrid.DisplayLayout.BorderStyle = UIElementBorderStyle.Solid;

            // Customize columns once after binding
            CustomizeUltraGridColumns();


        }


        private void LoadSitesFromIIS()
        {
            var selectedSites = GetSelectedSites();

            var sites = GetIISSites();
            var dt = CreateSitesDataTable();

            foreach (var site in sites)
            {
                bool isSelected = false;
                var selected = selectedSites.FirstOrDefault(c => c.Name.Equals(site.Name));
                if (selected != null)
                {
                    isSelected = selected.Select;
                }


                dt.Rows.Add(isSelected, site.Name, site.PhysicalPath, site.State);
            }
            // Bind the DataTable to ultraGrid
            ultraGrid.DataSource = dt;

            // Customize columns after binding
            var band = ultraGrid.DisplayLayout.Bands[0];

            band.Columns["Select"].Header.Caption = "Select";
            band.Columns["Select"].Width = 80;
            band.Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            band.Columns["Select"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit; 

            band.Columns["Name"].Header.Caption = "Site";
            band.Columns["Name"].Width = 150;
            band.Columns["Name"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            band.Columns["PhysicalPath"].Header.Caption = "Site Folder";
            band.Columns["PhysicalPath"].Width = 400;
            band.Columns["PhysicalPath"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            band.Columns["State"].Header.Caption = "Status";
            band.Columns["State"].Width = 100;
            band.Columns["State"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

          

        }
        private List<IISSiteInfo> GetIISSites()
        {
            var sites = new List<IISSiteInfo>();

            // PowerShell script to get Name, PhysicalPath, and State of IIS sites as JSON
            string script = @"Import-Module WebAdministration;  Get-Website | Select-Object Name, PhysicalPath, State | ConvertTo-Json ";

            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -Command \"{script}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            try
            {
                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(output))
                    {

                        var option = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            WriteIndented = true
                        };
                        if (output.TrimStart().StartsWith("["))
                        {
                            sites = JsonSerializer.Deserialize<List<IISSiteInfo>>(output, option);
                        }
                        else
                        {
                            var singleSite = JsonSerializer.Deserialize<IISSiteInfo>(output, option);
                            if (singleSite != null) sites.Add(singleSite);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Failed to get IIS sites: " + ex.Message);
            }

            return sites;
        }

        private List<IISSiteInfo> GetSelectedSites()
        {
            List<IISSiteInfo> selected = new List<IISSiteInfo>();

            foreach (UltraGridRow row in ultraGrid.Rows)
            {
                // Check if the checkbox cell is true (checked)
                if (row.Cells["Select"].Value is bool isChecked && isChecked)
                {
                    selected.Add(new IISSiteInfo
                    {
                        Name = row.Cells["Name"].Value?.ToString() ?? string.Empty,
                        PhysicalPath = row.Cells["PhysicalPath"].Value?.ToString() ?? string.Empty,
                        State = row.Cells["State"].Value?.ToString() ?? string.Empty,
                        Select = isChecked
                    });
                }
            }

            return selected;
        }

        private void BackupSelectedSites()
        {
            ClearLables();

            var selectedSites = GetSelectedSites();
            if (selectedSites.Count <= 0)
            {
                lblMsg.Text = "Please select at least one site to backup.";
                lblMsg.ForeColor = Color.Red;
                return;
            }

            int totalSites = selectedSites.Count;
            int currentSite = 0;

            foreach (var site in selectedSites)
            {
                currentSite++;
                try
                {
                    OnProgressUpdated($"Backing up site '{site.Name}' ({currentSite} of {totalSites})...", (currentSite * 100) / totalSites);

                    string sourceDir = site.PhysicalPath;
                    string backupDir = Path.Combine(_backupPath, $"{site.Name}_backup_{DateTime.Now:yyyyMMddHHmmss}");

                    CopyDirectory(sourceDir, backupDir);

                    if (_siteBackupDirectory.ContainsKey(site.Name))
                    {
                        _siteBackupDirectory.Remove(site.Name);
                    }

                    _siteBackupDirectory.Add(site.Name, backupDir);
                }
                catch (Exception ex)
                {
                    OnProgressUpdated($"Failed to backup site '{site.Name}': {ex.Message}");
                }
            }
            OnProgressUpdated("Backup completed.", 100);
        }
        private void CopySiteContent(string sourceRoot, string destinationSiteFolder)
        {
            // Copy frontend files/folders (everything except PetMatrixBackendAPI and ReportsViewer) directly to site root
            var frontendSource = new DirectoryInfo(sourceRoot);

            foreach (var file in frontendSource.GetFiles())
            {
                string destFile = Path.Combine(destinationSiteFolder, file.Name);
                file.CopyTo(destFile, true);
            }

            foreach (var dir in frontendSource.GetDirectories())
            {
                // Skip backend and ReportsViewer folders here, they get copied separately
                if (dir.Name.Equals("PetMatrixBackendAPI", StringComparison.OrdinalIgnoreCase) ||
                    dir.Name.Equals("ReportsViewer", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                CopyDirectory(dir.FullName, Path.Combine(destinationSiteFolder, dir.Name));
            }

            // Copy backend files to PetMatrixBackendAPI folder inside site folder
            string backendSourceFolder = Path.Combine(sourceRoot, "PetMatrixBackendAPI");
            if (Directory.Exists(backendSourceFolder))
            {
                string backendDestFolder = Path.Combine(destinationSiteFolder, "PetMatrixBackendAPI");
                Directory.CreateDirectory(backendDestFolder);
                CopyDirectory(backendSourceFolder, backendDestFolder);
            }

            // Copy ReportsViewer folder
            string reportsViewerSourceFolder = Path.Combine(sourceRoot, "ReportsViewer");
            if (Directory.Exists(reportsViewerSourceFolder))
            {
                string reportsViewerDestFolder = Path.Combine(destinationSiteFolder, "ReportsViewer");
                CopyDirectory(reportsViewerSourceFolder, reportsViewerDestFolder);
            }
        }


        private void CopyDirectory(string sourceDir, string destDir)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory does not exist: {sourceDir}");
            Directory.CreateDirectory(destDir);

            foreach (var file in dir.GetFiles())
            {
                file.CopyTo(Path.Combine(destDir, file.Name), true);
            }

            foreach (var subDir in dir.GetDirectories())
            {
                CopyDirectory(subDir.FullName, Path.Combine(destDir, subDir.Name));
            }
        }


        private void StopSite(string siteFolderName)
        {
            // Find IIS site name from folder name - simplified as folder name is site name
            var siteName = siteFolderName;

            RunAppCmd($"stop site \"{siteName}\"");
        }

        // Start IIS site by folder name using appcmd
        private void StartSite(string siteFolderName)
        {
            var siteName = siteFolderName;
            RunAppCmd($"start site \"{siteName}\"");
        }

        // Run appcmd with arguments
        private void RunAppCmd(string args)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"System32\inetsrv\appcmd.exe"),
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas" // run as admin
                };
                using var proc = Process.Start(psi);
                proc.WaitForExit();

                string output = proc.StandardOutput.ReadToEnd();
                string err = proc.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(err))
                {
                    MessageBox.Show("Error running appcmd: " + err);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to run appcmd: " + ex.Message);
            }
        }

        // Delete all files in selected site folders (with confirmation)
        private void DeleteFilesInSites()
        {
            List<IISSiteInfo> selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                lblMsg.Text = "Please select at least one site.";
                lblMsg.ForeColor = Color.Red;
                return;
            }
            var confirm = MessageBox.Show("Are you sure you want to delete all files in selected site folders? This cannot be undone!", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            foreach (var site in selected)
            {
                var folder = Path.Combine(_backendPath, site.PhysicalPath);
                try
                {
                    DeleteAllFiles(folder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting files in {site}: {ex.Message}");
                }
            }
            MessageBox.Show("Files deleted.");
        }

        private void DeleteAllFiles(string folder, bool isRoot = true)
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch
                {

                }
            }

            foreach (var dir in Directory.GetDirectories(folder))
            {
                // Skip deleting "Documents" folder if at root
                if (isRoot && string.Equals(new DirectoryInfo(dir).Name, "Documents", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                DeleteAllFiles(dir, false);

                try
                {
                    Directory.Delete(dir, true);
                }
                catch
                {
                    // Ignore folders that cannot be deleted
                }
            }
        }

        // Ping the site folder as hostname or IP (simplified)
        private async Task PingSiteAsync(string siteName)
        {
            string hostHeader = "";
            string ip = "";
            int port = 0;

            using (ServerManager iisManager = new ServerManager())
            {
                var site = iisManager.Sites[siteName];
                if (site != null)
                {
                    foreach (var binding in site.Bindings)
                    {
                        ip = binding.EndPoint?.Address.ToString() ?? "";
                        port = binding.EndPoint?.Port ?? 0;
                        hostHeader = binding.Host;

                        // Use first valid binding
                        break;
                    }
                }
            }

            string pingTarget = "127.0.0.1";

            // Determine ping target
            if (!string.IsNullOrEmpty(hostHeader) && hostHeader != "*" && hostHeader != "0.0.0.0")
            {
                pingTarget = hostHeader;
            }
            else if (!string.IsNullOrEmpty(ip) && ip != "0.0.0.0" && ip != "::")
            {
                pingTarget = ip;
            }

            if (pingTarget != null)
            {
                var ping = new Ping();
                try
                {
                    var reply = await ping.SendPingAsync($@"{pingTarget}:{port}", 2000);
                    string status = reply.Status == IPStatus.Success ? "Online" : "Offline";
                    UpdateSiteStatus(siteName, status);
                }
                catch (Exception ex)
                {
                    UpdateSiteStatus(siteName, "Offline");
                }
            }
            else
            {
                // Cannot ping unknown or invalid address
                UpdateSiteStatus(siteName, "Unknown");
            }

        }

        private void UpdateSiteStatus(string siteFolderName, string status)
        {

            foreach (UltraGridRow row in ultraGrid.Rows)
            {
                if (row.Cells["PhysicalPath"].Value?.ToString() == siteFolderName)
                {
                    row.Cells["State"].Value = status;
                    break;  // Exit after updating first match
                }
            }
        }

        // UI button click events

        private void btnBackup_Click(object sender, EventArgs e)
        {
            BackupSelectedSites();
        }

        private void btnStopIIS_Click(object sender, EventArgs e)
        {
            StopIIS();
        }

        private void StopIIS()
        {
            var selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                lblMsg.Text = "Please select at least one site.";
                lblMsg.ForeColor = Color.Red;
                return;
            }
            foreach (var site in selected)
            {
                StopSite(site.Name);
            }
            LoadSitesFromIIS();
        }

        private void btnStartIIS_Click(object sender, EventArgs e)
        {
            StartIIS();
        }

        private void StartIIS()
        {
            var selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                lblMsg.Text = "Please select at least one site.";
                lblMsg.ForeColor = Color.Red;
                return;
            }
            foreach (var site in selected)
            {
                StartSite(site.Name);
            }
            LoadSitesFromIIS();
        }

        private void btnDeleteFiles_Click(object sender, EventArgs e)
        {
            DeleteFilesInSites();
        }

        private void btnCopyAppSettings_Click(object sender, EventArgs e)
        {

            CopyAppSettings();
        }

        private void CopyAppSettings()
        {
            var sites = GetIISSites();
            if (sites.Count <= 0)
            {
                MessageBox.Show("No sites found.");
                return;
            }

            try
            {
                foreach (var site in sites)
                {
                    string sourceRoot = _siteBackupDirectory[site.Name];
                    var destRoot = site.PhysicalPath;

                    // Copy web.config from source root
                    string sourceWebConfig = Path.Combine(sourceRoot, "web.config");
                    if (File.Exists(sourceWebConfig))
                    {
                        string destWebConfig = Path.Combine(destRoot, "web.config");
                        File.Copy(sourceWebConfig, destWebConfig, overwrite: true);
                    }

                    // Copy appsettings.json from PetMatrixBackendAPI folder
                    string sourceAppSettings = Path.Combine(sourceRoot, "PetMatrixBackendAPI", "appsettings.json");
                    if (File.Exists(sourceAppSettings))
                    {
                        string destApiFolder = Path.Combine(destRoot, "PetMatrixBackendAPI");
                        Directory.CreateDirectory(destApiFolder);
                        string destAppSettings = Path.Combine(destApiFolder, "appsettings.json");
                        File.Copy(sourceAppSettings, destAppSettings, overwrite: true);
                    }

                    // Copy ReportsViewer folder
                    string sourceReportsViewer = Path.Combine(sourceRoot, "ReportsViewer");
                    string destReportsViewer = Path.Combine(destRoot, "ReportsViewer");
                    if (Directory.Exists(sourceReportsViewer))
                        CopyDirectory(sourceReportsViewer, destReportsViewer);
                }

                lblMsg.Text = "Copy completed successfully.";
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error during copy: {ex.Message}";
            }

        }
        private void btnReloadSites_Click(object sender, EventArgs e)
        {
            LoadSitesFromIIS();
        }
        private async void btnPingSite_Click(object sender, EventArgs e)
        {
            await PingSite();
        }

        private async Task PingSite()
        {
            var selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                MessageBox.Show("Select site(s) to ping.");
                return;
            }
            foreach (var site in selected)
            {
                await PingSiteAsync(site.Name);
            }
        }



        private void btnSetSiteRoot_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = "Select Site Root Directory",
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                _backendPath = folderDialog.SelectedPath;
                txtBackend.Text = _backendPath;

                ButtonsSwitch(true);

            }
        }

        private void btnCopyContent_Click(object sender, EventArgs e)
        {
            CopyContent();
        }

        private void CopyContent()
        {
            var selectedSites = GetSelectedSites();
            if (selectedSites.Count == 0)
            {
                lblMsg.Text = "Please set both Site Root and Backup Path before publishing.";
                lblMsg.BackColor = Color.Red;
                return;
            }



            try
            {
                var sourceFolder = _backendPath;

                foreach (var site in selectedSites)
                {

                    CopySiteContent(sourceFolder, site.PhysicalPath);

                }
                MessageBox.Show("Content copied successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error copying content: " + ex.Message);
            }
        }
        private void btnBackupPath_Click(object sender, EventArgs e)
        {
            BackupPath();
        }

        private void BackupPath()
        {
            using var fbd = new FolderBrowserDialog { Description = "Select Backup Destination Folder" };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            _backupPath = fbd.SelectedPath;
            txtBackup.Text = fbd.SelectedPath;
            ButtonsSwitch(true);
        }

        private void ButtonsSwitch(bool value)
        {

            if (!HasNoStr(txtBackup.Text) && txtBackup.Text.Length > 0)
            {
                btnBackupPath.ForeColor = Color.Green;
            }
            else
            {
                btnBackupPath.ForeColor = Color.Red;
            }

            if (!HasNoStr(txtBackend.Text) && txtBackend.Text.Length > 0)
            {
                btnBackend.ForeColor = Color.Green;
            }
            else
            {
                btnBackend.ForeColor = Color.Red;
            }

            if (!HasNoStr(txtFrontend.Text) && txtFrontend.Text.Length > 0)
            {
                btnFrontend.ForeColor = Color.Green;
            }
            else
            {
                btnFrontend.ForeColor = Color.Red;
            }

            if (!HasNoStr(txtReport.Text) && txtReport.Text.Length > 0)
            {
                btnReport.ForeColor = Color.Green;
            }
            else
            {
                btnReport.ForeColor = Color.Red;
            }



            btnReloadSites.Enabled = value;
            btnBackup.Enabled = value;
            btnStopIIS.Enabled = value;
            btnStartIIS.Enabled = value;
            btnDeleteFiles.Enabled = value;
            btnCopyAppSettings.Enabled = value;
            btnPingSite.Enabled = value;
            btnCopyContent.Enabled = value;
        }



        private bool HasNoStr(string str)
        {
            return string.IsNullOrWhiteSpace(str) || string.IsNullOrEmpty(str);
        }

        private DataTable CreateSitesDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Select", typeof(bool));      // checkbox column
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("PhysicalPath", typeof(string));
            dt.Columns.Add("State", typeof(string));
            return dt;
        }

        private void CustomizeUltraGridColumns()
        {
            var band = ultraGrid.DisplayLayout.Bands[0];

            band.Columns["Select"].Header.Caption = "Select";
            band.Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            band.Columns["Select"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;

            band.Columns["Name"].Header.Caption = "Name";
            band.Columns["Name"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            band.Columns["PhysicalPath"].Header.Caption = "Site Folder";
            band.Columns["PhysicalPath"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            band.Columns["State"].Header.Caption = "Status";
            band.Columns["State"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            // Set Header Font (size, style, color)
            band.Columns["Name"].Header.Appearance.FontData.SizeInPoints = 13; // Font size
            band.Columns["Name"].Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True; // Bold
            band.Columns["Name"].Header.Appearance.ForeColor = Color.Black; // Text color (black to match white theme)
            band.Columns["Name"].Header.Appearance.BackColor = Color.LightGray; // Light gray background for header

            band.Columns["State"].Header.Appearance.FontData.SizeInPoints = 13; // Font size
            band.Columns["State"].Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True; // Bold
            band.Columns["State"].Header.Appearance.ForeColor = Color.Black; // Text color
            band.Columns["State"].Header.Appearance.BackColor = Color.LightGray; // Light gray background for header

            // Optionally, align header text
            band.Columns["Name"].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center; // Center alignment
            band.Columns["State"].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center; // Center alignment

            // General header customizations for all columns
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.SizeInPoints = 13; // Set header font size for all columns
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True; // Set all headers to bold
            ultraGrid.DisplayLayout.Override.HeaderAppearance.ForeColor = Color.Black; // Set header text color
            ultraGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.LightGray; // Set header background color for all columns



            // Selection settings
            ultraGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            ultraGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;


            // Set the row height to a larger value (default ~20)
            ultraGrid.DisplayLayout.Override.RowSizing = RowSizing.Fixed;

            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.Name = "Segoe UI";
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.SizeInPoints = 11;
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;

            ultraGrid.DisplayLayout.Override.RowAppearance.FontData.Name = "Segoe UI";
            ultraGrid.DisplayLayout.Override.RowAppearance.FontData.SizeInPoints = 10;
            ultraGrid.DisplayLayout.Override.RowAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.False;



        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            ClearLables();

            if (HasNoStr(_backendPath) || HasNoStr(_backupPath))
            {
                lblMsg.Text = "Please set both Site Root and Backup Path before publishing.";
                lblMsg.BackColor = Color.Red;
                return;
            }

            // 1. backup selected sites
            // BackupSelectedSites();

        }



        private void ClearLables()
        {
            lblMsg.Text = string.Empty;
            lblMsg.BackColor = SystemColors.Control;
        }


        private void OnProgressUpdated(string message, int? percent = null)
        {
            if (ProgressUpdated != null)
            {
                ProgressUpdated(this, new ProgressEventArgs(message, percent));
            }
        }

        private void MainForm_ProgressUpdated(object? sender, ProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgressUI(e)));
            }
            else
            {
                UpdateProgressUI(e);
            }
        }

        private void UpdateProgressUI(ProgressEventArgs e)
        {
            lblMsg.Text = e.Message;

            // Optional: if you have a progress bar, update it here:
            if (e.Percent.HasValue)
            {
                progressBar1.Value = Math.Min(Math.Max(e.Percent.Value, 0), 100);
            }
        }

        private void btnFrontend_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = "Select Site Root Directory"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                _frontendPath = folderDialog.SelectedPath;
                txtFrontend.Text = _frontendPath;
                txtFrontend.Text = _frontendPath;

                ButtonsSwitch(true);

            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = "Select Site Root Directory"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                _reportPath = folderDialog.SelectedPath;
                txtReport.Text = _reportPath;
                _reportPath = _reportPath ;

                ButtonsSwitch(true);

            }
        }
    }
}
