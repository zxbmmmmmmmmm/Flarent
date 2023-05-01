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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarentApp.ViewModels.Dialogs
{
    public sealed partial class ReadModePageVM:ObservableObject
    {
        [ObservableProperty]
        public Discussion discussion = new();
        [ObservableProperty]
        public User user = new();
        [ObservableProperty]
        public ObservableCollection<object> posts = new();
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
        public async Task LoadMore()
        {
            var tup = new Tuple<ObservableCollection<Post>,string>(null,null);
            User = Discussion.User;
            if (LinkNext == null)
                tup = await FlarumApiProviders.GetPostsWithLink($"https://{Flarent.Settings.Forum}/api/posts?filter[discussion]={Discussion.Id}&filter[author]={User.UserName}", Flarent.Settings.Token);
            else if (LinkNext == "")
                return;
            else
                tup = await FlarumApiProviders.GetPostsWithLink(LinkNext, Flarent.Settings.Token);
            var _posts = tup.Item1;
            LinkNext = tup.Item2;
            var allPostsList = new List<int>();
            foreach (var item in Discussion.Posts)
            {
                allPostsList.Add(item.Id.Value);
            }
            for (int i = 0; i<_posts.Count; i++)
            {
                if (i != 0)
                {
                    if (_posts[i - 1].Number + 1 != _posts[i].Number)
                    {

                        var comments = allPostsList.GetRange(_posts[i - 1].Number.Value, _posts[i].Number.Value - _posts[i - 1].Number.Value - 1);
                        Posts.Add(new CommentList { Comments = comments ,ViewModel = this}) ;
                    }
                }
                Posts.Add(_posts[i]);
            }
            if(_posts.Count == 1&&PostIds.Count > 1)
            {
                Posts.Add(new CommentList { Comments = allPostsList.GetRange(1,PostIds.Count -1), ViewModel = this });
            }

        }
        [RelayCommand]
        public async Task ViewComments(List<int> posts)
        {
            NavigationService.OpenInRightPane(typeof(PostDetailPage), posts);

        }
    }
}
