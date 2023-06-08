using FlarumApi.Helpers;
using FlarumApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using User = FlarumApi.Models.User;

namespace FlarumApi
{
    public sealed class FlarumApiProviders
    {
        public static List<Reaction> Reactions;
        /// <summary>
        /// 通过需要的postId返回帖子
        /// </summary>
        /// <param name="postIds"></param>
        /// <returns></returns>
        public async static Task<List<Post>> GetPostsWithId(List<int> postIds, string forum , string token)
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
        public async static Task<Tuple<List<Post>, string>> GetPostsWithLink(string link , string token)
        { 
            var data = await NetworkHelper.GetAsync(link, token);
            var posts = FlarumApiConverters.GetPosts(data);
            string linkNext = string.Empty;
            if (data["links"]["next"] != null)
            {
                linkNext = data["links"]["next"].ToString();
            }
            return new Tuple<List<Post>, string>(posts, linkNext);
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
        public async static Task<ObservableCollection<Tag>> GetTags(string forum, string token,bool hideChild = false)
        {
            var link = $"https://{forum}/api/tags?include=children";
            var data = await NetworkHelper.GetAsync(link, token);
            var tags = FlarumApiConverters.GetTags(data,hideChild);
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
        public async static Task<Forum> GetForumInfo(string link, string token)
        {
            var data = await NetworkHelper.GetAsync(link, token);
            var forum = new Forum().CreateFromJson(data["data"]);
            forum.Reactions = InclusionTypeFilter.GetInclusions<Reaction>("reactions", data);
            return forum;
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
        public async static Task<Tuple<JObject, string>> CreateDiscussioinAsync(string title,string text, string link,List<Dictionary<string,string>> tags, string token)
        {
            var contents = new Dictionary<string, object>
            {
                {"data",new Dictionary<string,object>
                {
                    {"type","discussions" },
                    {"attributes",new Dictionary<string,string>
                    {                    
                        {"title",title },
                        {"content",text }
                    }
                    },
                    {"relationships",new Dictionary<string,object>
                    {
                        {"tags",new Dictionary<string,object>
                        {
                            { "data",tags }
                        }
                        }
                    }
                    }
                }
                }
            };
            var json = JsonConvert.SerializeObject(contents, Formatting.None);
            var data = await NetworkHelper.PostWithJsonAsync(link, json, token);
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
        public async static Task<Tuple<Post, string>> EditAsync(string text, string link, int postId, string token,string referer)
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
                    {"id",postId },

                }
                }
            };
            var json = JsonConvert.SerializeObject(contents, Formatting.None);
            var data = await NetworkHelper.PatchWithJsonAsync(link, json, token,referer);
            string error = string.Empty;
            var post = new Post();
            if (data["errors"] == null)
            {
                post = Post.CreateFromJson(data["data"]);
            }
            else
            {
                error = data["errors"][0]["code"].ToString();
            }
            var tuple = new Tuple<Post, string>(post, error);
            return tuple;

        }
        public async static Task<bool> VoteAsync(bool up, string link, int postId, string token)
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

                    {"attributes",new List<string>
                    {
                        up.ToString().ToLower(),
                        "false",
                        "vote",
                    }
                    },
                    {"id",postId.ToString() },

                }
                }
            };
            var json = JsonConvert.SerializeObject(contents, Formatting.None);
            var data = await NetworkHelper.PatchWithJsonAsync(link, json, token);
            if (data["data"] != null)
                return true;
            else
                return false;

        }
        public async static Task<UploadItem> UploadAsync(string link,string token, MultipartFormDataContent content)
        {
            var data = await NetworkHelper.UploadAsync(link, content, token);
            return null;
        }
        

    }
    public sealed class FlarumApiConverters
    {
        public static List<Post> GetPosts(JObject jObj)
        {
            var posts = new List<Post>();
            var data = jObj["data"];
            var inclusions = InclusionTypeFilter.FiltTypes(jObj["included"]);
            var users = inclusions.Item2;
            var discussions = inclusions.Item4;
            var postReactions = InclusionTypeFilter.GetInclusions<PostReaction>("post_reactions",jObj);

            foreach (var datum in data)
            {
                var post = Post.CreateFromJson(datum);
                if (post.DiscussionId != null)
                    post.Discussion = discussions.FirstOrDefault(p => p.Id == post.DiscussionId);
                try
                {
                    if(post.ReactionIds != null)
                    {
                        if (post.ReactionIds.Count != 0)
                        {
                            post.Reactions = new List<Reaction>();
                            foreach (var id in post.ReactionIds)
                            {
                                var postReaction = postReactions.FirstOrDefault(p => p.Id == id);
                                post.Reactions.Add(FlarumApiProviders.Reactions.FirstOrDefault(p => p.Id == postReaction.ReactionId));
                            }
                        }
                    }

                    if (post.LikeIds != null)
                    {
                        post.Likes = new List<User>();
                        foreach (var id in post.LikeIds)
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

                }
                finally
                {
                    post.User = users.FirstOrDefault(p => p.Id == post.UserId) ?? Preset.DefaultUser;
                    posts.Add(post);
                }

            }
            return posts;
        }

        public static Discussion GetDiscussion(JObject jObj)
        {
            var data = jObj["data"];
            var discussion = new Discussion().CreateFromJson(data);
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
                var discussion = new Discussion().CreateFromJson(datum);
                discussion.User = users.FirstOrDefault(p => p.Id == discussion.UserId) ?? Preset.DefaultUser ;
                discussion.LastPostedUser = users.FirstOrDefault(p => p.Id == discussion.LastPostedUserId) ?? Preset.DefaultUser;
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
                user =  new User().CreateFromJson(data[0]);
            else
                user =  new User().CreateFromJson(data);
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
        public static ObservableCollection<Tag> GetTags(JObject jObj,bool hideChild = false)
        {
            var tags = new ObservableCollection<Tag>();
            var data = jObj["data"];
            foreach(var item in data)
            {
                var tag = new Tag().CreateFromJson(item);
                if (!tag.IsChild||!hideChild)
                    tags.Add(tag);
            }
            foreach (var item in tags)
            {
                var inclustions = InclusionTypeFilter.FiltTypes(jObj["included"]);
                var children = inclustions.Item3;

                if (item.ChidrenIds != null)
                {
                    item.Chidren = new List<Tag>();
                    foreach (var id in item.ChidrenIds)
                    {
                        item.Chidren.Add(children.First(p => p.Id == id));
                    }
                }
            }
            return tags;
        }
        public static ObservableCollection<User> GetUsers(JObject jObj)
        {
            var users = new ObservableCollection<User>();
            var data = jObj["data"];
            foreach (var user in data)
                users.Add(new User().CreateFromJson(user));
            return users;
        }
        public static ObservableCollection<Notification> GetNotifications(JObject jObj)
        {
            var notifications = new ObservableCollection<Notification>();
            var data = jObj["data"];
            foreach (var notification in data)
            {
                notifications.Add(new Notification().CreateFromJson(notification));
            }
            return notifications;
        }
    }
    public class Preset
    {
        public static User DefaultUser
        {
            get
            {
                return new User
                {
                    Id = 0,
                    UserName = "已注销",
                    DisplayName = "已注销",
                    Slug = "已注销",
                    Bio = "此用户已注销",
                    DiscussionCount = 0,
                    CommentCount = 0,
                    AvatarUrl = "https://img1.baidu.com/it/u=1274193816,3942380583&fm=253&fmt=auto&app=138&f=JPEG?w=500&h=500",

                };

            }
        }
        public static User NotLoginedUser
        {
            get
            {
                return new User
                {
                    Id = 0,
                    UserName = "未登录",
                    DisplayName = "未登录",
                    Slug = "未登录",
                    Bio = "未登录",
                    DiscussionCount = 0,
                    CommentCount = 0,
                    AvatarUrl = "ms-appx:///Assets/App/guest.png",

                };

            }
        }
    }


}
