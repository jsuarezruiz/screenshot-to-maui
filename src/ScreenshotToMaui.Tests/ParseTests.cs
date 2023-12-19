using ScreenshotToMaui.Helpers;

namespace ScreenshotToMaui.Tests
{
    public class ParseTests
    {
        [Theory, TextFileData("sample1.txt")]
        public void BasicSanitizeTest(string xaml)
        {
            var result = ParseHelper.Sanitize(xaml);
            Assert.NotNull(result);
        }

        [Theory, TextFileData("sample1.txt")]
        public void GetImagesCountTest(string xaml)
        {
            var images = ParseHelper.GetImages(xaml);
            Assert.Equal(10, images.Count);
        }

        [Theory]
        [InlineData("https://placehold.co/100x100?description=Green%20Plant%20Icon")]
        public void GetImageDescriptionTest(string image)
        {
            var description = ParseHelper.GetImageDescription(image);
            Assert.Equal("Green Plant Icon", description);
        }

        [Theory, TextFileData("sample1.txt")]
        public void UpdateImagesTest(string xaml)
        {
            var newUrl = "https://test.com";

            Dictionary<string, string> images = new Dictionary<string, string>
            {
                { "https://placehold.co/100x100?description=Green%20Plant%20Icon", newUrl }
            };

            var result = ParseHelper.UpdateImages(xaml, images);
            Assert.Contains(newUrl, result);
        }
    }
}