using FlarumApi.Helpers;
using FlarumApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlarumApi
{
    public sealed class FlarumApiProviders
    {
        /// <summary>
        /// 通过需要的postId返回帖子
        /// </summary>
        /// <param name="postIds"></param>
        /// <returns></returns>
        public async static Task<ObservableCollection<Post>> GetPostsWithId(List<int> postIds, string forum , string token)
        {
            var link = $"https://{forum}/api/posts?page[limit]=30&filter[id]=";
            foreach (int postId in postIds)
            {
                link = link.Insert(link.Length, postId.ToString() + ",");//逐个添加需要的post
            }
            link = link.Remove(link.Length - 1, 1);//去除最后的逗号
            var data = await NetworkHelper.GetAsync(link,token);           
            return FlarumApiConverters.GetPosts(data);
        }
        /// <summary>
        /// 通过链接返回帖子
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public async static Task<Tuple<ObservableCollection<Post>, string>> GetPostsWithLink(string link , string token)
        { 
            var data = await NetworkHelper.GetAsync(link, token);
            var posts = FlarumApiConverters.GetPosts(data);
            string linkNext = string.Empty;
            if (data["links"]["next"] != null)
            {
                linkNext = data["links"]["next"].ToString();
            }
            return new Tuple<ObservableCollection<Post>, string>(posts, linkNext);
        }
        /// <summary>
        /// 获取单个讨论信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageNear"></param>
        /// <param name="forum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async static Task<Discussion> GetDiscussion(int id,int pageNear, string forum , string token)
        {
            var link = $"https://{forum}/api/discussions/{id}?page[near]={pageNear}";
            var data = await NetworkHelper.GetAsync(link,token);
            return FlarumApiConverters.GetDiscussion(data);
        }
        /// <summary>
        /// 根据链接获取首页讨论
        /// </summary>
        /// <param name="query"></param>
        /// <param name="targetLink"></param>
        /// <param name="forum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async static Task<Tuple<ObservableCollection<Discussion>,string>> GetDiscussions(string query,string targetLink,string forum , string token)
        {
            string link = string.Empty;
            if(targetLink != null)
            {
                link = targetLink;
            }
            else
            {
                link = $"https://{forum}/api/discussions?page[limit]=30{query}";
            }
            var data = await NetworkHelper.GetAsync(link, token);
            string linkNext = string.Empty;
            if(data["links"]["next"]!= null)
            {
                linkNext = data["links"]["next"].ToString();
            }
            var discussions = FlarumApiConverters.GetDiscussions(data);
            return new Tuple<ObservableCollection<Discussion>, string>(discussions, linkNext);
        }
        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        /// <param name="link"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async static Task<User> GetUser(string link , string token)
        {
            var data = await NetworkHelper.GetAsync(link, token);
            var user = FlarumApiConverters.GetUser(data);
            return user;
        }
        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <param name="forum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async static Task<ObservableCollection<Tag>> GetTags(string forum, string token)
        {
            var link = $"https://{forum}/api/tags";
            var data = await NetworkHelper.GetAsync(link, token);
            var tags = FlarumApiConverters.GetTags(data);
            return tags;
        }
        /// <summary>
        /// 根据论坛返回通知(需要登录)
        /// </summary>
        /// <param name="forum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async static Task<ObservableCollection<Notification>> GetNotifications(string forum, string token)
        {
            var link = $"https://{forum}/api/notifications";
            var data = await NetworkHelper.GetAsync(link, token);
            var notifications = FlarumApiConverters.GetNotifications(data);
            return notifications;
        }
        public async static Task<Tuple<ObservableCollection<User>,string>> GetUsers(string link,string token)
        {
            var data = await NetworkHelper.GetAsync(link, token);
            var users = FlarumApiConverters.GetUsers(data); 
            string linkNext = string.Empty;
            if (data["links"]["next"] != null)
            {
                linkNext = data["links"]["next"].ToString();
            }
            return new Tuple<ObservableCollection<User>, string>(users,linkNext);
        }
        /// <summary>
        /// 返回UserId,Token,StatusCode
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async static Task<Tuple<int,string,string>> GetToken(string userName, string password,string forum,bool isRemembered)
        {

            //var token = ApplicationData.Current.LocalSettings.Values["token"].ToString();
            //client.DefaultRequestHeaders.Add("Authorization", "Token " + token);


            var values = new Dictionary<string, string>
            {
                { "identification", userName },
                { "password", password },
                { "remember", isRemembered.ToString() },
            }; 
            var content = new FormUrlEncodedContent(values);

            var data = await NetworkHelper.PostAsync($"https://{forum}/api/token",content,"");
            string token  = string.Empty;
            int userId = 0;
            string error = string.Empty;
            if (data["token"] != null)
            {
                token = data["token"].ToString();
                userId = (int)data["userId"];
            }
            else
            {
                error = data["errors"][0]["code"].ToString();
            }
            var tuple = new Tuple<int, string, string>(userId, token, error);
            return tuple;

        }
        public async static Task<Tuple<JObject, string>> ReplyAsync(string text,string link,int discussionId,string token)
        {

            //var token = ApplicationData.Current.LocalSettings.Values["token"].ToString();
            //client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            //var datum = new ReplyData { data = new Reply { type = "posts", attributes = new ReplyAttributes { content = text }, relationships = new Relationships { discussion = new Discussion { data = new Data { id = discussionId.ToString() } } } } };
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(datum);

            var contents = new Dictionary<string, object>
            {
                {"data",new Dictionary<string,object>
                {
                    {"type","posts" },
                    {"attributes",new Dictionary<string,string>
                    {
                        {"content",text }
                    }
                    },
                    {"relationships",new Dictionary<string,object>
                    {
                        {"discussion",new Dictionary<string,object>
                        {
                            { "data",new Dictionary<string,string>
                            {
                                {"type","discussions" },
                                {"id",discussionId.ToString() }
                            }
                            }
                        }
                        }
                    }
                    }
                }
                }
            };
            var json = JsonConvert.SerializeObject(contents, Formatting.None);
            var data = await NetworkHelper.PostWithJsonAsync(link,json,token);
            string error = string.Empty;
            if (data["errors"] == null)
            {

            }
            else
            {
                error = data["errors"][0]["code"].ToString();
            }
            var tuple = new Tuple<JObject, string>(data, error);
            return tuple;

        }
    }
    public sealed class FlarumApiConverters
    {
        public static ObservableCollection<Post> GetPosts(JObject jObj)
        {
            var posts = new ObservableCollection<Post>();
            var data = jObj["data"];
            var inclusions = InclusionTypeFilter.FiltTypes(jObj["included"]);
            var users = inclusions.Item2;
            var discussions = inclusions.Item4;

            foreach (var datum in data)
            {
                var post = Post.CreateFromJson(datum);
                if (post.DiscussionId != null)
                    post.Discussion = discussions.FirstOrDefault(p => p.Id == post.DiscussionId);
                if(post.LikeIds != null)
                {
                    post.Likes = new List<User>();
                    foreach(var id in post.LikeIds)
                    {
                        post.Likes.Add(users.FirstOrDefault(p => p.Id == id));
                    }
                }
                if (post.Likes != null)
                {
                    if (post.Likes.Count != 0)
                        post.ShowLikeIcon = true;
                    else
                        post.ShowLikeIcon = false;
                }
                else
                    post.ShowLikeIcon = false;

                post.User = users.FirstOrDefault(p => p.Id == post.UserId) ?? Default.DefaultUser;
                posts.Add(post);
            }
            return posts;
        }

        public static Discussion GetDiscussion(JObject jObj)
        {
            var data = jObj["data"];
            var discussion = Discussion.CreateFromJson(data);
            var inclusions = InclusionTypeFilter.FiltTypes(jObj["included"]);
            var users = inclusions.Item2;
            var tags = inclusions.Item3;
            discussion.Tags = tags;
            return discussion;
        }
        public static ObservableCollection<Discussion> GetDiscussions(JObject jObj)
        {
            var discussions = new ObservableCollection<Discussion>();
            var data = jObj["data"];
            var inclustions = InclusionTypeFilter.FiltTypes(jObj["included"]);
            var posts = inclustions.Item1;
            var users = inclustions.Item2; 
            var tags = inclustions.Item3;
            foreach (var datum in data)
            {
                var discussion = Discussion.CreateFromJson(datum);
                discussion.User = users.FirstOrDefault(p => p.Id == discussion.UserId) ?? Default.DefaultUser ;
                discussion.LastPostedUser = users.FirstOrDefault(p => p.Id == discussion.LastPostedUserId) ?? Default.DefaultUser;
                discussion.FirstPost = posts.FirstOrDefault(p => p.Id == discussion.FirstPostId) ?? null;

                if (discussion.TagIds != null)
                {
                    discussion.Tags = new ObservableCollection<Tag>();
                    foreach (var id in discussion.TagIds)
                    {
                        var tag = tags.First(p => p.Id == id);
                        discussion.Tags.Add(tag);
                    }
                }

                discussions.Add(discussion);
            }
            return discussions;
        }
        public static User GetUser(JObject jObj)
        {
            var user = new User();
            var data = jObj["data"];



            if (data is JArray)
                user =  User.CreateFromJson(data[0]);
            else
                user =  User.CreateFromJson(data);
            if (jObj["included"] != null)
            {
                var inclustions = InclusionTypeFilter.FiltTypes(jObj["included"]);
                var userGroups = inclustions.Item5;
                if (user.GroupIds != null)
                {
                    user.UserGroups = new ObservableCollection<UserGroup>();
                    foreach (var id in user.GroupIds)
                    {
                        user.UserGroups.Add(userGroups.FirstOrDefault(p => p.Id == id));
                    }
                }
            }

            return user;
        }
        public static ObservableCollection<Tag> GetTags(JObject jObj)
        {
            var tags = new ObservableCollection<Tag>();
            var data = jObj["data"];
            foreach(var tag in data)
            {
                tags.Add(Tag.CreateFromJson(tag));
            }
            return tags;
        }
        public static ObservableCollection<User> GetUsers(JObject jObj)
        {
            var users = new ObservableCollection<User>();
            var data = jObj["data"];
            foreach (var user in data)
                users.Add(User.CreateFromJson(user));
            return users;
        }
        public static ObservableCollection<Notification> GetNotifications(JObject jObj)
        {
            var notifications = new ObservableCollection<Notification>();
            var data = jObj["data"];
            foreach (var notification in data)
            {
                notifications.Add(Notification.CreateFromJson(notification));
            }
            return notifications;
        }
    }

}
