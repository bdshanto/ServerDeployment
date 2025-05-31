using ServerDeployment.Console.Forms.AppForms;
using ServerDeployment.Domains.Utility;

namespace ServerDeployment.Console
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        { 
            ApplicationConfiguration.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Infragistics.Win.AppStyling.StyleManager.Load(Utilities.GetEmbeddedResourceStream("ServerDeployment.Console.StyleLibraries.FlatNature.isl"));

            Application.Run(new DeploymentForm()); 
        }
    }
}