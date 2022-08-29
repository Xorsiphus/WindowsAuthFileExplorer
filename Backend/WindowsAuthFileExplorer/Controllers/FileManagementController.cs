using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WindowsAuthFileExplorer.Services;

namespace WindowsAuthFileExplorer.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[EnableCors]
public class FileManagementController : ControllerBase
{
    private readonly ILogger<FileManagementController> _logger;
    private readonly IFileManagementService _fileService;

    public FileManagementController(ILogger<FileManagementController> logger, IFileManagementService fileService)
    {
        _logger = logger;
        _fileService = fileService;
    }

    [HttpGet("GetUserFiles")]
    public IActionResult GetUserFiles()
    {
        var files = _fileService.GetUserFiles(User.Identity?.Name);
        return Ok(files);
    }

    [HttpGet("GetFile/{fileName}")]
    public IActionResult? GetFile(string fileName)
    {
        var file = _fileService.GetFile(User.Identity?.Name, fileName);
        return file ?? null;
    }

    [HttpGet("GetFileExistence/{fileName}")]
    public IActionResult GetFileExistence(string fileName, [FromQuery] string fileHash)
    {
        var file = _fileService.CheckFileWithHash(User.Identity?.Name, fileName, fileHash);
        return Ok(file);
    }

    [HttpPost("AddFile")]
    public async Task<IActionResult> AddFile([FromForm] IFormFile? uploadedFile)
    {
        var files = await _fileService.SaveFile(User.Identity?.Name, uploadedFile);
        return Ok(files);
    }
    
    [HttpDelete("DeleteFile/{fileName}")]
    public IActionResult DeleteFile(string fileName)
    {
        _fileService.DeleteFile(User.Identity?.Name, fileName);
        return Ok();
    }
}