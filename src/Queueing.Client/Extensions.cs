using Microsoft.Extensions.Logging;

namespace Lib.Queueing.Client
{
    internal static class Extensions
    {
        public static void LogExceptionScope(this ILogger logger)
        {
            logger.LogError("log exception scope variables, check the next log for exception details");
        }
    }
}