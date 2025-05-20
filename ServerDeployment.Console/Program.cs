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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Infragistics.Win.AppStyling.StyleManager.Load("Aero.isl");
            Application.Run(new MainForm());
        }
    }
}