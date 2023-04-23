using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlarentApp.Helpers;
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
        public ObservableCollection<Post> posts = new();
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
            if (LinkNext == null)
                tup = await FlarumApiProviders.GetPostsWithLink($"https://{Flarent.Settings.Forum}/api/posts?filter[discussion]={Discussion.Id}?filter[author]={User.UserName}", Flarent.Settings.Token);
            else if (LinkNext == "")
                return;
            else
                tup = await FlarumApiProviders.GetPostsWithLink(LinkNext, Flarent.Settings.Token);
            var _posts = tup.Item1;
            LinkNext = tup.Item2;
            foreach (var post in _posts)
            {
                Posts.Add(post);
            }
        }
    }
}
