using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

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

        /// <returns>A dynamic object with only the filled properties of an object</returns>
        public static object ConvertToObjectWithoutPropertiesWithNullValues<T>(this T objectToTransform)
        {
            var type = objectToTransform.GetType();
            var returnClass = new ExpandoObject() as IDictionary<string, object>;
            foreach (var propertyInfo in type.GetProperties())
            {
                var value = propertyInfo.GetValue(objectToTransform);
                var valueIsAString = value is string;
                var valueIsNullOrWhiteSpace = value == null ? true : string.IsNullOrWhiteSpace(value.ToString());
                if (valueIsAString && !valueIsNullOrWhiteSpace && value != null)
                {
                    returnClass.Add(propertyInfo.Name, value);
                }

                //var valueIsNotAString = !(value is string && !string.IsNullOrWhiteSpace(value.ToString()));
                //if (valueIsNotAString && value != null)
                //{
                //    returnClass.Add(propertyInfo.Name, value);
                //}
            }

            return returnClass;
        }

        /// <summary>
        /// Usage: public List<string> Index(IFormFile file) => file.ReadAsList();
        /// https://stackoverflow.com/questions/40045147/how-to-read-into-memory-the-lines-of-a-text-file-from-an-iformfile-in-asp-net-co
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<string> ReadAsList(this IFormFile file)
        {
            List<string> lines = new List<string>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    lines.Add(reader.ReadLine());
            }
            return lines;
        }

        /// <summary>
        /// Usage: public List<string> Index(IFormFile file) => file.ReadAsList();
        /// https://stackoverflow.com/questions/40045147/how-to-read-into-memory-the-lines-of-a-text-file-from-an-iformfile-in-asp-net-co
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<string> ReadAsStringAsync(this IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync());
            }
            return result.ToString();
        }

        ///// <summary>
        ///// Usage: public Task<List<string>> Index(IFormFile file, [FromServices] ObjectPool<StringBuilder> pool) => file.ReadAsListAsync(pool);
        ///// https://stackoverflow.com/questions/40045147/how-to-read-into-memory-the-lines-of-a-text-file-from-an-iformfile-in-asp-net-co
        ///// </summary>
        ///// <param name="file"></param>
        ///// <param name="pool"></param>
        ///// <returns></returns>
        //public static async Task<string> ReadAsStringAsync(this IFormFile file, Object<StringBuilder> pool)
        //{
        //    var builder = pool.Get();
        //    try
        //    {
        //        using var reader = new StreamReader(file.OpenReadStream());
        //        while (reader.Peek() >= 0)
        //        {
        //            builder.AppendLine(await reader.ReadLineAsync());
        //        }
        //        return builder.ToString();
        //    }
        //    finally
        //    {
        //        pool.Return(builder);
        //    }
        //}

        /// <summary>
        /// https://stackoverflow.com/questions/40045147/how-to-read-into-memory-the-lines-of-a-text-file-from-an-iformfile-in-asp-net-co
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<List<string>> ReadFromFileAsync(IFormFile file)
        {
            List<string> result = new List<string>();
            if (file == null || file.Length == 0)
            {
                return await Task.FromResult(result);
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                result.Add(await reader.ReadLineAsync());
            }
            return result;
        }
    }

    public class DynamicObject
    {
        public dynamic Instance = new ExpandoObject();

        public void AddProperty(string name, object value)
        {
            ((IDictionary<string, object>)this.Instance).Add(name, value);
        }

        public dynamic GetProperty(string name)
        {
            if (((IDictionary<string, object>)this.Instance).ContainsKey(name))
                return ((IDictionary<string, object>)this.Instance)[name];
            else
                return null;
        }
    }
}
