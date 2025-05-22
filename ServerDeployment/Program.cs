using System.Reflection;
using ServerDeployment.Console;
using ServerDeployment.Console.Forms;
using ServerDeployment.Domains.Utility;

namespace ServerDeployment
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