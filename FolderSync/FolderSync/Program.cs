using System.Collections;
using System.Security.Cryptography;

class FolderSync
{
    private readonly string logFilePath;
    private readonly string sourceFilePath;
    private readonly string destinationFilePath;
    private readonly int syncFrequency;

    public FolderSync(string sourcePath, string destinationPath, string logPath, int syncFreq)
    {
        this.logFilePath = logPath;
        this.sourceFilePath = sourcePath;
        this.destinationFilePath = destinationPath;
        this.syncFrequency = syncFreq*1000;
    }

    private void Log(string message)
    {
        string logMessage = $"[{DateTime.Now}] - {message}\n";
        Console.WriteLine(logMessage);
        File.AppendAllText(logFilePath, logMessage);
    }

    private void synchronizeFolders()
    {
        if (!Directory.Exists(sourceFilePath))
        {
            Log("Error: The source folder does not exist.");
            return;
        }

        Directory.CreateDirectory(destinationFilePath);

        long totalSourceSize = GetDirectorySize(sourceFilePath);
        long availableSpace = GetAvailableFreeSpace(destinationFilePath);

        if (availableSpace < totalSourceSize)
        {
            Log($"Error: Not enough space for synchronization. Required: {totalSourceSize} bytes, Available: {availableSpace} bytes.");
            return;
        }

        var sourceFiles = Directory.GetFiles(sourceFilePath, "*", SearchOption.AllDirectories);
        var sourceDirectories = Directory.GetDirectories(sourceFilePath, "*", SearchOption.AllDirectories);
        var backupFiles = Directory.GetFiles(destinationFilePath, "*", SearchOption.AllDirectories);
        var backupDirectories = Directory.GetDirectories(destinationFilePath, "*", SearchOption.AllDirectories);

        foreach (var dir in sourceDirectories)
        {
            var relativePath = dir.Substring(sourceFilePath.Length + 1);
            var backupDir = Path.Combine(destinationFilePath, relativePath);
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
                Log($"Directory created: {relativePath}");
            }
        }

        foreach (var file in sourceFiles)
        {
            var relativePath = file.Substring(sourceFilePath.Length + 1);
            var backupFile = Path.Combine(destinationFilePath, relativePath);

            long fileSize = new FileInfo(file).Length;
            long freeSpace = GetAvailableFreeSpace(destinationFilePath);

            if (freeSpace < fileSize)
            {
                Log($"Error: Not enough space to copy {relativePath}. Required: {fileSize} bytes, Available: {freeSpace} bytes.");
                continue;
            }

            if (!File.Exists(backupFile) || !filesAreEqual(file, backupFile))
            {
                File.Copy(file, backupFile, true);
                Log($"File copied/updated: {relativePath}");
            }
        }

        foreach (var file in backupFiles)
        {
            var relativePath = file.Substring(destinationFilePath.Length + 1);
            var sourceFile = Path.Combine(sourceFilePath, relativePath);

            if (!File.Exists(sourceFile))
            {
                File.Delete(file);
                Log($"File removed: {relativePath}");
            }
        }

        foreach (var dir in backupDirectories.OrderByDescending(d => d.Length))
        {
            var relativePath = dir.Substring(destinationFilePath.Length + 1);
            var sourceDir = Path.Combine(sourceFilePath, relativePath);

            if (!Directory.Exists(sourceDir) && Directory.Exists(dir) && Directory.GetFileSystemEntries(dir).Length == 0)
            {
                Directory.Delete(dir);
                Log($"Directory removed: {relativePath}");
            }
        }
    }

    private long GetAvailableFreeSpace(string path)
    {
        string? root = Path.GetPathRoot(path);

        if (string.IsNullOrEmpty(root))
        {
            Log("Error: Could not determine the drive for the path: " + path);
            return 0;
        }

        DriveInfo drive = new DriveInfo(root);
        return drive.AvailableFreeSpace;
    }

    private long GetDirectorySize(string path)
    {
        return Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                        .Sum(file => new FileInfo(file).Length);
    }

    private bool filesAreEqual(string file1Path, string file2Path)
    {
        FileInfo fileInfo1 = new FileInfo(file1Path);
        FileInfo fileInfo2 = new FileInfo(file2Path);

        if (fileInfo1.Length != fileInfo2.Length || fileInfo1.LastWriteTimeUtc != fileInfo2.LastWriteTimeUtc)
        {
            return false;
        }

        if (IsFileLocked(file1Path) || IsFileLocked(file2Path))
        {
            return true;
        }

        using var md5 = MD5.Create();
        using var stream1 = File.OpenRead(file1Path);
        using var stream2 = File.OpenRead(file2Path);

        var hash1 = md5.ComputeHash(stream1);
        var hash2 = md5.ComputeHash(stream2);

        return hash1.SequenceEqual(hash2);
    }

    private bool IsFileLocked(string filePath)
    {
        try
        {
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
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

    public static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Use: FolderSync <sourcePath> <backupPath> <logFilePath> <syncIntervalInSeconds>");
            return;
        }

        Console.WriteLine("To stop the program, press CTRL+C");

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