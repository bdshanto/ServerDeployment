using System;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Diagnostics;

namespace ServerDeployment.Console
{
    internal static class Utilities
    {
        #region GetImageFromResource
        /// <summary>
        /// Returns an Image from the EmbeddedResources.
        /// </summary>
        /// <param name="imgName"></param>
        /// <returns></returns>
        internal static Bitmap GetImageFromResource(string imgName)
        {
            Bitmap bmp = null;
            Type thisType = typeof(Utilities);
            string fullResourceName = string.Format("OutlookCRM.Images.{0}", imgName);
            using (Stream stream = thisType.Assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream != null)
                {
                    bmp = (Bitmap)Image.FromStream(stream);
                }
            }
            return bmp;
        }
        #endregion // GetImageFromResource

        #region GetEmbeddedResourceStream

        /// <summary>
        /// Gets the embedded resource stream.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        internal static Stream GetEmbeddedResourceStream(string resourceName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            Debug.Assert(stream != null, "Unable to locate embedded resource.", "Resource name: {0}", resourceName);
            return stream;
        }

        #endregion //GetEmbeddedResourceStream

     

        #region GetAssemblyAttribute

        /// <summary>
        /// Retrieves the string representation of the request AssemblyAttribute from the project's AssemblyInfo.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static string GetAssemblyAttribute<T>(Func<T, string> value)
            where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
        }

        #endregion // GetAssemblyAttribute

        #region SetUIFriendlyString
        internal static void SetUIFriendlyString(Infragistics.Win.UltraWinGrid.ColumnHeader columnHeader)
        {
            columnHeader.Caption = System.Text.RegularExpressions.Regex.Replace(columnHeader.Caption, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
        #endregion // SetUIFriendlyString

    }
}
