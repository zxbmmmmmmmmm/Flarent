using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarentApp.Views.DetailPages;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace FlarentApp.ViewModels.Dialogs
{
    public sealed partial class ReadModePageVM:ObservableObject
    {
        [ObservableProperty]
        private Discussion discussion = new();
        [ObservableProperty]
        private User user = new();
        [ObservableProperty]
        private ObservableCollection<object> posts = new();
        public List<int> PostIds = new();
        public string LinkNext;

        /*[RelayCommand]
        public async Task GetPosts()
        {
            //if(Discussion == null)
                //Discussion = await FlarumApiProviders.GetDiscussion(2724,0,Flarent.Settings.Forum,Flarent.Settings.Token);
            foreach (var item in Discussion.Posts)//获取帖子列表
            {
                PostIds.Add((int)item.Id);
            }
            var tup = await FlarumApiProviders.GetPostsWithLink($"https://community.wvbtech.com/api/posts?filter[discussion]={Discussion.Id}?filter[author]={User.UserName}", Flarent.Settings.Token);
            var _posts = tup.Item1;
            LinkNext = tup.Item2;
            foreach(var post in _posts)
            {
                Posts.Add(post);
            }
        }*/
        [RelayCommand]
        public async Task LoadAll()
        {
            while(LinkNext != "")
            {
                try
                {
                    await LoadMore();
                }
                catch
                {

                }
            }
        }
        [RelayCommand]
        public async Task LoadMore()
        {

            var tup = new Tuple<List<Post>,string>(null,null);
            User = Discussion.User;
            if (LinkNext == null)
                tup = await FlarumApiProviders.GetPostsWithLink($"https://{Flarent.Settings.Forum}/api/posts?filter[discussion]={Discussion.Id}&filter[author]={User.UserName}", Flarent.Settings.Token);
            else if (LinkNext == "")
                return;
            else
                tup = await FlarumApiProviders.GetPostsWithLink(LinkNext, Flarent.Settings.Token);
            var posts = tup.Item1;
            LinkNext = tup.Item2;

            if(Discussion.Posts.Count == 0)
            {
                Discussion = await FlarumApiProviders.GetDiscussion(Discussion.Id.Value, 0, Flarent.Settings.Forum, Flarent.Settings.Token);
            }

            var allPostsList = new List<int>();
            foreach (var item in Discussion.Posts)
            {
                allPostsList.Add(item.Id.Value);
            }
            try
            {
                foreach (var post in posts)
                {
                    var index = posts.IndexOf(post);
                    Posts.Add(post);
                    if (index + 1 == posts.Count) return;

                    if (posts[index + 1].Number - 1 != post.Number)//中间有评论
                    {
                        var comments = allPostsList.GetRange(post.Number.Value, posts[index + 1].Number.Value - post.Number.Value - 1);
                        Posts.Add(new CommentList { Comments = comments, ViewModel = this });
                    }

                }
            }
            finally
            {
                if (allPostsList.Count != posts[posts.Count - 1].Number.Value&&LinkNext == "")//添加剩余的评论
                {
                    var num = posts[posts.Count - 1].Number.Value;
                    var comments = allPostsList.GetRange(posts[posts.Count - 1].Number.Value, allPostsList.Count - num);
                    Posts.Add(new CommentList { Comments = comments, ViewModel = this });
                }
            }
            //for (int i = 0; i<posts.Count; i++)
            //{
            //    if (i != 0)
            //    {
            //        if (posts[i - 1].Number + 1 != posts[i].Number)
            //        {

            //            var comments = allPostsList.GetRange(posts[i - 1].Number.Value, posts[i].Number.Value - posts[i - 1].Number.Value - 1);
            //            Posts.Add(new CommentList { Comments = comments ,ViewModel = this}) ;
            //        }
            //    }
            //    Posts.Add(posts[i]);
            //}


        }
        [RelayCommand]
        public void ViewComments(List<int> posts)
        {
            NavigationService.OpenInRightPane(typeof(PostDetailPage), posts);

        }
    }
}
