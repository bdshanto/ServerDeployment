using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Microsoft.Web.Administration;
using ServerDeployment.Console.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace ServerDeployment.Console.Forms
{
    public partial class MainForm : Form
    {
        private DataTable sitesDataTable;

        private string siteRootFolder = ""; // field to hold site root path
        private string siteBackupFolder = ""; // field to hold backup path

        private Dictionary<string, string> siteBackupDirectory = new();
        public MainForm()
        {
            InitializeComponent(); 

            InitializeUltraGrid();
            LoadSitesFromIIS();
            DispableAllButtons();
        }

        private void InitializeUltraGrid()
        {
            // Create schema once
            sitesDataTable = CreateSitesDataTable();

            // Bind empty schema
            ultraGrid.DataSource = sitesDataTable;

            // Customize columns once after binding
            CustomizeUltraGridColumns();


        }


        private void LoadSitesFromIIS()
        {


            var sites = GetIISSites();
            var dt = CreateSitesDataTable();

            foreach (var site in sites)
            {
                dt.Rows.Add(false, site.Name, site.PhysicalPath, site.State);
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

            // Selection settings
            ultraGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            ultraGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;

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
                        State = row.Cells["State"].Value?.ToString() ?? string.Empty
                    });
                }
            }

            return selected;
        }

        private void BackupSelectedSites()
        {
            var selectedSites = GetSelectedSites();
            if (selectedSites.Count <= 0)
            {
                MessageBox.Show("Please select at least one site to backup.");
                return;
            }

            foreach (var site in selectedSites)
            {
                try
                {
                    string sourceDir = site.PhysicalPath;
                    string folderName = @$"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}";
                    string backupDir = Path.Combine(siteBackupFolder, folderName,$"{site.Name}_backup_{DateTime.Now:yyyyMMddHHmmss}");
                    CopyDirectory(sourceDir, backupDir);

                    if (siteBackupDirectory.ContainsKey(site.Name))
                    {
                        siteBackupDirectory.Remove(site.Name);
                    }

                    siteBackupDirectory.Add(site.Name, backupDir);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to backup site '{site.Name}': {ex.Message}");
                }
            }
            MessageBox.Show("Backup completed.");
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
                MessageBox.Show("Please select at least one site.");
                return;
            }
            var confirm = MessageBox.Show("Are you sure you want to delete all files in selected site folders? This cannot be undone!", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            foreach (var site in selected)
            {
                var folder = Path.Combine(siteRootFolder, site.PhysicalPath);
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
            var selected = GetSelectedSites();
            foreach (var site in selected)
            {
                StopSite(site.Name);
            }
            LoadSitesFromIIS();
            // MessageBox.Show("Stop commands sent.");
        }

        private void btnStartIIS_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedSites();
            foreach (var site in selected)
            {
                StartSite(site.Name);
            }
            LoadSitesFromIIS();
            // MessageBox.Show("Start commands sent.");
        }

        private void btnDeleteFiles_Click(object sender, EventArgs e)
        {
            DeleteFilesInSites();
        }

        private void btnCopyAppSettings_Click(object sender, EventArgs e)
        {
            var sites = GetIISSites();
            if (sites.Count <= 0)
            {
                MessageBox.Show("No sites found.");
                return;
            }
            /*
                        using var sourceDialog = new FolderBrowserDialog
                        {
                            Description = "Select Source Directory"
                        };
                        if (sourceDialog.ShowDialog() != DialogResult.OK)
                            return;*/

            // string sourceRoot = sourceDialog.SelectedPath;



            try
            {
                foreach (var site in sites)
                {
                    string sourceRoot = siteBackupDirectory[site.Name];
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

                MessageBox.Show("Copy completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during copy: {ex.Message}");
            }

        }

        private void btnReloadSites_Click(object sender, EventArgs e)
        {
            LoadSitesFromIIS();
        }
        private async void btnPingSite_Click(object sender, EventArgs e)
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
            MessageBox.Show("Ping completed.");
        }



        private void btnSetSiteRoot_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = "Select Site Root Directory"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                siteRootFolder = folderDialog.SelectedPath;
                txtSiteRoot.Text = siteRootFolder;

                ButtonsSwitch(true);

            }
        }

        private void btnCopyContent_Click(object sender, EventArgs e)
        {
            var selectedSites = GetSelectedSites();
            if (selectedSites.Count == 0)
            {
                MessageBox.Show("Please select at least one site to copy content.");
                return;
            }

            var sourceFolder = siteRootFolder;


            try
            {
                foreach (var site in selectedSites)
                {

                    CopyDirectory(sourceFolder, site.PhysicalPath);
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
            using var fbd = new FolderBrowserDialog { Description = "Select Backup Destination Folder" };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            siteBackupFolder = fbd.SelectedPath;
            txtBackupPath.Text = fbd.SelectedPath;
            ButtonsSwitch(true);

        }

        private void ButtonsSwitch(bool value)
        {
            if (HasNoStr(siteRootFolder) || HasNoStr(siteBackupFolder)) return;

            btnReloadSites.Enabled = value;
            btnBackup.Enabled = value;
            btnStopIIS.Enabled = value;
            btnStartIIS.Enabled = value;
            btnDeleteFiles.Enabled = value;
            btnCopyAppSettings.Enabled = value;
            btnPingSite.Enabled = value;
            btnCopyContent.Enabled = value;

        }

        private void DispableAllButtons()
        {
            btnReloadSites.Enabled = false;
            btnBackup.Enabled = false;
            btnStopIIS.Enabled = false;
            btnStartIIS.Enabled = false;
            btnDeleteFiles.Enabled = false;
            btnCopyAppSettings.Enabled = false;
            btnPingSite.Enabled = false;
            btnCopyContent.Enabled = false;

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
            SetColumnWidthsByPercent(); 

            ultraGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            ultraGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
        }

        private void SetColumnWidthsByPercent()
        {
            ultraGrid.Width = 730;
            if (ultraGrid.DisplayLayout.Bands.Count == 0)
                return; // No bands yet

            int totalWidth = ultraGrid.ClientSize.Width;

            var band = ultraGrid.DisplayLayout.Bands[0];

            // Calculate widths
            int selectWidth = (int)(totalWidth * 0.10);
            int nameWidth = (int)(totalWidth * 0.25);
            int physicalPathWidth = (int)(totalWidth * 0.45);

            // Assign widths
            band.Columns["Select"].Width = selectWidth;
            band.Columns["Name"].Width = nameWidth;
            band.Columns["PhysicalPath"].Width = physicalPathWidth;

            // Assign remaining width to last column to avoid rounding issues
            int assignedWidth = selectWidth + nameWidth + physicalPathWidth;
            int remainingWidth = totalWidth - assignedWidth;

            if (remainingWidth < 0) remainingWidth = 0; // Safety check

            band.Columns["State"].Width = remainingWidth;
        }

     

    }
}
