using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;

namespace StravaUploader
{
    public class NotificationService : INotificationService, IDisposable
    {
        public NotificationService()
        {
            ToastNotificationManagerCompat.OnActivated += Toast_OnActivated;
        }

        public void Dispose()
        {
            ToastNotificationManagerCompat.Uninstall();
        }

        public void Show(string message, string? url = null)
        {
            var builder = new ToastContentBuilder().AddText(message);

            if (url != null)
            {
                builder.AddArgument("url", url);
            }

            builder.Show();
        }

        private void Toast_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            ToastArguments args = ToastArguments.Parse(e.Argument);
            if (args.Contains("url"))
            {
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = args["url"]
                });
            }
        }
    }
}
