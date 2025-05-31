using System.Text;


namespace ServerDeployment.Applications.Helpers
{
    public static class SLogger
    {
        private static readonly object Lock = new object(); // Prevents race conditions in multi-threaded scenarios. 
        private const string LogFolderText = "sLogs";

        public static void WriteLog(string logText) => WriteLog(logText, LogFolderText);


        /// <summary>
        /// FolderNames will create into the Logs folder
        /// </summary>
        /// <param name="logText"></param>
        /// <param name="folderNames"></param>
        public static void WriteLog(string logText, params string[] folderNames)
        {


            if (string.IsNullOrWhiteSpace(logText)) return;

            // Ensure "sLog" is at the start of the folder path
            var pathList = new List<string> { LogFolderText }; // Start with "sLog"

            // Add all other folder names, but avoid duplicates of "sLog"
            foreach (var folderName in folderNames)
            {
                if (!folderName.Equals(LogFolderText, StringComparison.OrdinalIgnoreCase))
                {
                    pathList.Add(folderName);
                }
            }

            // Combine the folder names into a single folder path
            string folderPath = Path.Combine(pathList.ToArray());


            try
            {
                string logFilePath = GetLogFilePath(folderPath);
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} ------: {logText}{Environment.NewLine}";

                lock (Lock) // Ensures thread safety
                {
                    File.AppendAllText(logFilePath, logEntry);
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        public static void WriteLog(Exception ex) => WriteLog(ex, LogFolderText);

        /// <summary>
        /// FolderNames will create into the Logs folder
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="folderNames"></param>
        public static void WriteLog(Exception ex, params string[] folderNames)
        {
            if (ex == null) return;

            try
            {
                StringBuilder logBuilder = new StringBuilder();
                logBuilder.AppendLine($"Message: {ex.Message}{Environment.NewLine}");
                logBuilder.AppendLine($"StackTrace: {ex.StackTrace}{Environment.NewLine}");
                logBuilder.AppendLine($"Source: {ex.Source}{Environment.NewLine}");
                logBuilder.AppendLine($"TargetSite: {ex.TargetSite} {Environment.NewLine}");

                if (ex.InnerException != null)
                {
                    logBuilder.AppendLine($"InnerException: {ex.InnerException.Message} {Environment.NewLine}");
                    logBuilder.AppendLine($"InnerStackTrace: {ex.InnerException.StackTrace} {Environment.NewLine}");
                }
                logBuilder.AppendLine($"##################################################################################");

                WriteLog(logBuilder.ToString(), folderNames);
            }
            catch (Exception e)
            {
                WriteLog(e, folderNames);
            }
        }

        private static void HandleError(Exception ex)
        {
            // Consider adding logic to handle errors when logging fails.
            // For instance, send an email alert or write to a separate fallback log file.
            Console.Error.WriteLine($"Error while logging: {ex.Message}");
        }


        private static string GetLogFilePath(string folderName)
        {
            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
            string logFolderPath = Path.Combine(appRoot, folderName);

            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            return Path.Combine(logFolderPath, $"Log_{DateTime.Now:yyyyMMdd}.txt");
        }
    }
}