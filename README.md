# Server Deployment Utility for IIS Web Applications

## Overview

This Windows Forms application streamlines deployment and management of IIS-hosted web applications, including backend APIs, Angular frontends, and ReportViewer web apps. It helps automate tasks such as site backup, stopping/starting IIS sites, file cleanup, and content publishing.

---

## Features

- **IIS Site Discovery:** Lists all IIS websites with status and physical paths.
- **Selective Site Management:** Choose one or more sites for backup and deployment.
- **Backup:** Backup entire site directories with progress reporting.
- **Stop/Start IIS Sites:** Control IIS sites via AppCmd commands.
- **Delete Site Files:** Safely delete files/folders while excluding critical folders (e.g., `Documents`).
- **Deploy Content:** Copy backend, frontend, and report files from source folders to target sites.
- **Configuration Sync:** Copy/update configuration files (`web.config`, `appsettings.json`) after deployment.
- **File/Folder Validation:** Validates backend, frontend, and report folders for expected files/folders before deployment.
- **Progress Reporting:** UI progress bars and colored status messages update in real-time.

---

## Technology Stack

- .NET 8 Windows Forms
- Infragistics WinForms UltraGrid control
- PowerShell (to retrieve IIS site info)
- Windows AppCmd utility for IIS control
- JSON serialization with `System.Text.Json`

---

## Expected Folder Structures

### Backend Deployment Folder Must Contain
- `Documents/`, `runtimes/`
- Config files: `appsettings.json`, `web.config`, `efpt.config.json`, `libman.json`
- DLLs matching patterns `Microsoft.*.dll`, `System.*.dll`

### Frontend Angular Build Folder Must Contain
- `index.html`, `assets/`
- JS and CSS files matching dynamic regex patterns like `main.*.bundle.js`

### ReportViewer Folder Must Contain
- Directories like `bin`, `Content`, `fonts`, `Scripts`, `SqlServerTypes`
- Files such as `About.aspx`, `Default.aspx`, `ReportViewer.aspx`, `Site.Master`, etc.

---

## Usage

1. Launch the app with administrative privileges.
2. Select sites from the IIS list to backup/deploy.
3. Set backup destination and deployment source folders (backend, frontend, report).
4. Use buttons to perform backup, stop/start sites, delete files, copy configs, and deploy content.
5. Monitor status messages and progress bars for real-time feedback.

---

## How It Works

- Uses PowerShell to list IIS sites with name, physical path, and state.
- Uses Windows AppCmd to stop and start IIS sites.
- Recursively copies or deletes files and folders with UI progress updates.
- Validates expected file sets before deployment to avoid incomplete releases.

---

## Getting Started

- Clone or download the repository.
- Open in Visual Studio 2022+ and build the solution.
- Run the executable as Administrator (required for IIS control).
- Configure the backup and deployment folders before publishing.

---

## Notes

- The tool is designed to work with IIS sites structured similarly to PetMatrix deployments.
- Administrative rights are required for IIS operations.
- The UI leverages Infragistics UltraGrid for a rich user experience.

---

## Author

Miracle Advance Technologies  
Md Hasibul Islam Shanto  
LinkedIn: [bdshanto](https://www.linkedin.com/in/bdshanto)

---

## License

MIT License (see LICENSE file)
