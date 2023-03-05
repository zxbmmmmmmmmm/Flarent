using CommunityToolkit.Common;
using FlarumApi;
using FlarumApi.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Helpers;
using Windows.ApplicationModel.Background;

namespace Tasks
{
    public sealed class DynamicNotifyTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            //
            // Call asynchronous method(s) using the await keyword.
            //
            var data = await FlarumApiProviders.GetDiscussions(null, $"https://{SettingsHelper.Forum}/api/discussions?sort=-createdAt", null, null);
            var discussions = data.Item1;
            if (data.Item1 == null)
                deferral.Complete();
            var discussion = discussions[0];
            var content = discussion.FirstPost.ContentHtml.DecodeHtml();
            TilePusher.UpdateDiscussion(discussion.Title, content);

            if (content.Length >= 40)
            {
                content = content.Remove(40);
                content = content.Insert(content.Length, "...");
            }
            var image = HtmlHelper.GetFirstImage(discussion.FirstPost.ContentHtml);

            var toast = new ToastContentBuilder()
                .AddText(discussion.Title)
                .AddText(content)
                .AddAttributionText($"{discussion.User.DisplayName} 发布于 {DateHelper.FriendFormat((DateTime)discussion.CreatedAt)}")
                .AddAppLogoOverride(new Uri(discussion.User.AvatarUrl));
            if (image != null)
                toast.AddHeroImage(new Uri(image));
            toast.Show();
            //new NotificationPusher().PushDiscussion();

            //
            // Once the asynchronous method(s) are done, close the deferral.
            //
            deferral.Complete();
        }
    }
}
