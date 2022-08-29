using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WindowsAuthFileExplorer.Models;

namespace WindowsAuthFileExplorer.Services.Impl;

public static class FileHashService
{
    public static bool CreateHashFile(string rootPath, string username)
    {
        if (!File.Exists(GetHashFilePath(rootPath, username)))
        {
            using var fs = File.Create(GetHashFilePath(rootPath, username));
            fs.Write(Encoding.ASCII.GetBytes("[]"));
            return true;
        }

        return false;
    }

    public static bool CheckFileHash(string rootPath, string username, string? fileName, string? fileHash)
    {
        var fileHashModels = GetHashFile(GetHashFilePath(rootPath, username));

        if (fileHashModels == null)
        {
            CreateHashFile(rootPath, username);
        }

        var file = fileHashModels?.FirstOrDefault(m => m.Name != null && m.Name.Equals(fileName));
        if (file?.Hash != null && file.Hash.Equals(fileHash))
        {
            return true;
        }

        return false;
    }

    public static void AddFileHash(string rootPath, string username, string fileName)
    {
        var fileHashModels = GetHashFile(GetHashFilePath(rootPath, username));
        if (fileHashModels == null)
        {
            CreateHashFile(rootPath, username);
            fileHashModels = GetHashFile(GetHashFilePath(rootPath, username));
        }

        using var md5 = MD5.Create();
        using var stream = File.OpenRead(Path.Combine(rootPath, username, fileName));
        var hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();

        if (fileHashModels != null && !fileHashModels.Any(f => f.Hash != null && f.Hash.Equals(hash)))
        {
            fileHashModels.Add(new FileHashModel()
            {
                Name = fileName,
                Hash = hash
            });

            UpdateHashFile(rootPath, username, fileHashModels);
        }
    }

    public static void DeleteFileHash(string rootPath, string username, string fileName)
    {
        var fileHashModels = GetHashFile(GetHashFilePath(rootPath, username));
        if (fileHashModels == null)
        {
            CreateHashFile(rootPath, username);
            fileHashModels = GetHashFile(GetHashFilePath(rootPath, username));
        }

        if (fileHashModels != null)
        {
            fileHashModels = fileHashModels.Where(f => f.Name != null && !f.Name.Equals(fileName)).ToList();

            UpdateHashFile(rootPath, username, fileHashModels);
        }
    }

    private static IList<FileHashModel>? GetHashFile(string path)
        => JsonSerializer.Deserialize<IList<FileHashModel>>(File.ReadAllText(path));

    private static void UpdateHashFile(string rootPath, string username, IEnumerable<FileHashModel> fileHashModels)
    {
        var resultJson = JsonSerializer.Serialize(fileHashModels);
        File.WriteAllText(GetHashFilePath(rootPath, username), resultJson);
    }

    public static string GetHashFileName() => "files.json";

    private static string GetHashFilePath(string rootPath, string username) =>
        Path.Combine(rootPath, username, GetHashFileName());
}