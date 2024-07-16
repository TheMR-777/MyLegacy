# SLA-Remake: Upgrade Mechanism

The **Evolver**.

## Introduction

This document describes the mechanism for upgrading the SLA-Remake application. Here is a brief overview of the process:

### Overall Approach

1. The client has the `EvolverJob`, which runs at regular intervals (`x` seconds) to check for updates.
   - Checks for the update.
   - Downloads the update.
   - After a successful download, its path is saved in the `SQLite3.Updates` table.

2. Upon user login, the client checks for updates in the `SQLite3.Updates` table.
   - If the path is valid and is a `zip` file, the `evolver` is initiated with the path to the update package passed as an argument.

3. The `evolver` (independent module) immediately:
   - Kills the client app.
   - Creates a backup of the current version.
   - Extracts (unzips) the update package to the installation folder.
   - Restores the SQLite3 cache file from the backup.
   - Initiates the client app with the argument: `--no-start`.
   - Deletes the update package.

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
    version TEXT,
    path TEXT,
    timestamp TEXT
}
```

- The `evolver` must have the following permissions on macOS:
  - 'Accessibility' permission to kill the client app.
  - 'Full Disk Access' permission to access the installation folder.

## Module 01: The Job in Client

The `EvolverJob` runs periodically as a background service to check for updates.

- `EvolverJob` Requests for the Update Information

Possible Response Packet:
```json
{
    "version": "4.1.0.0",
    "url-windows": {
      "link": "<direct-download-link>",
      "hash": "<sha256-hash>"
    },
    "url-macos": {
      "link": "<direct-download-link>",
      "hash": "<sha256-hash>"
    }
}
```

- The client app compares its version with the version received from the server.
- If the version is different, the client app downloads the latest version.
  - The package is placed in a temporary folder provided by .NET.
  - The hash of the downloaded package is verified.
- After a successful download, the `SQLite3.Updates` table is updated with the version and path to the downloaded package.
  - A new entry is created without replacing the old one, maintaining a record of all downloaded updates.

### Exception Cases

- URLs not provided.
- Server is unreachable or down.
- Links are invalid.
- Hash verification fails.

## Module 02: Launching Evolver

This module is invoked after the employee logs in.

- The client app checks for updates in the `SQLite3.Updates` table.
  - Only the latest entry is considered.
- If the version is different, the path is valid, and it is a `zip` file:
  - The `evolver` is initiated with the path to the update package passed as an argument.

### Exception Cases

- The version is the same.
- The path is invalid.
- The path is not a `zip` file.

## Module 03: The Evolver

This module operates independently from the client app. The `evolver` is a separate program made with C#/.NET that quickly performs its tasks.

0. Validation of:
   - Arguments count
   - Path to the update package

1. Kills the client app.
   - Uses the following technique to kill the client app:

```c#
var processes = Process.GetProcesses();
processes.Where(p => p.ProcessName.Contains("SLA-Remake", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(p => p.Kill());
```

2. Creates a backup of the current version.
   - The backup is stored in the `SLA-Remake/Backups` folder with the name `<original-folder-name> - <timestamp>`.
   - The backup contains the entire `SLA-Remake - Windows / macOS` folder.

3. Extracts (unzips) the update package to the installation folder.
   - The update package is unzipped to the `SLA-Remake - Windows / macOS` folder.
   - The update package is deleted after successful extraction.

4. Restores the SQLite3 cache file from the backup.
   - The `SLA-Remake.sqlite` file is restored from the latest backup.
   - The latest two backups are kept, and the rest are deleted upon successful restoration.

5. Initiates the client app with the argument: `--no-start`.
   - The client app runs in the background without starting the main window immediately.

### Exception Cases

- The client app cannot be killed.
- Required permissions are missing.