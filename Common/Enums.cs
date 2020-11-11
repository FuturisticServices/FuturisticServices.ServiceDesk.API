using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuturisticServices.ServiceDesk.API.Common
{
    public static class Enums
    {
        [Description("Lookup groups")]
        public enum LookupGroups
        {
            [Description("Suscripton renewal timeframes")]
            SubscriptionRenewalTimeframes,

            [Description("States")]
            States,

            [Description("Countries")]
            Countries,

            [Description("Address types")]
            AddressTypes,

            [Description("Phone number types")]
            PhoneNumberTypes,

            [Description("Email address types")]
            EmailAddressTypes,

            [Description("Website types")]
            WebsiteTypes,

            [Description("Departments")]
            Departments,

            [Description("Database platforms")]
            DatabasePlatforms,
        }

        [Description("Database platforms")]
        public enum DatabasePlatforms
        {
            [Description("Azure Cosmos DB")]
            azureCosmosDb,

            [Description("AWS Dynamo DB")]
            awsDynamoDb
        }

        public static string GetDescription<T>(this T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }

        public static T GetFromDescription<T>(string description) where T : Enum, IConvertible
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
        }

        // Convert string to camel case.
        public static string ToCamelCase(this string the_string, bool removeUnderscores = true)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null || the_string.Length < 2)
                return the_string;

            if (removeUnderscores)
                the_string = the_string.Replace("_", " ");

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            StringBuilder result = new StringBuilder(words[0].ToLower());
            for (int i = 1; i < words.Length; i++)
            {
                result.Append(words[i].Substring(0, 1).ToUpper() + words[i].Substring(1));
            }

            return result.ToString();
        }

        // Convert string to pascal case.
        public static string ToPascalCase(this string the_string, bool removeUnderscores = true)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null || the_string.Length < 2)
                return the_string;

            if (removeUnderscores)
                the_string = the_string.Replace("_", " ");

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            StringBuilder result = new StringBuilder(string.Empty);
            for (int i = 0; i < words.Length; i++)
            {
                result.Append(words[i].Substring(0, 1).ToUpper() + words[i].Substring(1));
            }

            return result.ToString();
        }
    }
}
