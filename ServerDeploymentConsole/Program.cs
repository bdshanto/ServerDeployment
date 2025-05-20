using ARAKDataSetup.Domains.Utility;
using ServerDeployment.Console.Forms;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace ServerDeployment.Console
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

            AppUtility.ConnectionString = conString;

            ApplicationConfiguration.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            
            const string themeName = "Theme-01.isl";

            var outputStyleFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StyleLibraries", themeName);
            if (!File.Exists(outputStyleFile))
            {
                string projectBaseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
                var sourceStyleFile = Path.Combine(projectBaseDir, "StyleLibraries", themeName);

                if (File.Exists(sourceStyleFile))
                {
                    var targetDir = Path.GetDirectoryName(outputStyleFile);
                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                    }
                     
                    File.Copy(sourceStyleFile, outputStyleFile, overwrite: true);
                }
            }

            Infragistics.Win.AppStyling.StyleManager.Load(outputStyleFile);
            
           // Infragistics.Win.AppStyling.StyleManager.Load(Utilities.GetEmbeddedResourceStream("ServerDeploymentConsole.StyleLibraries.Theme-01.isl"));

            Application.Run(new DeploymentForm());
        }
    }
}