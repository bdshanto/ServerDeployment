using ARAKDataSetup.Domains.Utility;
using ServerDeployment.Console.Forms;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace ServerDeployment.Console
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

            AppUtility.ConnectionString = conString;

            ApplicationConfiguration.Initialize();
            
             Application.Run(new MainForm());
        }
    }
}