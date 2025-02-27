FolderSync

Description

FolderSync is a C# application that synchronizes two folders (source and backup), ensuring that the destination folder (backup) always contains an identical copy of the source folder. Synchronization is performed periodically, and all operations are logged.

Features

One-way synchronization: The backup is always updated to match the source.

Operation logging: Logs of all operations.

Periodic execution: Synchronization occurs automatically at configurable intervals.


How to Build and Run

Requirements

.NET 8.0 or later installed.

A compatible development environment (e.g. Visual Studio).


Build



Run

To execute the program, use the following command:

 FolderSync.exe <sourcePath> <backupPath> <logFilePath> <syncIntervalInSeconds>

Example:

 FolderSync.exe "C:\SourceFolder" "C:\BackupFolder" "C:\logs\sync.log" 60

This will synchronize C:\SourceFolder with C:\BackupFolder every 60 seconds, logging the actions in sync.log.


Project Structure



Future Improvements

Implement a Windows service to run the application automatically in the background.

Improve logging with structured formats (JSON).

Create a graphical user interface (GUI) for easy configuration and better readability.


License

This project is released under the MIT license.

Project developed by Ângelo Galvão