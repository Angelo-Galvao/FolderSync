﻿using System.Collections;
using System.Security.Cryptography;

class FolderSync
{
    private readonly string logFilePath;
    private readonly string sourceFilePath;
    private readonly string destinationFilePath;
    private readonly int syncFrequency;

    public FolderSync(string sourcePath, string destinationPath, string logPath, int syncFreq)
    {
        this.logFilePath = logFilePath;
        this.sourceFilePath = sourceFilePath;
        this.destinationFilePath = destinationFilePath;
        this.syncFrequency = syncFrequency;
    }

    private void Log(string message)
    {
        string logMessage = $"[{DateTime.Now}] - {message}\n";
        Console.WriteLine(logMessage);
        File.AppendAllText(logFilePath, logMessage);
    }

    private void synchronizeFolders()
    {

    }

    private bool filesAreEqual(string file1Path, string file2Path)
    {
        using var md5 = MD5.Create();
        using var stream1 = File.OpenRead(file1Path);
        using var stream2 = File.OpenRead(file2Path);

        var hash1 = md5.ComputeHash(stream1);
        var hash2 = md5.ComputeHash(stream2);

        return hash1.SequenceEqual(hash2);
    }

    private void Start()
    {
        while (true)
        {
            try
            {
                synchronizeFolders();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log($"Error during synchronization: {e.Message}");
            }
            Thread.Sleep(syncFrequency);
        }
    }

    public static void main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Uso: FolderSync <sourcePath> <replicaPath> <logFilePath> <syncIntervalInSeconds>");
            return;
        }

        var source = args[0];
        var destination = args[1];
        var logs = args[2];

        if (!int.TryParse(args[3], out int syncFreq) || syncFreq <= 0)
        {
            Console.WriteLine("Error: The synchronization interval must be a positive and integer value.");
            return;
        }

        FolderSync folderSync = new FolderSync(source, destination, logs, syncFreq);
        folderSync.Start();
    }
}