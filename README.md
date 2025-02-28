FolderSync

Description

FolderSync is a C# application that synchronizes two folders (source and backup), ensuring that the destination folder (backup) always contains an identical copy of the source folder. Synchronization is performed periodically, and all operations are logged.


Features

One-way synchronization: The replica is always updated to match the source.

Change detection: Copies new files and updates existing ones.

File removal: Deletes files from the replica that no longer exist in the source.

Operation logging: Detailed logs of all operations.

Periodic execution: Synchronization occurs automatically at configurable intervals.

Integrity verification: Uses MD5 to ensure files are identical.


How to Build and Run

Requirements

.NET 8.0 or later installed.

A compatible development environment (e.g. Visual Studio)

Build

If using the command line, navigate to the project directory (where the .csproj file is present) and run:

 dotnet build

Run

Navigate to FolderSync\FolderSync\FolderSync\bin\Debug\net8.0

To execute the program, use the following command:

 FolderSync.exe "<sourcePath>" "<backupPath>" "<logFilePath>" <syncIntervalInSeconds>

 	or, if you're using the Windows Powershell

 .\FolderSync.exe <sourcePath>" "<backupPath>" "<logFilePath>" <syncIntervalInSeconds>

Example:

 FolderSync.exe "C:\SourceFolder" "C:\BackupFolder" "C:\logs\sync.log" 60

This will synchronize C:\SourceFolder with C:\BackupFolder every 60 seconds, logging the actions in sync.log.


Future Improvements

Implement a Windows service to run the application automatically in the background instead of putting a thread to sleep. This affects programs where the synchronization interval is bigger.

Improve logging with structured formats (JSON).

Create a graphical user interface for easy configuration and better readability.


License

This project is released under the MIT license.

Project developed by Ângelo Galvão