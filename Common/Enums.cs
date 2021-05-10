using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangledServices.ServicePortal.API.Common
{
    public static class Enums
    {
        [Description("Lookup items")]
        public enum LookupItems
        {
            [Description("Subscription renewal timeframes")]
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

        [Description("Subscription renewal timeframes")]
        public enum SubscriptionRenewalTimeframes
        {
            [Description("Monthly Recurring")]
            MonthlyRecurring,

            [Description("Yearly Recurring")]
            YearlyRecurring
        }

        [Description("Database platforms")]
        public enum DatabasePlatforms
        {
            [Description("Azure Cosmos DB")]
            azureCosmosDb,

            [Description("AWS Dynamo DB")]
            awsDynamoDb
        }

        public static class Role
        {
            public const string SystemRoot = "System Root";
            public const string AdminRoot = "Admin Root";
            public const string SystemUser = "System User";
            public const string AdminUser = "Admin User";
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
            if (the_string == null || the_string.Length < 2) return the_string;

            if (removeUnderscores) the_string = the_string.Replace("_", " ");

            // Split the string into words.
            string[] words = the_string.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            StringBuilder result = new StringBuilder(string.Empty);
            for (int i = 0; i < words.Length; i++)
            {
                result.Append(words[i].Substring(0, 1).ToUpper() + words[i].Substring(1));
            }

            return result.ToString();
        }

        // Convert string to title case.
        public static string ToTitleCase(this string the_string, bool removeUnderscores = true)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null || the_string.Length < 2) return the_string;

            if (removeUnderscores) the_string = the_string.Replace("_", " ");

            // Split the string into words.
            string[] words = the_string.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(the_string.ToLower());
        }
    }
}
