using Microsoft.AspNetCore.Mvc;
using WindowsAuthFileExplorer.ExceptionMiddleware.Exceptions;
using WindowsAuthFileExplorer.Models;

namespace WindowsAuthFileExplorer.Services.Impl;

public class FileManagementService : IFileManagementService
{
    private readonly string _filePath;

    public FileManagementService(IHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "Files");
    }

    public IEnumerable<FileListModel> GetUserFiles(string? username)
    {
        if (username == null)
        {
            return new List<FileListModel>();
        }

        var path = Path.Combine(_filePath, username);
        CreateUserDir(_filePath, username);

        var filesInfo = new DirectoryInfo(path).GetFiles();
        return filesInfo
            .Where(f => !f.Name.Equals(FileHashService.GetHashFileName()))
            .Select(f => new FileListModel
        {
            Name = f.Name,
            Size = f.Length
        }).ToList();
    }

    public bool CheckFileWithHash(string? username, string? fileName, string? fileHash)
    {
        if (username == null)
        {
            throw new BadHttpRequestException("Username is not presented");
        }
        
        return FileHashService.CheckFileHash(_filePath, username, fileName, fileHash);
    }

    public async Task<long> SaveFile(string? username, IFormFile? uploadedFile)
    {
        if (uploadedFile == null)
        {
            throw new BadHttpRequestException("File is not presented");
        }

        if (username == null)
        {
            throw new BadHttpRequestException("Username is not presented");
        }
        
        CreateUserDir(_filePath, username);
        
        var path = Path.Combine(_filePath, username, uploadedFile.FileName);
        await using var fileStream = new FileStream(path, FileMode.Create);
        await uploadedFile.CopyToAsync(fileStream);
        await fileStream.DisposeAsync();

        FileHashService.AddFileHash(_filePath, username, uploadedFile.FileName);

        return uploadedFile.Length;
    }

    public void DeleteFile(string? username, string fileName)
    {
        if (username == null)
        {
            throw new BadHttpRequestException("Username is not presented");
        }
        
        if (fileName == null)
        {
            throw new BadHttpRequestException("File name is not presented");
        }

        FileHashService.DeleteFileHash(_filePath, username, fileName);

        var path = Path.Combine(GetDirPath(_filePath, username), fileName);
        File.Delete(path);
    }
    
    public FileResult GetFile(string? username, string? fileName)
    {
        if (fileName == null)
        {
            throw new BadHttpRequestException("File name is not presented");
        }

        if (username == null)
        {
            throw new BadHttpRequestException("Username is not presented");
        }

        var filePath = Path.Combine(_filePath, username, fileName);
        if (File.Exists(filePath) && !fileName.Equals(FileHashService.GetHashFileName()))
        {
            return new PhysicalFileResult(filePath, "application/octet-stream");
        }

        throw new EntityNotFoundException($"File '{fileName}' at '{username}' not found");
    }

    private static void CreateUserDir(string rootPath, string username)
    {
        if (!Directory.Exists(GetDirPath(rootPath, username)))
        {
            Directory.CreateDirectory(GetDirPath(rootPath, username));
            FileHashService.CreateHashFile(rootPath, username);
        }
    }

    private static string GetDirPath(string rootDir, string username) => Path.Combine(rootDir, username);
}