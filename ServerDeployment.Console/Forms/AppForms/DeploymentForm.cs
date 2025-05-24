using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.RegularExpressions;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Microsoft.Web.Administration;
using ServerDeployment.Applications.Helpers;
using ServerDeployment.Console.Helpers;
using ServerDeployment.Domains.Utility;
using Application = System.Windows.Forms.Application;

namespace ServerDeployment.Console.Forms.AppForms
{
    public partial class DeploymentForm : Form
    {
        private DataTable _sitesDataTable;

        private string _backupPath = @"D:\Workspace\Office\ARAK\Resources\Publish";
        private string _backendPath = @"D:\Workspace\Office\ARAK\Resources\Publish\ARAK-Backend";
        private string _frontendPath = @"D:\Workspace\Office\ARAK\Resources\Publish\ARAK-Frontend";
        private string _reportPath = @"";

        private readonly Dictionary<string, string> _siteBackupDirectory = new();

        public delegate void StatusUpdateHandler(string message, Color color);
        public event StatusUpdateHandler? StatusUpdated;



        private readonly List<string> _expectedReportViewerFilesAndFolders = new List<string>
        {
            "bin",
            "Content",
            "fonts",
            "Scripts",
            "SqlServerTypes",
            "About.aspx",
            "Bundle.config",
            "Contact.aspx",
            "Default.aspx",
            "favicon.ico",
            "Global.asax",
            "ReportViewer.aspx",
            "Site.Master",
            "Site.Mobile.Master",
            "ViewSwitcher.ascx"
        };

        private readonly List<object> _expectedFrontendFilesAndFolders = new List<object>
        {
            "index.html",
            "assets", // folder
            new Regex(@"^main\.[a-z0-9]+\.bundle\.js$", RegexOptions.IgnoreCase),
            new Regex(@"^main-[a-z0-9]+\.css$", RegexOptions.IgnoreCase),
            new Regex(@"^polyfills\.[a-z0-9]+\.bundle\.js$", RegexOptions.IgnoreCase),
            new Regex(@"^vendors~main\.[a-z0-9]+\.chunk\.js$", RegexOptions.IgnoreCase),
            new Regex(@"^vendors~main-[a-z0-9]+\.css$", RegexOptions.IgnoreCase),
            new Regex(@"^vendors~polyfills\.[a-z0-9]+\.chunk\.js$", RegexOptions.IgnoreCase)
        };
        private readonly List<object> expectedBackendFiles = new List<object>
        {
            "Documents",
            "runtimes",
            "appsettings.json",
            "efpt.config.json",
            "EPPlus.dll",
            "libman.json",
            "web.config",
            "PetMatrix.API.dll",
            "PetMatrix.API.exe",
            "PetMatrix.API.runtimeconfig.json",
            new Regex(@"^Microsoft\..+\.dll$"),
            new Regex(@"^System\..+\.dll$"),
            new Regex(@"^Newtonsoft\.Json\.dll$"),
            new Regex(@"^PetMatrix\..+\.dll$"),
            new Regex(@"^PetMatrix\..+\.pdb$")
        };

        public event EventHandler<ProgressEventArgs>? ProgressUpdated;


        public DeploymentForm()
        {
            InitializeComponent();
            ProgressUpdated += DeploymentForm_ProgressUpdated;
            StatusUpdated += DeploymentForm_StatusUpdated;


            ButtonsSwitch(true);
            InitializeUltraGrid();
            LoadSitesFromIis();

            StatusUpdated?.Invoke("Please set both Site Root and Backup Path before publishing.", Color.Red);


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


        private void LoadSitesFromIis()
        {

            List<IISSiteInfo> sites = GetIISSites();
            DataTable dt = CreateSitesDataTable();

            List<IISSiteInfo> selectedSites = GetSelectedSites();

            foreach (var site in sites)
            {
                bool isSelected = false;
                var selected = selectedSites.FirstOrDefault(c => c.Name.Equals(site.Name));
                if (selected != null)
                {
                    isSelected = selected.Select;
                }

                var contentSize = string.Empty;

                contentSize = AppUtility.GetDirectorySize(site.PhysicalPath);

                dt.Rows.Add(isSelected, site.Name, site.PhysicalPath, contentSize, site.State);
            }
            // Bind the DataTable to ultraGrid
            ultraGrid.DataSource = dt;

            // Customize columns after binding
            var band = ultraGrid.DisplayLayout.Bands[0];

            band.Columns[nameof(IISSiteInfo.Select)].Header.Caption = nameof(IISSiteInfo.Select);
            band.Columns[nameof(IISSiteInfo.Select)].Width = 80;
            band.Columns[nameof(IISSiteInfo.Select)].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            band.Columns[nameof(IISSiteInfo.Select)].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;

            band.Columns[nameof(IISSiteInfo.Name)].Header.Caption = @"Site";
            band.Columns[nameof(IISSiteInfo.Name)].Width = 150;
            band.Columns[nameof(IISSiteInfo.Name)].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            band.Columns[nameof(IISSiteInfo.PhysicalPath)].Header.Caption = @"Site Folder";
            band.Columns[nameof(IISSiteInfo.PhysicalPath)].Width = 400;
            band.Columns[nameof(IISSiteInfo.PhysicalPath)].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            band.Columns[nameof(IISSiteInfo.State)].Header.Caption = @"Status";
            band.Columns[nameof(IISSiteInfo.State)].Width = 100;
            band.Columns[nameof(IISSiteInfo.State)].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;



        }
        private List<IISSiteInfo> GetIISSites()
        {
            var sites = new List<IISSiteInfo>();

            // PowerShell script to get Name, PhysicalPath, and State of IIS sites as JSON
            //            string script = @"Import-Module WebAdministration;  Get-Website | Select-Object Name, PhysicalPath, State | ConvertTo-Json @";
            string script = @"Import-Module WebAdministration; Get-Website | Select-Object Name, PhysicalPath, State | ConvertTo-Json -Depth 3";


            var psi = new ProcessStartInfo
            {
                FileName = @"powershell.exe",
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
                MessageBox.Show("Failed to get IIS sites: @" + ex.Message);
            }

            return sites;
        }

        private List<IISSiteInfo> GetSelectedSites()
        {
            List<IISSiteInfo> selected = new List<IISSiteInfo>();

            foreach (UltraGridRow row in ultraGrid.Rows)
            {
                // Check if the checkbox cell is true (checked)
                if (row.Cells[nameof(IISSiteInfo.Select)].Value is bool isChecked && isChecked)
                {
                    var site = new IISSiteInfo
                    {
                        Name = row.Cells[nameof(IISSiteInfo.Name)].Value?.ToString() ?? string.Empty,
                        PhysicalPath = row.Cells[nameof(IISSiteInfo.PhysicalPath)].Value?.ToString() ?? string.Empty,
                        State = row.Cells[nameof(IISSiteInfo.State)].Value?.ToString() ?? string.Empty,
                        Select = isChecked
                    };
                    site.ContentSize = AppUtility.GetDirectorySize(site.PhysicalPath);

                    selected.Add(site);
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
                StatusUpdated?.Invoke("Please select at least one site to backup.", Color.Red);
                return;
            }

            int totalSites = selectedSites.Count;
            int currentSite = 0;

            foreach (var site in selectedSites)
            {
                currentSite++;
                try
                {
                    OnProgressUpdated($"Backing up site '{site.Name}' ({currentSite} of {totalSites})...", (currentSite * 100) / totalSites, ProgressType.Backup);

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
            OnProgressUpdated("Backup completed.", 100, ProgressType.Backup);
        }




        private void CopyDirectory(string sourceDir, string destDir)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
            {
                SLogger.WriteLog($"{nameof(CopyDirectory)}: Source directory does not exist: {sourceDir}");
                StatusUpdated?.Invoke($"Source directory does not exist: {sourceDir}", Color.Red);
                throw new DirectoryNotFoundException($"Source directory does not exist: {sourceDir}");
            }
            Directory.CreateDirectory(destDir);

            foreach (var file in dir.GetFiles())
            {
                string destFile = Path.Combine(destDir, file.Name);
                file.CopyTo(destFile, true);
                StatusUpdated?.Invoke($"Copied file: {file.Name}", Color.Black);
                Application.DoEvents(); // To keep UI responsive and update status label
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
                    Verb = @"runas" // run as admin
                };
                using var proc = Process.Start(psi);
                proc.WaitForExit();

                string output = proc.StandardOutput.ReadToEnd();
                string err = proc.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(err))
                {


                    StatusUpdated?.Invoke("Error running appcmd: @" + err, Color.Black);
                }
            }
            catch (Exception ex)
            {
                StatusUpdated?.Invoke("Error running appcmd: @" + ex.Message, Color.Red);
                SLogger.WriteLog(ex);
            }
        }

        // Delete all files in selected site folders (with confirmation)
        private void DeleteFilesInSites()
        {
            List<IISSiteInfo> selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                StatusUpdated?.Invoke("Please select at least one site.", Color.Red);
                return;
            }
            var confirm = MessageBox.Show(@"Are you sure you want to delete all files in selected site folders? This cannot be undone!", @"Confirm Delete", MessageBoxButtons.YesNo);
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
                    StatusUpdated?.Invoke($@"Error deleting files in {site}: {ex.Message}", Color.Red);

                    SLogger.WriteLog(ex);
                }
            }
            StatusUpdated?.Invoke(@"Files deleted.", Color.Red);
        }

        private void DeleteAllFiles(string folder, bool isRoot = true)
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);

                    File.Delete(file);
                    StatusUpdated?.Invoke($"Deleted file: {Path.GetFileName(file)}", Color.Black);
                    System.Windows.Forms.Application.DoEvents(); // To refresh UI immediately
                }
                catch (Exception ex)
                {
                    if (TryDeleteFile(file))
                    {
                        StatusUpdated?.Invoke($"Deleted file forcefully: {Path.GetFileName(file)}", Color.Black);
                    }
                    else
                    {
                        StatusUpdated?.Invoke($"Failed to delete file (locked?): {Path.GetFileName(file)}", Color.Red);
                    }

                }
            }

            foreach (var dir in Directory.GetDirectories(folder))
            {
                // Skip deleting @"Documents" folder if at root
                if (isRoot && string.Equals(new DirectoryInfo(dir).Name, @"Documents", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                DeleteAllFiles(dir, false);

                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception ex)
                {
                    SLogger.WriteLog(ex);
                }
            }
        }

        private bool TryDeleteFile(string filePath, int retries = 3, int delayMs = 200)
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                    return true;
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(delayMs);
                }
                catch (UnauthorizedAccessException)
                {
                    System.Threading.Thread.Sleep(delayMs);
                }
                catch (Exception ex)
                {
                    SLogger.WriteLog(ex);
                    return false;
                }
            }
            return false;
        }


        // Ping the site folder as hostname or IP (simplified)
        private async Task PingSiteAsync(string siteName)
        {
            string hostHeader = @"";
            string ip = @"";
            int port = 0;

            using (ServerManager iisManager = new ServerManager())
            {
                var site = iisManager.Sites[siteName];
                if (site != null)
                {
                    foreach (var binding in site.Bindings)
                    {
                        ip = binding.EndPoint?.Address.ToString() ?? @"";
                        port = binding.EndPoint?.Port ?? 0;
                        hostHeader = binding.Host;

                        // Use first valid binding
                        break;
                    }
                }
            }

            string pingTarget = @"127.0.0.1";

            // Determine ping target
            if (!string.IsNullOrEmpty(hostHeader) && hostHeader != @"*" && hostHeader != @"0.0.0.0")
            {
                pingTarget = hostHeader;
            }
            else if (!string.IsNullOrEmpty(ip) && ip != @"0.0.0.0" && ip != @"::")
            {
                pingTarget = ip;
            }

            if (pingTarget != null)
            {
                var ping = new Ping();
                try
                {
                    var reply = await ping.SendPingAsync($@"{pingTarget}:{port}", 2000);
                    string status = reply.Status == IPStatus.Success ? @"Online" : @"Offline";
                    UpdateSiteStatus(siteName, status);
                }
                catch (Exception ex)
                {
                    UpdateSiteStatus(siteName, @"Offline");
                }
            }
            else
            {
                // Cannot ping unknown or invalid address
                UpdateSiteStatus(siteName, @"Unknown");
            }

        }

        private void UpdateSiteStatus(string siteFolderName, string status)
        {

            foreach (UltraGridRow row in ultraGrid.Rows)
            {
                if (row.Cells[nameof(IISSiteInfo.PhysicalPath)].Value?.ToString() == siteFolderName)
                {
                    row.Cells[nameof(IISSiteInfo.State)].Value = status;
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
            StopIis();
        }

        private void StopIis()
        {
            var selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                lblMsg.Text = @"Please select at least one site.";
                lblMsg.BackColor = Color.Red;
                return;
            }
            foreach (var site in selected)
            {
                StopSite(site.Name);
            }
            LoadSitesFromIis();
        }

        private void btnStartIIS_Click(object sender, EventArgs e)
        {
            StartIis();
        }

        private void StartIis()
        {
            var selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                lblMsg.Text = @"Please select at least one site.";
                lblMsg.BackColor = Color.Red;
                return;
            }
            foreach (var site in selected)
            {
                StartSite(site.Name);
            }
            LoadSitesFromIis();
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
                lblMsg.Text = @"No sites found.";
                lblMsg.BackColor = Color.Red;
                return;
            }

            try
            {
                int totalSites = sites.Count;
                int currentSite = 0;
                foreach (var site in sites)
                {
                    OnProgressUpdated($"Updating '{site.Name}' configurations, ({currentSite} of {totalSites})...", (currentSite * 100) / totalSites, ProgressType.AppSettings);
                    string sourceRoot = _siteBackupDirectory[site.Name];
                    var destRoot = site.PhysicalPath;

                    // Copy Frontend web.config from Existing source to root
                    string frontendWebConfig = Path.Combine(sourceRoot, @"web.config");
                    if (File.Exists(frontendWebConfig))
                    {
                        string destWebConfig = Path.Combine(destRoot, @"web.config");
                        File.Copy(frontendWebConfig, destWebConfig, overwrite: true);
                    }

                    // Copy appsettings.json from Existing directory  PetMatrixBackendAPI
                    string backendAppSettings = Path.Combine(sourceRoot, @"PetMatrixBackendAPI", @"appsettings.json");
                    if (File.Exists(backendAppSettings))
                    {
                        string destApiFolder = Path.Combine(destRoot, @"PetMatrixBackendAPI");
                        Directory.CreateDirectory(destApiFolder);
                        string destAppSettings = Path.Combine(destApiFolder, @"appsettings.json");
                        File.Copy(backendAppSettings, destAppSettings, overwrite: true);
                    }

                    // Copy web.config from Existing directory to PetMatrixBackendAPI
                    string backendWebConfig = Path.Combine(sourceRoot, @"PetMatrixBackendAPI", @"web.config");
                    if (File.Exists(backendWebConfig))
                    {
                        string destApiFolder = Path.Combine(destRoot, @"PetMatrixBackendAPI");
                        Directory.CreateDirectory(destApiFolder);
                        string destAppSettings = Path.Combine(destApiFolder, @"web.config");
                        File.Copy(backendWebConfig, destAppSettings, overwrite: true);
                    }

                    // Copy ReportsViewer web.config
                    string reportsViewerWebConfig = Path.Combine(sourceRoot, @"ReportsViewer", @"Web.config");
                    if (File.Exists(reportsViewerWebConfig))
                    {
                        string destApiFolder = Path.Combine(destRoot, @"ReportsViewer");
                        Directory.CreateDirectory(destApiFolder);
                        string destAppSettings = Path.Combine(destApiFolder, @"Web.config");
                        File.Copy(reportsViewerWebConfig, destAppSettings, overwrite: true);
                    }

                }

                lblMsg.Text = @"Copy completed successfully.";
                StatusUpdated?.Invoke(@"Copy completed successfully.", Color.Green);
            }
            catch (Exception ex)
            {
                StatusUpdated?.Invoke(@$"Error during copy: {ex.Message}", Color.Red);
                SLogger.WriteLog(ex);
            }

        }
        private void btnReloadSites_Click(object sender, EventArgs e)
        {
            LoadSitesFromIis();
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
                lblMsg.Text = @"Select site(s) to ping.";
                lblMsg.BackColor = Color.Red;
                return;
            }
            foreach (var site in selected)
            {
                await PingSiteAsync(site.Name);
            }
        }



        private void btnBackend_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = @"Select Backend Deployment Folder"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {


                string selectedPath = folderDialog.SelectedPath;
                var fileSystemEntries = Directory.EnumerateFileSystemEntries(selectedPath)
                    .Select(Path.GetFileName)
                    .ToList();

                var missingItems = new List<string>();

                foreach (var expected in expectedBackendFiles)
                {
                    bool found = false;

                    if (expected is string s)
                    {
                        string fullPath = Path.Combine(selectedPath, s);
                        if (Directory.Exists(fullPath))
                            found = true;
                        else if (File.Exists(fullPath))
                            found = true;
                    }
                    else if (expected is Regex regex)
                    {
                        found = fileSystemEntries.Any(f => regex.IsMatch(f));
                    }

                    if (!found)
                        missingItems.Add(expected is string str ? str : expected.ToString());
                }

                if (missingItems.Count == 0)
                {
                    lblMsg.BackColor = Color.Green;
                    lblMsg.Text = "✅ All backend deployment files are present.";

                    _backendPath = folderDialog.SelectedPath;
                    txtBackend.Text = _backendPath;

                    ButtonsSwitch(true);
                }
                else
                {
                    StatusUpdated?.Invoke(@"❌Missing files or folders:\n" + string.Join("\n", missingItems), Color.Red);

                }

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
                StatusUpdated?.Invoke("Please set both Site Root and Backup Path before publishing.", Color.Red);
                return;
            }

            var pathMap = new List<(string Path, DeployEnum Type)>
    {
        (_backendPath, DeployEnum.PetMatrixBackendAPI),
        (_frontendPath, DeployEnum.Frontend),
        (_reportPath, DeployEnum.ReportsViewer)
    };

            if (pathMap.All(p => AppUtility.HasNoStr(p.Path)))
            {
                StatusUpdated?.Invoke("Please set at least one path to copy content.", Color.Red);
                return;
            }

            try
            {
                var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };

                foreach (var site in selectedSites)
                {
                   

                    foreach (var (configuredPath, deployType) in pathMap)
                    {
                        string source;
                        if (AppUtility.HasAnyStr(configuredPath))
                        {
                            source = configuredPath;
                        }
                        else
                        {
                            if (!_siteBackupDirectory.TryGetValue(site.Name, out string siteBackupDir))
                            {
                                StatusUpdated?.Invoke($"Backup path not found for site {site.Name}", Color.Red);
                                return;
                            }
                            source = GetDefaultSourcePath(deployType, siteBackupDir);
                        }

                        string destinationFolder;
                        if (deployType == DeployEnum.PetMatrixBackendAPI)
                        {
                            destinationFolder = Path.Combine(site.PhysicalPath, nameof(DeployEnum.PetMatrixBackendAPI));
                        }
                        else if (deployType == DeployEnum.ReportsViewer)
                        {
                            destinationFolder = Path.Combine(site.PhysicalPath, nameof(DeployEnum.ReportsViewer));
                        }
                        else
                        {
                            destinationFolder = site.PhysicalPath;
                        }

                        CopySiteContent(source, destinationFolder);
                    }
                };

                StatusUpdated?.Invoke("Content copied successfully.", Color.Green);
            }
            catch (AggregateException aggEx)
            {
                StatusUpdated?.Invoke("Error copying content: " + aggEx.Flatten().Message, Color.Red);
                SLogger.WriteLog(aggEx);
            }
            catch (Exception ex)
            {
                StatusUpdated?.Invoke("Error copying content: " + ex.Message, Color.Red);
                SLogger.WriteLog(ex);
            }
        }
        private void CopySiteContent(string sourceRoot, string destinationFolder)
        {
            if (!Directory.Exists(sourceRoot))
            {
                StatusUpdated?.Invoke($"Source path not found: {sourceRoot}", Color.Red);
                SLogger.WriteLog($"Source path not found: {sourceRoot}");
                return;
            }

            CopyDirectory(sourceRoot, destinationFolder);
        }

        private string GetDefaultSourcePath(DeployEnum deployType, string siteBackupDir)
        {
            return deployType switch
            {
                //DeployEnum.Frontend => Path.Combine(siteBackupDir, "ARAK-Frontend"),
                DeployEnum.PetMatrixBackendAPI => Path.Combine(siteBackupDir, nameof(DeployEnum.PetMatrixBackendAPI)),
                DeployEnum.ReportsViewer => Path.Combine(siteBackupDir, nameof(DeployEnum.ReportsViewer)),
                _ => siteBackupDir
            };
        }

        // Helper method to set label status
        private void SetStatus(string message, Color color)
        {
            lblMsg.Text = message;
            // lblMsg.BackColor = color;
        }

        private void btnBackupPath_Click(object sender, EventArgs e)
        {
            BackupPath();
        }

        private void BackupPath()
        {
            using var fbd = new FolderBrowserDialog { Description = @"Select Backup Destination Folder" };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            _backupPath = fbd.SelectedPath;
            txtBackup.Text = fbd.SelectedPath;
            ButtonsSwitch(true);
        }

        private void ButtonsSwitch(bool value)
        {
            txtBackup.Text = _backupPath;
            txtFrontend.Text = _frontendPath;
            txtBackend.Text = _backendPath;

            if (!HasNoStr(txtBackup.Text) && txtBackup.Text.Length > 0)
            {
                btnBackupPath.BackColor = Color.Green;
            }
            else
            {
                btnBackupPath.BackColor = Color.Red;
            }

            if (!HasNoStr(txtBackend.Text) && txtBackend.Text.Length > 0)
            {
                btnBackend.BackColor = Color.Green;
            }
            else
            {
                btnBackend.BackColor = Color.Red;
            }

            if (!HasNoStr(txtFrontend.Text) && txtFrontend.Text.Length > 0)
            {
                btnFrontend.BackColor = Color.Green;
            }
            else
            {
                btnFrontend.BackColor = Color.Red;
            }

            if (!HasNoStr(txtReport.Text) && txtReport.Text.Length > 0)
            {
                btnReport.BackColor = Color.Green;
            }
            else
            {
                btnReport.BackColor = Color.Red;
            }



            btnReloadSites.Enabled = value;
            btnBackup.Enabled = value;
            btnStopIIS.Enabled = value;
            btnStartIIS.Enabled = value;
            btnDeleteFiles.Enabled = value;
            btnCopyAppSettings.Enabled = value;
            btnPingSite.Enabled = value;
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
            dt.Columns.Add(nameof(IISSiteInfo.Select), typeof(bool));      // checkbox column
            dt.Columns.Add(nameof(IISSiteInfo.Name), typeof(string));
            dt.Columns.Add(nameof(IISSiteInfo.PhysicalPath), typeof(string));
            dt.Columns.Add(nameof(IISSiteInfo.ContentSize), typeof(string));
            dt.Columns.Add(nameof(IISSiteInfo.State), typeof(string));
            return dt;
        }

        private void CustomizeUltraGridColumns()
        {
            var band = ultraGrid.DisplayLayout.Bands[0];

            band.Columns[nameof(IISSiteInfo.Select)].Header.Caption = nameof(IISSiteInfo.Select);
            band.Columns[nameof(IISSiteInfo.Select)].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            band.Columns[nameof(IISSiteInfo.Select)].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;

            band.Columns[nameof(IISSiteInfo.Name)].Header.Caption = nameof(IISSiteInfo.Name);
            band.Columns[nameof(IISSiteInfo.Name)].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            band.Columns[nameof(IISSiteInfo.PhysicalPath)].Header.Caption = @"Site Folder";
            band.Columns[nameof(IISSiteInfo.PhysicalPath)].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            band.Columns[nameof(IISSiteInfo.ContentSize)].Header.Caption = @"Content Size";
            band.Columns[nameof(IISSiteInfo.ContentSize)].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            band.Columns[nameof(IISSiteInfo.State)].Header.Caption = @"Status";
            band.Columns[nameof(IISSiteInfo.State)].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            // Set Header Font (size, style, color)
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.FontData.SizeInPoints = 13; // Font size
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True; // Bold
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.BackColor = Color.Black; // Text color (black to match white theme)
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.BackColor = Color.LightGray; // Light gray background for header

            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.FontData.SizeInPoints = 13; // Font size
            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True; // Bold
            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.BackColor = Color.Black; // Text color
            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.BackColor = Color.LightGray; // Light gray background for header

            // Optionally, align header text
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center; // Center alignment
            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center; // Center alignment

            // General header customizations for all columns
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.SizeInPoints = 13; // Set header font size for all columns
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True; // Set all headers to bold
            ultraGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.Black; // Set header text color
            ultraGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.LightGray; // Set header background color for all columns



            // Selection settings
            ultraGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            ultraGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;


            // Set the row height to a larger value (default ~20)
            ultraGrid.DisplayLayout.Override.RowSizing = RowSizing.Fixed;

            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.Name = @"Segoe UI";
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.SizeInPoints = 11;
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;

            ultraGrid.DisplayLayout.Override.RowAppearance.FontData.Name = @"Segoe UI";
            ultraGrid.DisplayLayout.Override.RowAppearance.FontData.SizeInPoints = 10;
            ultraGrid.DisplayLayout.Override.RowAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.False;



        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            ClearLables();

            if (HasNoStr(_backendPath) || HasNoStr(_backupPath))
            {
                lblMsg.Text = @"Please set both Site Root and Backup Path before publishing.";
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



        private void OnProgressUpdated(string message, int? percent = null, ProgressType progressFor = ProgressType.Backup)
        {
            ProgressUpdated?.Invoke(this, new ProgressEventArgs(message, percent, progressFor));
        }

        private void DeploymentForm_ProgressUpdated(object? sender, ProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgressUi(e)));
            }
            else
            {
                UpdateProgressUi(e);
            }
        }
        private void DeploymentForm_StatusUpdated(string message, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(() => SetStatus(message, color));
            }
            else
            {
                SetStatus(message, color);
            }
        }

        private void UpdateProgressUi(ProgressEventArgs e)
        {
            lblMsg.Text = e.Message;

            if (e.Percent.HasValue)
            {
                int value = Math.Min(Math.Max(e.Percent.Value, 0), 100);

                switch (e.ProgressFor)
                {
                    case ProgressType.Backup:
                        progressBarBackup.Value = value;
                        break;
                    case ProgressType.Report:
                        progressBarReport.Value = value;
                        break;
                    case ProgressType.Frontend:
                        progressBarFrontend.Value = value;
                        break;
                    case ProgressType.Backend:
                        progressBarBackend.Value = value;
                        break;
                        //toDO: ProgressType.AppSettings
                }
            }
        }

        private void btnFrontend_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = @"Select Angular Build Folder"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {



                string selectedPath = folderDialog.SelectedPath;

                // Get all files and folders names in the root of selected path
                var fileSystemEntries = Directory.EnumerateFileSystemEntries(selectedPath)
                    .Select(Path.GetFileName)
                    .ToList();

                var missingItems = new List<string>();
                foreach (var expected in _expectedFrontendFilesAndFolders)
                {
                    bool found = false;

                    if (expected is string str)
                    {
                        if (str.Equals("assets", StringComparison.OrdinalIgnoreCase))
                        {
                            // Check folder existence
                            string assetsPath = Path.Combine(selectedPath, "assets");
                            found = Directory.Exists(assetsPath);
                        }
                        else
                        {
                            // Check exact file
                            found = fileSystemEntries.Any(f => f.Equals(str, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    else if (expected is Regex regex)
                    {
                        found = fileSystemEntries.Any(f => regex.IsMatch(f));
                    }

                    if (!found)
                    {
                        missingItems.Add(expected is string ? (string)expected : expected.ToString());
                    }

                    if (!found) break;
                }
                if (missingItems.Count == 0)
                {

                    StatusUpdated?.Invoke(@"All required files and folders are present.", Color.Green);

                    _frontendPath = folderDialog.SelectedPath;
                    txtFrontend.Text = _frontendPath;

                    ButtonsSwitch(true);
                }
                else
                {
                    StatusUpdated?.Invoke(@"Missing files or folders:\n" + string.Join("\n", missingItems), Color.Red);
                }

            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = @"Select Report Directory"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                var missingItems = new List<string>();
                foreach (var item in _expectedReportViewerFilesAndFolders)
                {
                    string fullPath = Path.Combine(folderDialog.SelectedPath, item);
                    if (item.Contains(".") && !File.Exists(fullPath)) // file check
                    {
                        missingItems.Add(item);
                    }
                    else if (!item.Contains(".") && !Directory.Exists(fullPath)) // folder check
                    {
                        missingItems.Add(item);
                    }
                }

                if (missingItems.Count == 0)
                {
                    _reportPath = folderDialog.SelectedPath;
                    txtReport.Text = _reportPath;

                    StatusUpdated?.Invoke(@"All required report files and folders are present.", Color.Green);

                }
                else
                {
                    StatusUpdated?.Invoke(@"Missing files or folders:\n" + string.Join("\n", missingItems), Color.Red);
                }

                ButtonsSwitch(true);
            }
        }




    }
}
