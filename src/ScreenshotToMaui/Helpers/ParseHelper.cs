using System.Text.RegularExpressions;

namespace ScreenshotToMaui.Helpers
{
    public static class ParseHelper
    {
        public static string Sanitize(string xaml)
        {
            // Check if has ContentPage
            var contentPageIndex = xaml.IndexOf("ContentPage");
            bool hasContentPage = contentPageIndex != -1;

            if (hasContentPage)
            {
                // Remove ContentPage opening
                var openingIndex = xaml.IndexOf('>');
                xaml = xaml.Remove(0, openingIndex + 1);

                // Remove ContentPage closing
                var closingIndex = xaml.IndexOf("</ContentPage>");
                xaml = xaml.Remove(closingIndex);
            }

            xaml = xaml.Replace("&", "&amp;");

            return xaml;
        }

        public static List<string> GetImages(string xaml)
        {
            const string image = "https://placehold.co/100x100?description=";

            List<string> images = new List<string>();

            string[] lines = xaml.Split("\n");

            foreach (var line in lines)
            {
                if (line.Contains(image))
                {
                    Match url = Regex.Match(line, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                    images.Add(url.Value);
                }
            }

            return images;
        }

        public static string GetImageDescription(string image)
        {
            const string placeholder = "https://placehold.co/100x100?description=";
            string description = image.Remove(0, placeholder.Length);
            description = description.Replace("%20", " ");

            return description;
        }

        public static string UpdateImages(string xaml, Dictionary<string, string> images)
        {
            string result = xaml;

            foreach (var replacement in images)
            {
                result = result.Replace(replacement.Key, replacement.Value);
            }

            return result;
        }
    }
}
