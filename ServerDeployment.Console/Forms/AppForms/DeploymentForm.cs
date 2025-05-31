using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using ServerDeployment.Applications.Helpers;
using ServerDeployment.Console.Helpers;
using ServerDeployment.Domains.Utility;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Application = System.Windows.Forms.Application;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace ServerDeployment.Console.Forms.AppForms
{
    public partial class DeploymentForm : Form
    {
        private DataTable _sitesDataTable;

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


            ButtonAppearance();
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

        private async Task LoadSitesFromIisAsync()
        {
            List<IISSiteInfo> sites = GetIISSites();
            DataTable dt = CreateSitesDataTable();

            List<IISSiteInfo> selectedSites = GetSelectedSites();

            // For better performance, calculate sizes asynchronously in parallel
            var sizeTasks = sites.Select(site => Task.Run(() =>
            {
                string sizeStr = string.Empty;
                try
                {
                    sizeStr = AppUtility.GetDirectorySize(site.PhysicalPath);
                }
                catch
                {
                    sizeStr = "N/A";
                }
                return (Site: site, Size: sizeStr);
            })).ToArray();

            var results = await Task.WhenAll(sizeTasks);

            foreach (var result in results)
            {
                var site = result.Site;
                string contentSize = result.Size;

                bool isSelected = false;
                var selected = selectedSites.FirstOrDefault(c => c.Name.Equals(site.Name));
                if (selected != null)
                {
                    isSelected = selected.Select;
                }

                dt.Rows.Add(isSelected, site.Name, site.PhysicalPath, contentSize, site.State);
            }

            // Bind to UI thread
            if (ultraGrid.InvokeRequired)
            {
                ultraGrid.Invoke(new Action(() =>
                {
                    BindAndCustomizeGrid(dt);
                }));
            }
            else
            {
                BindAndCustomizeGrid(dt);
            }
        }

        private void BindAndCustomizeGrid(DataTable dt)
        {
            ultraGrid.DataSource = dt;

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

        private void CopyDirectory(string sourceDir, string destDir, Action onFileCopied)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
            {
                SLogger.WriteLog($"{nameof(CopyDirectory)}: Source directory does not exist: {sourceDir}");
                StatusUpdated?.Invoke($"Source directory does not exist: {sourceDir}", Color.Red);
                return;
            }

            Directory.CreateDirectory(destDir);

            foreach (var file in dir.GetFiles())
            {
                try
                {
                    string destFile = Path.Combine(destDir, file.Name);
                    file.CopyTo(destFile, true);
                    onFileCopied?.Invoke(); // report file copied
                }
                catch (Exception ex)
                {
                    StatusUpdated?.Invoke($"Failed to copy file: {file.Name}. Error: {ex.Message}", Color.Red);
                    SLogger.WriteLog(ex);
                }
            }

            foreach (var subDir in dir.GetDirectories())
            {
                CopyDirectory(subDir.FullName, Path.Combine(destDir, subDir.Name), onFileCopied);
            }
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

        // Start IIS site by folder name using appcmd
        private void StartSite(string siteFolderName)
        {
            var siteName = siteFolderName;
            RunAppCmd($"start site \"{siteName}\"");

            UpdateSiteStatus(siteFolderName, "Started");

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

        private void UpdateSiteStatus(string siteFolderName, string status)
        {
            foreach (UltraGridRow row in ultraGrid.Rows)
            {
                if (row.Cells[nameof(IISSiteInfo.PhysicalPath)].Value?.ToString() == siteFolderName)
                {
                    row.Cells[nameof(IISSiteInfo.State)].Value = status;
                    break; // Exit after updating first match
                }
            }
        }

        private async void btnBackup_Click(object sender, EventArgs e)
        {
            // BackupSelectedSites();

            await BackupSelectedSitesAsync();
        }
        private async Task BackupSelectedSitesAsync()
        {
            SetStatus("", Color.White);

            var selectedSites = GetSelectedSites();
            if (selectedSites.Count <= 0)
            {
                StatusUpdated?.Invoke("Please select at least one site to backup.", Color.Red);
                progressBarBackup.Visible = false;
                return;
            }

            int totalFiles = CountFilesInSites(selectedSites);
            if (totalFiles == 0)
            {
                StatusUpdated?.Invoke("No files to backup.", Color.Red);
                progressBarBackend.Visible = false;
                return;
            }


            btnBackup.Enabled = false;
            progressBarBackend.Minimum = 0;
            progressBarBackend.Maximum = 100;
            progressBarBackend.Value = 0;
            progressBarBackend.Visible = true;

            int copiedFiles = 0;
            object lockObj = new object();

            await Task.Run(() =>
            {
                foreach (var site in selectedSites)
                {
                    try
                    {
                        string sourceDir = site.PhysicalPath;
                        string backupDir = Path.Combine(txtBackup.Text, $"{site.Name}_backup_{DateTime.Now:yyyyMMddHHmmss}");

                        // Pass callback to increment copied files and update progress
                        CopyDirectory(sourceDir, backupDir, () =>
                        {
                            lock (lockObj)
                            {
                                copiedFiles++;
                                int progressPercent = (copiedFiles * 100) / totalFiles;
                                this.Invoke(new Action(() =>
                                {
                                    progressBarBackend.Value = progressPercent;
                                    btnBackup.Text = $"Backing up... {progressPercent}%";
                                    StatusUpdated?.Invoke($"Backing up... {progressPercent}%", Color.Black);
                                }));
                            }
                        });

                        lock (_siteBackupDirectory)
                        {
                            if (_siteBackupDirectory.ContainsKey(site.Name))
                                _siteBackupDirectory.Remove(site.Name);
                            _siteBackupDirectory.Add(site.Name, backupDir);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() =>
                        {
                            StatusUpdated?.Invoke($"Failed to backup site '{site.Name}': {ex.Message}", Color.Red);
                        }));
                    }
                }
            });

            this.Invoke(new Action(() =>
            {
                progressBarBackend.Value = 100;
                btnBackup.Text = "Backup Completed";
                btnBackup.Enabled = true;
                StatusUpdated?.Invoke("Backup completed.", Color.Green);
                progressBarBackend.Visible = false;
            }));
        }
        private async void btnStopIIS_Click(object sender, EventArgs e)
        {
            await StopIisAsync();
        }
        private async Task StopIisAsync()
        {
            var selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                SetStatus("Please select at least one site.", Color.Red);
                return;
            }

            foreach (var site in selected)
            {
                await StopSiteAsync(site.Name);
            }

            await LoadSitesFromIisAsync();
        }

        private Task StopSiteAsync(string siteFolderName)
        {
            return Task.Run(() =>
            {
                RunAppCmd($"stop site \"{siteFolderName}\"");
            });
        }


        private int CountFilesInSites(List<IISSiteInfo> sites)
        {
            int totalFiles = 0;
            foreach (var site in sites)
            {
                if (Directory.Exists(site.PhysicalPath))
                {
                    totalFiles += Directory.GetFiles(site.PhysicalPath, "*", SearchOption.AllDirectories).Length;
                }
            }
            return totalFiles;
        }


        private async void btnStartIIS_Click(object sender, EventArgs e)
        {
            await StartIisAsync();
        }

        private async Task StartIisAsync()
        {
            var selected = GetSelectedSites();
            if (selected.Count == 0)
            {
                SetStatus("Please select at least one site.", Color.Red);
                return;
            }

            int totalSites = selected.Count;
            int currentSite = 0;

            await Task.Run(() =>
            {
                foreach (var site in selected)
                {
                    currentSite++;

                    // Start the site
                    StartSite(site.Name);

                    // Update status on UI thread
                    this.Invoke(new Action(() =>
                    {
                        SetStatus($"Started site '{site.Name}' ({currentSite} of {totalSites})", Color.Black);
                    }));
                }
            });

            // Optionally final status
            // SetStatus("All selected sites started.", Color.Green);


        }
        private async void btnDeleteFiles_Click(object sender, EventArgs e)
        {
            await DeleteFilesInSitesAsync();
        }
        private async Task DeleteFilesInSitesAsync()
        {
            var selectedSites = GetSelectedSites();
            if (selectedSites.Count == 0)
            {
                StatusUpdated?.Invoke("Please select at least one site.", Color.Red);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtBackend.Text) || !Directory.Exists(txtBackend.Text))
            {
                StatusUpdated?.Invoke("Please select a valid Backend Deployment Folder.", Color.Red);
                return;
            }



            int totalSites = selectedSites.Count;
            int currentSite = 0;

            btnDeleteFiles.Enabled = false;

            await Task.Run(() =>
            {
                foreach (var site in selectedSites)
                {
                    currentSite++;
                    string folder = Path.Combine(txtBackend.Text, site.PhysicalPath);

                    this.Invoke(() =>
                    {
                        OnProgressUpdated($"Deleting site '{site.Name}' ({currentSite} of {totalSites})...",
                            (currentSite * 100) / totalSites,
                            ProgressType.Delete);
                    });
                    try
                    {
                        DeleteAllFiles(folder);
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(() =>
                        {
                            StatusUpdated?.Invoke($"Error deleting files in {site.Name}: {ex.Message}", Color.Red);
                        });
                        SLogger.WriteLog(ex);
                    }
                }
            });

            btnDeleteFiles.Enabled = true;

            StatusUpdated?.Invoke("Files deleted.", Color.Green);
        }

        private async void btnCopyAppSettings_Click(object sender, EventArgs e)
        {
            await CopyAppSettingsAsync();
        }

        private async Task CopyAppSettingsAsync()
        {
            var sites = GetIISSites();
            if (sites.Count <= 0)
            {
                SetStatus("No sites found.", Color.Red);
                return;
            }

            btnCopyAppSettings.Enabled = false;

            int totalSites = sites.Count;
            int currentSite = 0;

            try
            {
                foreach (var site in sites)
                {
                    currentSite++;

                    string progressMessage = $"Updating '{site.Name}' configurations, ({currentSite} of {totalSites})...";
                    int progressPercent = (currentSite * 100) / totalSites;

                    // Report progress on UI thread
                    this.Invoke(() => OnProgressUpdated(progressMessage, progressPercent, ProgressType.AppSettings));

                    string sourceRoot;
                    if (!_siteBackupDirectory.TryGetValue(site.Name, out sourceRoot))
                    {
                        StatusUpdated?.Invoke($"Backup directory not found for site {site.Name}.", Color.Red);
                        continue; // Skip this site but continue with others
                    }

                    var destRoot = site.PhysicalPath;

                    await Task.Run(() =>
                    {
                        // Frontend web.config
                        string frontendWebConfig = Path.Combine(sourceRoot, "web.config");
                        if (File.Exists(frontendWebConfig))
                        {
                            string destWebConfig = Path.Combine(destRoot, "web.config");
                            File.Copy(frontendWebConfig, destWebConfig, true);
                        }

                        // Backend appsettings.json
                        string backendAppSettings = Path.Combine(sourceRoot, "PetMatrixBackendAPI", "appsettings.json");
                        if (File.Exists(backendAppSettings))
                        {
                            string destApiFolder = Path.Combine(destRoot, "PetMatrixBackendAPI");
                            Directory.CreateDirectory(destApiFolder);
                            string destAppSettings = Path.Combine(destApiFolder, "appsettings.json");
                            File.Copy(backendAppSettings, destAppSettings, true);
                        }

                        // Backend web.config
                        string backendWebConfig = Path.Combine(sourceRoot, "PetMatrixBackendAPI", "web.config");
                        if (File.Exists(backendWebConfig))
                        {
                            string destApiFolder = Path.Combine(destRoot, "PetMatrixBackendAPI");
                            Directory.CreateDirectory(destApiFolder);
                            string destWebConfigDest = Path.Combine(destApiFolder, "web.config");
                            File.Copy(backendWebConfig, destWebConfigDest, true);
                        }

                        // ReportsViewer web.config
                        string reportsViewerWebConfig = Path.Combine(sourceRoot, "ReportsViewer", "Web.config");
                        if (File.Exists(reportsViewerWebConfig))
                        {
                            string destApiFolder = Path.Combine(destRoot, "ReportsViewer");
                            Directory.CreateDirectory(destApiFolder);
                            string destWebConfigDest = Path.Combine(destApiFolder, "Web.config");
                            File.Copy(reportsViewerWebConfig, destWebConfigDest, true);
                        }
                    });
                }

                StatusUpdated?.Invoke("Copy completed successfully.", Color.Green);
            }
            catch (Exception ex)
            {
                StatusUpdated?.Invoke($"Error during copy: {ex.Message}", Color.Red);
                SLogger.WriteLog(ex);
            }
            finally
            {
                btnCopyAppSettings.Enabled = true;
            }
        }

        private void btnReloadSites_Click(object sender, EventArgs e)
        {
            LoadSitesFromIis();
        }

        #region Backend

        private bool CheckBackendFiles(string path, out List<string> missingItems)
        {
            missingItems = new List<string>();

            var fileSystemEntries = Directory.Exists(path)
                ? Directory.EnumerateFileSystemEntries(path).Select(Path.GetFileName).ToList()
                : new List<string>();

            foreach (var expected in expectedBackendFiles)
            {
                bool found = false;

                if (expected is string s)
                {
                    string fullPath = Path.Combine(path, s);
                    found = Directory.Exists(fullPath) || File.Exists(fullPath);
                }
                else if (expected is Regex regex)
                {
                    found = fileSystemEntries.Any(f => regex.IsMatch(f));
                }

                if (!found)
                    missingItems.Add(expected is string str ? str : expected.ToString());
            }

            return missingItems.Count == 0;
        }

        private void txtBackend_Leave(object sender, EventArgs e)
        {
            if (txtBackend.Text.StartsWith("\"") && txtBackend.Text.EndsWith("\""))
            {
                txtBackend.Text = txtBackend.Text.Trim('"');
            }

            if (AppUtility.HasAnyStr(txtBackend.Text))
            {
                bool allPresent = CheckBackendFiles(txtBackend.Text, out var missingItems);

                if (allPresent)
                {
                    StatusUpdated?.Invoke("✅ All backend deployment files are present.", Color.Green);
                    ButtonAppearance();
                }
                else
                {
                    txtBackend.Text = "";
                    StatusUpdated?.Invoke("❌ Missing files or folders:" + Environment.NewLine + string.Join(Environment.NewLine, missingItems), Color.Red);
                }
            }
        }

        private void btnBackend_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = "Select Backend Deployment Folder"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath;

                bool allPresent = CheckBackendFiles(selectedPath, out var missingItems);

                if (allPresent)
                {
                    StatusUpdated?.Invoke("✅ All backend deployment files are present.", Color.Green);
                    txtBackend.Text = selectedPath;
                    ButtonAppearance();
                }
                else
                {
                    StatusUpdated?.Invoke("❌ Missing files or folders:" + Environment.NewLine + string.Join(Environment.NewLine, missingItems), Color.Red);
                }
            }
        }


        #endregion

        #region Frontend

        private bool ValidateFrontendFolder(string selectedPath, out List<string> missingItems)
        {
            missingItems = new List<string>();

            if (!Directory.Exists(selectedPath))
            {
                missingItems.Add("Folder does not exist.");
                return false;
            }

            var fileSystemEntries = Directory.EnumerateFileSystemEntries(selectedPath)
                .Select(Path.GetFileName)
                .ToList();

            foreach (var expected in _expectedFrontendFilesAndFolders)
            {
                bool found = false;

                if (expected is string str)
                {
                    if (str.Equals("assets", StringComparison.OrdinalIgnoreCase))
                    {
                        string assetsPath = Path.Combine(selectedPath, "assets");
                        found = Directory.Exists(assetsPath);
                    }
                    else
                    {
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
                    break; // Stop on first missing item
                }
            }

            return missingItems.Count == 0;
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

                if (ValidateFrontendFolder(selectedPath, out var missingItems))
                {
                    StatusUpdated?.Invoke("All required files and folders are present.", Color.Green);
                    txtFrontend.Text = selectedPath;
                    ButtonAppearance();
                }
                else
                {
                    StatusUpdated?.Invoke("Missing files or folders:" + Environment.NewLine + string.Join(Environment.NewLine, missingItems), Color.Red);
                }
            }
        }

        // Consider using txtFrontend_Leave instead of KeyUp to avoid too frequent validations
        private void txtFrontend_Leave(object sender, EventArgs e)
        {
            // Trim quotes if any
            if (txtFrontend.Text.StartsWith("\"") && txtFrontend.Text.EndsWith("\""))
            {
                txtFrontend.Text = txtFrontend.Text.Trim('"');
            }

            if (AppUtility.HasAnyStr(txtFrontend.Text))
            {
                if (ValidateFrontendFolder(txtFrontend.Text, out var missingItems))
                {
                    StatusUpdated?.Invoke("All required files and folders are present.", Color.Green);
                    ButtonAppearance();
                }
                else
                {
                    StatusUpdated?.Invoke("Missing files or folders:" + Environment.NewLine + string.Join(Environment.NewLine, missingItems), Color.Red);
                }
            }
        }


        #endregion


        #region Report
        private bool ValidateReportDirectory(string path, out List<string> missingItems)
        {
            missingItems = new List<string>();

            if (!Directory.Exists(path))
            {
                missingItems.Add("Directory does not exist.");
                return false;
            }

            foreach (var item in _expectedReportViewerFilesAndFolders)
            {
                string fullPath = Path.Combine(path, item);

                if (item.Contains("."))
                {
                    if (!File.Exists(fullPath))
                        missingItems.Add(item);
                }
                else
                {
                    if (!Directory.Exists(fullPath))
                        missingItems.Add(item);
                }
            }

            return missingItems.Count == 0;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = "Select Report Directory"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath;
                if (ValidateReportDirectory(selectedPath, out var missingItems))
                {
                    txtReport.Text = selectedPath;
                    StatusUpdated?.Invoke("All required report files and folders are present.", Color.Green);
                    ButtonAppearance();
                }
                else
                {
                    StatusUpdated?.Invoke("Missing files or folders:" + Environment.NewLine + string.Join(Environment.NewLine, missingItems), Color.Red);
                }
            }
        }

        private void txtReport_Leave(object? sender, EventArgs e)
        {
            if (txtReport.Text.StartsWith("\"") && txtReport.Text.EndsWith("\""))
            {
                txtReport.Text = txtReport.Text.Trim('"');
            }

            if (AppUtility.HasAnyStr(txtReport.Text))
            {
                if (ValidateReportDirectory(txtReport.Text, out var missingItems))
                {
                    StatusUpdated?.Invoke("All required report files and folders are present.", Color.Green);
                    ButtonAppearance();
                }
                else
                {
                    txtReport.Text = string.Empty;
                    StatusUpdated?.Invoke("Missing files or folders:" + Environment.NewLine + string.Join(Environment.NewLine, missingItems), Color.Red);
                }
            }
            else
            {
                txtReport.Text = string.Empty;
                StatusUpdated?.Invoke("Please select a valid Report Directory.", Color.Red);
            }
        }


        #endregion

        private async void btnCopyContent_Click(object sender, EventArgs e)
        {
            await CopyContentAsync();
        }
        private async Task CopyContentAsync()
        {
            var selectedSites = GetSelectedSites();
            if (selectedSites.Count == 0)
            {
                StatusUpdated?.Invoke("Please set both Site Root and Backup Path before publishing.", Color.Red);
                return;
            }

            var pathMap = new List<(string Path, DeployEnum Type)>
                            {
                                (txtBackend.Text, DeployEnum.PetMatrixBackendAPI),
                                (txtFrontend.Text, DeployEnum.Frontend),
                                (txtReport.Text, DeployEnum.ReportsViewer)
                            };

            if (pathMap.All(p => AppUtility.HasNoStr(p.Path)))
            {
                StatusUpdated?.Invoke("Please set at least one path to copy content.", Color.Red);
                return;
            }

            // Count total files for progress reporting
            int totalFiles = 0;
            foreach (var site in selectedSites)
            {
                foreach (var p in pathMap)
                {
                    string source;
                    if (AppUtility.HasAnyStr(p.Path))
                    {
                        source = p.Path;
                    }
                    else
                    {
                        string backupDir;
                        if (!_siteBackupDirectory.TryGetValue(site.Name, out backupDir))
                        {
                            StatusUpdated?.Invoke($"Backup path not found for site {site.Name}", Color.Red);
                            btnCopyContent.Enabled = true;
                            return;
                        }
                        source = GetDefaultSourcePath(p.Type, backupDir);
                    }

                    if (source != null && Directory.Exists(source))
                    {
                        totalFiles += Directory.GetFiles(source, "*", SearchOption.AllDirectories).Length;
                    }
                }
            }

            if (totalFiles == 0)
            {
                StatusUpdated?.Invoke("No files to copy.", Color.Red);
                return;
            }

            int copiedFiles = 0;
            object lockObj = new object();

            btnCopyContent.Enabled = false;

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
                        if (!_siteBackupDirectory.TryGetValue(site.Name, out var siteBackupDir))
                        {
                            StatusUpdated?.Invoke($"Backup path not found for site {site.Name}", Color.Red);
                            btnCopyContent.Enabled = true;
                            return;
                        }

                        source = GetDefaultSourcePath(deployType, siteBackupDir);
                    }

                    string destinationFolder = deployType switch
                    {
                        DeployEnum.PetMatrixBackendAPI => Path.Combine(site.PhysicalPath, nameof(DeployEnum.PetMatrixBackendAPI)),
                        DeployEnum.ReportsViewer => Path.Combine(site.PhysicalPath, nameof(DeployEnum.ReportsViewer)),
                        _ => site.PhysicalPath,
                    };

                    await Task.Run(() =>
                    {
                        CopyDirectory(source, destinationFolder, () =>
                        {
                            lock (lockObj)
                            {
                                copiedFiles++;
                                int percent = (copiedFiles * 100) / totalFiles;
                                this.Invoke(() =>
                                {
                                    StatusUpdated?.Invoke($"Copied {copiedFiles} of {totalFiles} files ({percent}%)", Color.Black);
                                    // Optionally update a progress bar here
                                });
                            }
                        });
                    });
                }
            }

            StatusUpdated?.Invoke("Content copied successfully.", Color.Green);
            btnCopyContent.Enabled = true;
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
        //SystemColors.Control
        private void SetStatus(string message, Color color)
        {
            if (lblMsg.InvokeRequired)
            {
                lblMsg.Invoke(new Action(() =>
                {
                    lblMsg.Text = message;
                    lblMsg.ForeColor = color;
                }));
            }
            else
            {
                lblMsg.Text = message;
                lblMsg.ForeColor = color;
            }
        }

        private void btnBackupPath_Click(object sender, EventArgs e)
        {
            BackupPath();
        }

        private void BackupPath()
        {
            using var fbd = new FolderBrowserDialog { Description = @"Select Backup Destination Folder" };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            txtBackup.Text = fbd.SelectedPath;
            ButtonAppearance();
        }

        private void ButtonAppearance()
        {

            if (AppUtility.HasAnyStr(txtBackup.Text))
            {
                btnBackupPath.ForeColor = Color.Green;
            }
            else
            {
                btnBackupPath.ForeColor = Color.Red;
            }

            if (AppUtility.HasAnyStr(txtBackend.Text))
            {
                btnBackend.ForeColor = Color.Green;
            }
            else
            {
                btnBackend.ForeColor = Color.Red;
            }
            if (AppUtility.HasAnyStr(txtFrontend.Text))
            {
                btnFrontend.ForeColor = Color.Green;
            }
            else
            {
                btnFrontend.ForeColor = Color.Red;
            }
            if (AppUtility.HasAnyStr(txtReport.Text))

            {
                btnReport.ForeColor = Color.Green;
            }
            else
            {
                btnReport.ForeColor = Color.Red;
            }
        }

        private void ButtonSwitch(bool value)
        {
            btnReloadSites.Enabled = value;
            btnBackup.Enabled = value;
            btnStopIIS.Enabled = value;
            btnStartIIS.Enabled = value;
            btnDeleteFiles.Enabled = value;
            btnCopyAppSettings.Enabled = value;
            btnCopyContent.Enabled = value;
        }



        private DataTable CreateSitesDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(nameof(IISSiteInfo.Select), typeof(bool)); // checkbox column
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
            band.Columns[nameof(IISSiteInfo.Select)].Style = ColumnStyle.CheckBox;
            band.Columns[nameof(IISSiteInfo.Select)].CellActivation = Activation.AllowEdit;

            band.Columns[nameof(IISSiteInfo.Name)].Header.Caption = nameof(IISSiteInfo.Name);
            band.Columns[nameof(IISSiteInfo.Name)].CellActivation = Activation.NoEdit;

            band.Columns[nameof(IISSiteInfo.PhysicalPath)].Header.Caption = @"Site Folder";
            band.Columns[nameof(IISSiteInfo.PhysicalPath)].CellActivation = Activation.NoEdit;

            band.Columns[nameof(IISSiteInfo.ContentSize)].Header.Caption = @"Content Size";
            band.Columns[nameof(IISSiteInfo.ContentSize)].CellActivation = Activation.NoEdit;

            band.Columns[nameof(IISSiteInfo.State)].Header.Caption = @"Status";
            band.Columns[nameof(IISSiteInfo.State)].CellActivation = Activation.NoEdit;

            // Set Header Font (size, style, color)
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.FontData.SizeInPoints = 13; // Font size
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.FontData.Bold = DefaultableBoolean.True; // Bold
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.ForeColor = Color.Black; // Text color (black to match white theme)
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.ForeColor = Color.LightGray; // Light gray background for header

            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.FontData.SizeInPoints = 13; // Font size
            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.FontData.Bold = DefaultableBoolean.True; // Bold
            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.ForeColor = Color.Black; // Text color
            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.ForeColor = Color.LightGray; // Light gray background for header

            // Optionally, align header text
            band.Columns[nameof(IISSiteInfo.Name)].Header.Appearance.TextHAlign = HAlign.Center; // Center alignment
            band.Columns[nameof(IISSiteInfo.State)].Header.Appearance.TextHAlign = HAlign.Center; // Center alignment

            // General header customizations for all columns
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.SizeInPoints = 13; // Set header font size for all columns
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True; // Set all headers to bold
            ultraGrid.DisplayLayout.Override.HeaderAppearance.ForeColor = Color.Black; // Set header text color
            ultraGrid.DisplayLayout.Override.HeaderAppearance.ForeColor = Color.LightGray; // Set header background color for all columns


            // Selection settings
            ultraGrid.DisplayLayout.Override.SelectTypeRow = SelectType.Extended;
            ultraGrid.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
            ultraGrid.DisplayLayout.Override.RowSelectors = DefaultableBoolean.False;


            // Set the row height to a larger value (default ~20)
            ultraGrid.DisplayLayout.Override.RowSizing = RowSizing.Fixed;

            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.Name = @"Segoe UI";
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.SizeInPoints = 11;
            ultraGrid.DisplayLayout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;

            ultraGrid.DisplayLayout.Override.RowAppearance.FontData.Name = @"Segoe UI";
            ultraGrid.DisplayLayout.Override.RowAppearance.FontData.SizeInPoints = 10;
            ultraGrid.DisplayLayout.Override.RowAppearance.FontData.Italic = DefaultableBoolean.False;
        }

        private async void btnPublish_Click(object sender, EventArgs e)
        {
            // Clear previous messages

            SetStatus("", Color.Red);

            // Validate paths and at least one path length should be > 0
            if (AppUtility.HasNoStr(txtBackend.Text)
                && AppUtility.HasNoStr(txtFrontend.Text)
                && AppUtility.HasNoStr(txtReport.Text)
                )
            {
                StatusUpdated?.Invoke("Please set at least one path to publish content.", Color.Red);
                return;
            }
            if (AppUtility.HasNoStr(txtBackup.Text))
            {
                StatusUpdated?.Invoke("Please select the backup path", Color.Yellow);
                return;
            }

            // 1. Backup selected sites
            await BackupSelectedSitesAsync();
            if (_siteBackupDirectory.Count == 0)
            {
                StatusUpdated?.Invoke("No sites were backed up. Please select sites to backup.", Color.Red);
                return;
            }

            // 2. STOP IIS Sites
            await StopIisAsync();

            // 3. Delete files in selected sites
            await DeleteFilesInSitesAsync();

            // 4. Copy content to sites
            await CopyContentAsync();

            // 5. Copy appsettings and web.config files
            await CopyAppSettingsAsync();


            // 6. Start IIS Sites
            await StartIisAsync();





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
            StatusUpdated?.Invoke(e.Message, Color.Green);

            if (!e.Percent.HasValue) return;

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



        private void txtBackup_Leave(object? sender, EventArgs e)
        {
            if (txtBackup.Text.StartsWith("\"") && txtBackup.Text.EndsWith("\""))
            {
                txtBackup.Text = txtBackup.Text.Trim('"');
            }
        }

    }
}