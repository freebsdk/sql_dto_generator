using System.Runtime.CompilerServices;

namespace sqlc;



public class AppException : Exception
{
    public AppException(ErrorCodes errorCode,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0,
        [CallerMemberName] string callerMemberName = "")
    : base(errorCode.ToString())
    {
    }
}