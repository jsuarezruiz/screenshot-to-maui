namespace ScreenshotToMaui.Extensions
{
    public static class TaskExtensions
    {
        public static void FireAndForget(this Task task)
        {
            if (task == null)
            {
                return;
            }
            _ = Task.Run(() => task);
        }
    }
}