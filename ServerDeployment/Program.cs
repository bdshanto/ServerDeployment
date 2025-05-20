using System.Reflection;
using ARAKDataSetup.Domains.Utility;
using ServerDeployment.Console.Forms; 

namespace ServerDeployment.Console
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var conString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

            AppUtility.ConnectionString = conString;

            ApplicationConfiguration.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false); 

            Infragistics.Win.AppStyling.StyleManager.Load(Utilities.GetEmbeddedResourceStream("ServerDeployment.StyleLibraries.FlatNature.isl"));

            Application.Run(new DeploymentForm());
        }
    }
}