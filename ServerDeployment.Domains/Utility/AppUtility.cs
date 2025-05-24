using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ServerDeployment.Domains.Utility
{
    public static class AppUtility
    {
        public static string ConnectionString { get; set; }

        public static T MapObject<T>(object source)
        {
            try
            {
                T destination = Activator.CreateInstance<T>();
                var sourceProperties = source.GetType().GetProperties();
                var destinationProperties = destination.GetType().GetProperties();
                foreach (var property in sourceProperties)
                {
                    var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == property.Name);
                    if (destinationProperty == null) continue;
                    var value = property.GetValue(source);

                    if (value == null) continue;
                    destinationProperty.SetValue(destination, property.GetValue(source));
                }

                return destination;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw null;
            }
        }

        #region Has no string 

        public static bool HasNoStr(string strVal)
        {
            return string.IsNullOrEmpty(strVal) || string.IsNullOrWhiteSpace(strVal);
        }

        public static bool HasAnyStr(string strVal)
        {
            return !HasNoStr(strVal) && strVal.Length > 0;
        }

        #endregion


        #region Enums

        public static string GetEnumDescriptionByEnumValue<T>(int enumValue)
        {
            if (!Enum.IsDefined(typeof(T), enumValue))
                return string.Empty;
            var enumType = typeof(T);
            var memberInfos = enumType.GetMember(Enum.GetName(enumType, enumValue));
            if (memberInfos.Length <= 0) return string.Empty;
            var memberInfo = memberInfos[0];
            var attributes = memberInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            return attributes.Length > 0 ? ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description : string.Empty;

        }

        public static string GetEnumDescriptionByEnumName<T>(string enumValue)
        {
            if (!Enum.IsDefined(typeof(T), enumValue)) return string.Empty;
            var enumType = typeof(T);
            var memberInfos = enumType.GetMember(enumValue);
            if (memberInfos.Length <= 0) return string.Empty;
            var memberInfo = memberInfos[0];
            var attributes = memberInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            return attributes.Length > 0 ? ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description : string.Empty;
        }

        #endregion


        public static DateTime? GetUtcDateTime(DateTime? dateTime)
        {
            if (dateTime == DateTime.MinValue) return null;
            return dateTime;
            var dateTimeType = dateTime.Value.Date;
            var data = DateTime.SpecifyKind(dateTimeType, DateTimeKind.Utc);
            return data;
        }

        // trim all string and double space using regEX
        public static string TrimStr(string str)
        {
            if (HasNoStr(str)) return string.Empty;
            var regex = new Regex(@"\s+");
            var data = regex.Replace(str.Trim(), " ");
            return data;
        }

        // trim all string and double space using regEX
        public static string TrimAllSpaceStr(string str)
        {
            if (HasNoStr(str)) return string.Empty;
            // TrimAllSpaceStr, -, & , ., /, \, (, )
            var regex = new Regex(@"[\s\-\&\.\,\/\\\(\)]+");
            var data = regex.Replace(str.Trim(), "");
            return data;
        }

        // check gender value and return f/m/-
        // check gender value and return f/m
        public static string GetGenderValue(string val)
        {
            if (HasNoStr(val)) return "";
            val = TrimStr(val).ToLowerInvariant();
            switch (val)
            {
                case "F":
                case "f":
                case "female":
                case "Female":
                    return "F";
                case "M":
                case "m":
                case "male":
                case "Male":

                    return "M";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Check if string is null or empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool HasNoStrValue(string? str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Check if string has value
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool HasStrValue(string? str)
        {
            return !HasNoStrValue(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static string GetDirectorySize(string folderPath)
        {
            decimal size = 0;

            // Get all files in the folder and add their sizes
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    FileInfo fi = new FileInfo(file);
                    size += fi.Length;
                }
                catch
                {
                    // Handle files that might be inaccessible (e.g., permissions)
                }
            }

            var dSize = FormatSize(size);
            return dSize;
        }

        private static string FormatSize(decimal bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            decimal len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        #region Database Helper

        public static T CreateObjectFromRow<T>(DataRow row)
        {
            T item = Activator.CreateInstance<T>();
            SetValueFromRow(item, row);
            return item;
        }

        private static void SetValueFromRow<T>(T item, DataRow row)
        {
            foreach (DataColumn c in row.Table.Columns)
            {
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }

        public static string GetSelectSql<T>(long id)
        {
            Type type = Activator.CreateInstance<T>().GetType();
            string primaryKey = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (string.IsNullOrEmpty(primaryKey) && !prop.GetMethod.IsVirtual)
                {
                    var attribute = Attribute.GetCustomAttribute(prop, typeof(KeyAttribute)) as KeyAttribute;
                    if (attribute != null)
                    {
                        primaryKey = prop.Name;
                        break;
                    }
                }
            }

            return string.Format("SELECT * FROM {0} WHERE {1} = {2}", type.Name, primaryKey, id);
        }

        #endregion
    }
}