using CommunityToolkit.Common;
using FlarumApi;
using FlarumApi.Models;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
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
            if (!SettingsHelper.IsNotifyEnabled)
            {
                deferral.Complete();
                return;
            }
            var data = await FlarumApiProviders.GetDiscussions(null, $"https://{SettingsHelper.Forum}/api/discussions?sort=-createdAt", null, SettingsHelper.Token);
            var discussions = data.Item1;
            if (data.Item1 == null)
                deferral.Complete();
            var discussion = discussions.First();

            var now = DateTime.Now;
            var dt = discussion.CreatedAt.Value;
            TimeSpan ts = now - dt;
            if (ts.TotalSeconds >= 900)//超过15分钟则不推送
            {
                deferral.Complete();
                return;
            }
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
                .AddAppLogoOverride(new Uri(discussion.User.AvatarUrl),ToastGenericAppLogoCrop.Circle)
                .AddArgument("discussion",discussion.Id.Value);
                
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
