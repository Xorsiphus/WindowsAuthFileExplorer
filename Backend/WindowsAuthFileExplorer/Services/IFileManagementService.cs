using Microsoft.AspNetCore.Mvc;
using WindowsAuthFileExplorer.Models;

namespace WindowsAuthFileExplorer.Services;

public interface IFileManagementService
{
    IEnumerable<FileListModel> GetUserFiles(string? username);
    Task<long> SaveFile(string? username, IFormFile? uploadedFile);
    FileResult? GetFile(string? username, string? fileName);
    bool CheckFileWithHash(string? username, string? fileName, string? fileHash);
    void DeleteFile(string? username, string fileName);
}