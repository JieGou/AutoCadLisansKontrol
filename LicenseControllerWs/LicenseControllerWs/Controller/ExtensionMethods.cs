using System;

namespace LicenseControllerWs.Model
{
    public static class ExtensionMethods
    {
        public static string UppercaseFirstLetter(this string value)
        {
            //
            // Uppercase the first letter in the string.
            //
            if (value.Length > 0)
            {
                char[] array = value.ToCharArray();
                array[0] = char.ToUpper(array[0]);
                return new string(array);
            }
            return value;
        }

        public static string ReplaceEncodedHtmlCharacter(this string xml)
        {

            if (xml.Length > 0)
            {
                xml = xml.Replace("&lt;", "<").Replace("&gt;", ">");
            }
            return xml;
        }

        public static string SubstringTime(this string time)
        {
            try
            {
                time = time.Substring(0, 5);
            }
            catch
            {
                return time;
            }
            return time;
        }
    }
}