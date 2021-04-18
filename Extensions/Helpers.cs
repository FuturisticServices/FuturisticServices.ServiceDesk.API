using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace TangledServices.ServicePortal.API.Extensions
{
    public static class Helpers
    {
        // Convert the string to Pascal case.
        public static string ToPascalCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null) return the_string;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }

        // Convert the string to camel case.
        public static string ToCamelCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null || the_string.Length < 2)
                return the_string;

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }

        // Capitalize the first character and add a space before
        // each capitalized letter (except the first character).
        public static string ToProperCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null) return the_string;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Start with the first character.
            string result = the_string.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (int i = 1; i < the_string.Length; i++)
            {
                if (char.IsUpper(the_string[i])) result += " ";
                result += the_string[i];
            }

            return result;
        }

        public static string GetRandomNumber()
        {
            Random randomGenerator = new Random((int)DateTime.Now.Ticks);
            int randomNumber = randomGenerator.Next(100000, 999999);
            return randomNumber.ToString("D6");
        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public static bool IsCreditCardNumberValid(string creditCardNumber)
        {
            //Strip any non-numeric values
            creditCardNumber = Regex.Replace(creditCardNumber, @"[^\d]", "");

            //Build your Regular Expression
            Regex expression = new Regex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$");

            //Return if it was a match or not
            return expression.IsMatch(creditCardNumber);
        }
    }
}
