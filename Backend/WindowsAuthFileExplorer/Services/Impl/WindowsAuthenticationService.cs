using System.Runtime.InteropServices;
using WindowsAuthFileExplorer.Models;

namespace WindowsAuthFileExplorer.Services.Impl;

public class WindowsAuthenticationService
{
    private UserCredentialsModel UserRequest { get; set; }

    #region DllImportLogOnUser

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    private static extern bool LogonUser(string? username, string? domain, string? password, int logType, int logpv,
        ref IntPtr intPtr);

    #endregion

    public WindowsAuthenticationService(UserCredentialsModel userRequest)
    {
        UserRequest = userRequest;
    }

    public bool Authenticate()
    {
        var ip = IntPtr.Zero;
        var isAuthenticated = LogonUser(UserRequest.Username, UserRequest.Domain, UserRequest.Password, 2, 0, ref ip);

        return isAuthenticated;
    }
}