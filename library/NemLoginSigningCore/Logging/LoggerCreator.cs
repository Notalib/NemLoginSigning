using Microsoft.Extensions.Logging;

namespace NemLoginSigningCore.Logging
{
    /// <summary>
    /// Loggin class that provices methods for getting both generic and
    /// specific category named loggers
    /// </summary>
    public static class LoggerCreator
    {
        public static ILoggerFactory LoggerFactory { get; set; }

        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

        public static ILogger CreateLogger(string name) => LoggerFactory.CreateLogger(name);
    }
}
