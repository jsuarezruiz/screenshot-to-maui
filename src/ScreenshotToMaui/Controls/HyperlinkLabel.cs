namespace ScreenshotToMaui.Controls
{
    public class HyperlinkLabel : Label
    {
        public HyperlinkLabel()
        {
            TextDecorations = TextDecorations.Underline;
            TextColor = Colors.LightBlue;

            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await Launcher.OpenAsync(Url))
            });
        }

        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkLabel));
       
        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }
    }
}