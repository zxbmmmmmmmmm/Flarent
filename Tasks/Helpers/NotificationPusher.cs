using FlarumApi;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Common.Extensions;
using CommunityToolkit.Common;

namespace Tasks.Helpers
{
    public sealed class NotificationPusher
    {
        public void PushSampleNotification()
        {
            new ToastContentBuilder()
                .SetToastScenario(ToastScenario.Reminder)
                .AddArgument("action", "viewEvent")
                .AddArgument("eventId", 1983)
                .AddText($"Notification")
                .AddText("Conf Room 2001 / Building 135")
                .AddText("10:00 AM - 10:30 AM")
                .AddComboBox("snoozeTime", "15", ("1", "1 minute"),
                                                 ("15", "15 minutes"),
                                                 ("60", "1 hour"),
                                                 ("240", "4 hours"),
                                                 ("1440", "1 day"))
                .AddButton(new ToastButton()
                    .SetSnoozeActivation("snoozeTime"))
                .AddButton(new ToastButton()
                    .SetDismissActivation())
                .Show();
        }
        public async void PushDiscussion()
        {
            var data = await FlarumApiProviders.GetDiscussions(null, null, "community.wvbtech.com", null);
            var discussions = data.Item1;
            new ToastContentBuilder()
                .AddText(discussions[0].Title)
                .AddText(discussions[0].FirstPost.ContentHtml.DecodeHtml())
                .Show();

        }
    }
}
