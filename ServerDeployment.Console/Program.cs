using System.Reflection;
using ServerDeployment.Console.Forms.AppForms;
using ServerDeployment.Domains.Utility;
using ServerDeployment.Forms;

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

            Infragistics.Win.AppStyling.StyleManager.Load(Utilities.GetEmbeddedResourceStream("ServerDeployment.Console.StyleLibraries.FlatNature.isl"));

            Application.Run(new DeploymentForm());
           // Application.Run(new MainForm());
        }
    }
}