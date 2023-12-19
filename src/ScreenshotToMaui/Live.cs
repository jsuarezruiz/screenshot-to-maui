#nullable disable
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ScreenshotToMaui
{
    public static class Live
    {
        public static readonly Regex Regex = new Regex("x:Class=\"([^\"]+)\"");

        public static Page GetPage(Page page, string fullTypeName)
        {
            if (page is null)
                return null;

            if (page.GetType().FullName == fullTypeName)
                return page;

            return null;
        }

        public static Task UpdatePageFromXamlAsync(Page page, string xaml)
        {
            var taskCompletionSource = new TaskCompletionSource<Page>();

#pragma warning disable CS0618 // Type or member is obsolete
            Device.BeginInvokeOnMainThread(() =>
            {
                var bindingContext = page.BindingContext;
                try
                {
                    Debug.WriteLine("Loading XAML...");
                    LoadXaml(page, xaml);
                    page.ForceLayout(); // Update
                    taskCompletionSource.SetResult(page);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    taskCompletionSource.SetException(exception);
                }
                finally
                {
                    page.BindingContext = bindingContext;
                    Debug.WriteLine("XAML Loaded!");
                }
            });
#pragma warning restore CS0618 // Type or member is obsolete

            return taskCompletionSource.Task;
        }

        public static string GetXamlException(Exception exception)
        {
            XNamespace xmlns = "http://schemas.microsoft.com/dotnet/2021/maui";

            var errorPage = new XDocument(
                new XElement(xmlns + "ContentPage",
                new XElement(xmlns + "ScrollView",
                new XElement(xmlns + "StackLayout",
                new XAttribute("Margin", "12, 0"),
                new XElement(xmlns + "Label",
                    new XAttribute("Text", "Error"),
                    new XAttribute("TextColor", "Red"),
                    new XAttribute("FontSize", "Medium")
                ),
                new XElement(xmlns + "Label",
                    new XAttribute("Text", exception.Message),
                    new XAttribute("TextColor", "Red"),
                    new XAttribute("LineBreakMode", "CharacterWrap"),
                    new XAttribute("FontSize", "Small")
                ))))).ToString();

            return errorPage;
        }

        public static void LoadXaml(BindableObject view, string xaml)
        {
            var xamlAssembly = Assembly.Load(new AssemblyName("Microsoft.Maui.Controls.Xaml"));
            var xamlLoader = xamlAssembly.GetType("Microsoft.Maui.Controls.Xaml.XamlLoader");
            var load = xamlLoader.GetRuntimeMethod("Load", new[] { typeof(BindableObject), typeof(string) });

            try
            {
                load.Invoke(null, new object[] { view, xaml });
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException; // To show to the user in the ErrorPage!
            }
        }
    }
}
