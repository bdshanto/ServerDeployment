using ServerDeployment.Console.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// PowerShell SDK is required for PowerShell integration
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Windows.Forms;

namespace ServerDeployment.Console.Forms
{
    public partial class MainForm : Form
    {
        private string sitesRoot = @"D:\Workspace\Office\ARAK\Resources\Websites";
        public MainForm()
        {
            InitializeComponent();
            InitializeDataGrid();
            LoadSitesFromIIS();
        }

        private void InitializeDataGrid()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.MultiSelect = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.Columns.Clear();

            var checkBoxCol = new DataGridViewCheckBoxColumn()
            {
                HeaderText = "Select",
                Width = 50,
                Name = "chkSelect"
            };
            dataGridView1.Columns.Add(checkBoxCol);

            var nameCol = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Site",
                Name = "Site",
                ReadOnly = true,
                Width = 100
            };
            dataGridView1.Columns.Add(nameCol);
            var siteFolder = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Site Folder",
                Name = "SiteFolder",
                ReadOnly = true,
                Width = 600
            };
            dataGridView1.Columns.Add(siteFolder);

            var statusCol = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Status",
                Name = "Status",
                ReadOnly = true,
                Width = 100
            };
            dataGridView1.Columns.Add(statusCol);
        }

        private void LoadSitesFromIIS()
        {
            dataGridView1.Rows.Clear();

            var sites = GetIISSites();

            foreach (var site in sites)
            {
                dataGridView1.Rows.Add(false, site.Name, site.PhysicalPath, site.State);
            }
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
            List<IISSiteInfo> selected = new();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chkSelect"].Value) == true)
                {
                    selected.Add(new IISSiteInfo
                    {
                        Name = row.Cells["Site"].Value.ToString(),
                        PhysicalPath = row.Cells["SiteFolder"].Value.ToString(),
                        State = row.Cells["Status"].Value.ToString()
                    });
                }
            }
            return selected;
        }

        private void BackupSelectedSites()
        {
            var selectedSites = GetSelectedSites(); // your method returning selected IISSiteInfo list 
            {
                MessageBox.Show("Please select at least one site to backup.");
                return;
            }

            using var fbd = new FolderBrowserDialog { Description = "Select Backup Destination Folder" };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            foreach (var site in selectedSites)
            {
                try
                {
                    string sourceDir = site.PhysicalPath;
                    string backupDir = Path.Combine(fbd.SelectedPath, $"{site.Name}_backup_{DateTime.Now:yyyyMMddHHmmss}");
                    CopyDirectory(sourceDir, backupDir);
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
                file.CopyTo(Path.Combine(destDir, file.Name), true);

            foreach (var subDir in dir.GetDirectories())
                CopyDirectory(subDir.FullName, Path.Combine(destDir, subDir.Name));
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
                var folder = Path.Combine(sitesRoot, site.PhysicalPath);
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

        private void DeleteAllFiles(string folder)
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
                    // Ignore files that cannot be deleted
                }
            }

            foreach (var dir in Directory.GetDirectories(folder))
            {
                DeleteAllFiles(dir);
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

        // Copy appsettings.json from each selected site to a backup folder
        private void CopyAppSettings()
        {
            var selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one site.");
                return;
            }

            using var fbd = new FolderBrowserDialog()
            {
                Description = "Select destination folder for appsettings.json backup"
            };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            foreach (var site in selected)
            {
                var sourceFile = Path.Combine(sitesRoot, site.PhysicalPath, "appsettings.json");
                if (!File.Exists(sourceFile)) continue;

                var destFile = Path.Combine(fbd.SelectedPath, $"{site}_appsettings_{DateTime.Now:yyyyMMddHHmmss}.json");
                try
                {
                    File.Copy(sourceFile, destFile, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error copying appsettings.json for {site}: {ex.Message}");
                }
            }

            MessageBox.Show("Appsettings.json files copied.");
        }

        // Ping the site folder as hostname or IP (simplified)
        private async Task PingSiteAsync(string siteFolderName)
        {
            // For demonstration, treat siteFolderName as hostname (user can adjust as needed)
            var ping = new Ping();
            try
            {
                var reply = await ping.SendPingAsync(siteFolderName, 2000);
                string status = reply.Status == IPStatus.Success ? "Online" : "Offline";
                UpdateSiteStatus(siteFolderName, status);
            }
            catch
            {
                UpdateSiteStatus(siteFolderName, "Offline");
            }
        }

        private void UpdateSiteStatus(string siteFolderName, string status)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((string)row.Cells["SiteFolder"].Value == siteFolderName)
                {
                    row.Cells["Status"].Value = status;
                    break;
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
                StopSite(site.PhysicalPath);
            }
            MessageBox.Show("Stop commands sent.");
        }

        private void btnStartIIS_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedSites();
            foreach (var site in selected)
            {
                StartSite(site.PhysicalPath);
            }
            MessageBox.Show("Start commands sent.");
        }

        private void btnDeleteFiles_Click(object sender, EventArgs e)
        {
            DeleteFilesInSites();
        }

        private void btnCopyAppSettings_Click(object sender, EventArgs e)
        {
            CopyAppSettings();
        }

    }
}
