# SLA-Remake: Upgrade Mechanism

The **Evolver**.

## Introduction

This document describes the mechanism for upgrading the SLA-Remake application. Here are the main requirements for the upgrade mechanism:

### Requirements

Inspired from Chromium Updater.

- Absolute Silent Update.
- Support Updating from:
  - Direct Download Link.
  - SMB Shared Drive Path.
- No User Interaction, and Notification.
- Data and State Preservation, and Integrity.
- Only Update when Client is Inactive.
- Reliable and Secure.
- Reliable Rollback Mechanism.

Here is a brief overview of the process:

### Overall Approach

1. The client has the `EvolverJob`, which runs at regular intervals (`x` seconds) to check for updates.
   - Checks for the update.
   - Downloads the update.
   - Extracts (Unzips) the update.
   - Path to the Extracted update is saved in the `SQLite3.UpdatesRecord` table

2. Upon user login, the client checks for updates in the `SQLite3.UpdatesRecord` table.
   - The `evolver` is initiated with the path to the update package passed as an argument, if the update is found.

3. The `evolver` (independent module) immediately:
   - Terminates the client app.
   - Creates a backup of the current version.
   - Moves the update package content to the installation folder.
   - Restores the SQLite3 cache file from the backup.
   - Initiates the client app with the argument: `--silent-start`.

## Initial Setup

- The client has the **SLA-Remake** installed on their system.
- The folder structure is as follows:

```
SLA-Remake
├── SLA-Remake - Windows / macOS
│   ├── SLA-Remake.exe (executable)
│   ├── SLA-Remake.sqlite (cache)
│   └── ...
├── Evolver
│   ├── Evolver.exe (executable)
│   └── ...
├── Backups
│   ├── SLA-Remake - Windows / macOS - <timestamp_01>
│   ├── SLA-Remake - Windows / macOS - <timestamp_02>
│   └── ...
├── Install.sh (script)
└── ...
```

- The `SLA-Remake.sqlite` has an additional table, for update records, with the following schema:

```dbml
Table Update 
{
   id INTEGER PRIMARY KEY AUTOINCREMENT,
   go_live BIT,
   version TEXT,
   path TEXT,
   timestamp DATETIME
}
```

- The `evolver` must have the following permissions on macOS:
  - 'Accessibility' permission to terminate the client app.
  - 'Full Disk Access' permission to access the installation folder.

## Module 01: The Job in Client

The `EvolverJob` runs periodically as a background service to check for updates.

0. `EvolverJob` Requests for the Update Information

Possible Response Packet:
```json
{
    "version": "4.1.0.0",
    "url-windows": {
      "link": "<direct-download-link-or-shared-drive-path>",
      "hash": "<sha256-hash>"
    },
    "url-macos": {
      "link": "<direct-download-link-or-shared-drive-path>",
      "hash": "<sha256-hash>"
    }
}
```

1. The client app verifies the package to download, according to the following criteria:
   - The received version is higher than the current version.
   - The received version is not present in the `SQLite3.UpdatesRecord` table.
   - The URL is provided, and is valid.
   - The hash is provided, and is valid.
  
2. If the above criteria is met, the client app downloads the package (in a temporary folder provided by .NET), and validates the package as follows:
   - The package must be a `zip` file.
   - The hash of the package must match the provided hash.
   - The folder structure must be similar to:
```
"<extracted-dir>/SLA-Remake - Windows or macOS/SLA-Remake.exe"
```

3. If the above criteria is met, the package is extracted.
   - The extracted package is also placed in a temporary folder provided by .NET.
   - The `zip` is deleted after successful extraction.
   
4. The path to the extracted package is saved in the `SQLite3.UpdatesRecord` table with the following schema:
   - `go_live`: set to `1`.
   - `version`: The version received from the server.
   - `timestamp`: The current timestamp.
   - `path`: The path to the extracted package.
   - A new entry is created without replacing the old one, maintaining a record of all downloaded updates.

### Exception Cases

- Any of the criteria is not met.
- Download link is not a Direct Download Link.
- Server is unreachable or down.

## Module 02: Launching Evolver

This module is invoked after the employee logs in.

- The client app checks for updates in the `SQLite3.UpdatesRecord` table.
  - Only the latest entry is considered.
  
-  If the following criteria is met, The `evolver` is initiated with the path to the update package passed as an argument.
   - The `path` is valid.
   - The `go_live` flag is set to `1`.
  
- The `go_live` flag is set to `0` just before the `evolver` is initiated.

### Exception Cases

- The path is invalid.

## Module 03: The Evolver

This module operates independently from the client app. The `evolver` is a separate program made with C#/.NET that quickly performs its tasks. The `evolver` only contains those tasks, which client app cannot perform due to its own termination.

0. Validation of:
   - Arguments count
   - Path to the update package

1. Terminates the client app.
   - Uses the following technique to terminate the client app:

```c#
var processes = Process.GetProcesses();
processes.Where(p => p.ProcessName.Contains("SLA-Remake", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(p => p.Kill());
```

2. Creates a backup of the current version.
   - The backup is stored in the `SLA-Remake/Backups` folder with the name `<original-folder-name> - <timestamp>`.
   - The backup contains the entire `SLA-Remake - Windows / macOS` folder.

3. Moves the new content to the installation folder.
   - The update package is copied to `SLA-Remake` folder, as `SLA-Remake - Windows / macOS`.
   - The update package is deleted after successful operation.

4. Restores the SQLite3 cache file from the backup.
   - The `SLA-Remake.sqlite` file is restored from the latest backup.
   - The latest two backups are kept, and the rest are deleted upon successful restoration.

5. Initiates the client app with the argument: `--silent-start`.
   - The client app runs in the background without starting the main window immediately.

### Exception Cases

- The client app cannot be terminated.
- Required permissions are missing.
