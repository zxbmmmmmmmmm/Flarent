using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FlarumApi.Models
{
    public class Forum 
    {
        public string FavIcon { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string BaseUrl { get; set; }
        public static Forum CreateFromJson(JToken token)
        {
            var attributes = token.Value<JToken>("attributes");
            var forum = new Forum
            { 
                BaseUrl = attributes.Value<string>("baseUrl"),
                Website = attributes.Value<string>("baseUrl").Replace("https://",""),
                Logo = attributes.Value<string>("logoUrl")?? "ms-appx:///Assets/StoreLogo.png",
                FavIcon = attributes.Value<string>("faviconUrl") ?? "ms-appx:///Assets/StoreLogo.png",
                Name = attributes.Value<string>("title"),
                Description = attributes.Value<string>("description"),

            };
            return forum;
        }
    }

    public class Discussion
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int? CommentCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastPostedAt { get; set; }
        public bool? HasBestAnswer { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
        public int LastPostedUserId { get; set; }
        public User LastPostedUser { get; set; }
        public int FirstPostId { get; set; }
        public Post FirstPost { get; set; }
        public List<int> TagIds { get; set; }
        public ObservableCollection<Tag> Tags { get; set; }
        public ObservableCollection<Post> Posts { get; set; }
        /// <summary>
        /// 从json创建一个Discussion
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Discussion CreateFromJson(JToken token)
        {
            var attributes = token.Value<JToken>("attributes");
            var relationships = token.Value<JToken>("relationships");
            var discussion = new Discussion
            {
                Id = token.Value<int?>("id"),
                Title = attributes.Value<string>("title"),
                CommentCount = attributes.Value<int?>("commentCount"),
                CreatedAt = attributes.Value<DateTime?>("createdAt") ?? null,
                LastPostedAt = attributes.Value<DateTime?>("lastPostedAt") ?? null,
                HasBestAnswer = attributes.Value<bool?>("hasBestAnswer"),
            };
            discussion.Posts = new ObservableCollection<Post>();
            if (relationships != null)
            {
                if (relationships["posts"] != null)
                {
                    foreach (var datum in relationships["posts"]["data"])
                    {
                        discussion.Posts.Add(new Post { Id = (int?)datum["id"] });
                    }
                }
                if (relationships["user"] != null)
                    discussion.UserId = (int)relationships["user"]["data"]["id"];
                if (relationships["lastPostedUser"] != null)
                    discussion.LastPostedUserId = (int)relationships["lastPostedUser"]["data"]["id"];
                if (relationships["firstPost"] != null)
                    discussion.FirstPostId = (int)relationships["firstPost"]["data"]["id"];
                if (relationships["tags"] != null)
                {
                    discussion.TagIds = new List<int>();
                    foreach (var tag in relationships["tags"]["data"])
                        discussion.TagIds.Add((int)tag["id"]);
                }
            }
            return discussion;
        }
    }
    public class Post
    {
        public int? Id { get; set; }
        public int? Number { get; set; }
        public string ContentType { get; set; }
        public string ContentHtml { get; set; }
        public int? Votes { get; set; }
        public bool HasUpvoted { get; set; }
        public object Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public int? DiscussionId { get; set; }
        public Discussion Discussion { get; set; }
        public SpecialContent SpecialContent { get; set; }
        public List<int> LikeIds { get; set; }
        public List<User> Likes { get; set; }
        public bool ShowLikeIcon { get; set; }

        public static Post CreateFromJson(JToken token)
        {
            var attributes = token.Value<JToken>("attributes");
            var relationships = token.Value<JToken>("relationships");
            var post = new Post
            {
                Id = token.Value<int?>("id") ?? null,
            };
            if (relationships != null)
            {
                if (relationships["user"] != null)
                {
                    post.UserId = relationships.Value<JToken>("user").Value<JToken>("data").Value<int?>("id") ?? null;
                }
                if (relationships["discussion"] != null)
                {
                    post.DiscussionId = relationships.Value<JToken>("discussion").Value<JToken>("data").Value<int?>("id") ?? null;
                }
                if (relationships["likes"] != null)
                {
                    post.LikeIds = new List<int>();
                    foreach (var like in relationships["likes"]["data"])
                    {
                        post.LikeIds.Add((int)like["id"]);
                    }
                }
            }
            post.Number = attributes.Value<int?>("number") ?? null;
            post.Content = attributes.Value<object>("content") ?? null;
            post.ContentType = attributes.Value<string>("contentType") ?? null;
            post.Votes = attributes.Value<int?>("votes") ?? null;
            post.HasUpvoted = attributes.Value<bool>("hasUpvoted");
            post.ContentHtml = attributes.Value<string>("contentHtml") ?? null;
            post.CreatedAt = attributes.Value<DateTime?>("createdAt") ?? null;
            post.EditedAt = attributes.Value<DateTime?>("editedAt") ?? null;

            if (post.ContentType != "comment"&&post.ContentHtml == null)
            {
                post.SpecialContent = new SpecialContent();
                switch (post.ContentType)
                {
                    case "discussionStickied":
                        if (post.Content.ToString().Contains("true"))
                        {
                            post.SpecialContent.Icon = "\uE840";
                            post.SpecialContent.Description = "置顶了此贴";
                        }
                        else
                        {
                            post.SpecialContent.Icon = "\uE196";
                            post.SpecialContent.Description = "取消置顶此贴";
                        }
                        break;
                    case "discussionTagged":
                        post.SpecialContent.Icon = "\uE1CB";
                        post.SpecialContent.Description = "更改了标签";
                        break;
                    case "discussionSplit":
                        post.SpecialContent.Icon = "\uE14B";
                        post.SpecialContent.Description = "拆分回复";
                        break;
                    case "discussionRenamed":
                        post.SpecialContent.Icon = "\uE13E";
                        var jobj = JArray.Parse(post.Content.ToString());                        
                        post.SpecialContent.Description = $"将标题由`{jobj[0]}`更改为`{jobj[1]}`";
                        break;
                    default:
                        post.SpecialContent.Icon = "\uF142";
                        post.SpecialContent.Description = "未知操作";
                        break;
                }
            }
            return post;
        }

    }
    public class SpecialContent
    {
        public string Icon { get; set; }
        public string Description { get; set; }

    }
    public class User
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Slug { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? JoinTime { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public string Bio { get; set; }
        public int DiscussionCount { get; set; }
        public int CommentCount { get; set; }
        public ObservableCollection<UserGroup> UserGroups { get; set; }
        public List<int> GroupIds{ get; set; }
        public static User CreateFromJson(JToken token)
        {
            var attributes = token.Value<JToken>("attributes");
            var relationships = token.Value<JToken>("relationships");
            var user = new User
            {
                Id = token.Value<int?>("id"),
                UserName = attributes.Value<string>("username"),
                DisplayName = attributes.Value<string>("displayName"),
                Slug = attributes.Value<string>("slug"),
                AvatarUrl = attributes.Value<string>("avatarUrl")?? "https://img.moegirl.org.cn/common/thumb/b/b7/Transparent_Akkarin.jpg/280px-Transparent_Akkarin.jpg",
                Bio = attributes.Value<string>("bio"),
                DiscussionCount = attributes.Value<int>("discussionCount"),
                CommentCount = attributes.Value<int>("commentCount"),
                JoinTime = attributes.Value<DateTime?>("joinTime") ?? null,
                LastSeenAt = attributes.Value<DateTime?>("lastSeenAt") ?? null
            };
            if (relationships != null)
            {
                if (relationships["groups"] != null)
                {
                    user.GroupIds = new List<int>();
                    foreach (var like in relationships["groups"]["data"])
                    {
                        user.GroupIds.Add((int)like["id"]);
                    }
                }
            }
            return user;
        }
    }
    public class UserGroup
    {
        public int Id { get; set; }
        public string NameSingular { get; set; }
        public string NamePlural { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public static UserGroup CreateFromJson(JToken token)
        {
            var attributes = token.Value<JToken>("attributes");
            var userGroup = new UserGroup
            {
                Id = token.Value<int>("id"),
                NameSingular = attributes.Value<string>("nameSingular"),
                NamePlural = attributes.Value<string>("namePlural"),
                Color = attributes.Value<string>("color"),
                Icon = attributes.Value<string>("icon"),
            };
            return userGroup;
        }
    }

        /*
        /// <summary>
        /// 对应api内relationships
        /// <para>所有内容有为int，对应(内容)->data->id</para>
        /// </summary>
        public class RelationShips
        {
            public int FirstPost { get; set; }
            public int LastPost { get; set; }
            public int User { get; set; }
            public int LastPostedUser { get; set; }
            public static RelationShips CreateFromJson(JToken token)
            {
                var relationships = new RelationShips
                {
                    FirstPost = token.Value<JToken>("firstPost").Value<int>("data");
                };

            }
        }*/

    public class Tag
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int PostCount { get; set; }
        public int DiscussionCount { get; set; }
        public static Tag CreateFromJson(JToken token)
        {
            var attributes = token.Value<JToken>("attributes");
            var tag = new Tag
            {
                Id = token.Value<int?>("id"),
                Name = attributes.Value<string>("name"),
                Slug = attributes.Value<string>("slug"),
                Icon = attributes.Value<string>("icon"),
                Description = attributes.Value<string>("description"),
                Color = attributes.Value<string>("color"),
                PostCount = attributes.Value<int>("postCount"),
                DiscussionCount = attributes.Value<int>("discussionCount"),
            };

            return tag;
        }

    }
    public class Notification
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public object Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
        public static Notification CreateFromJson(JToken token)
        {
            var attributes = token.Value<JToken>("attributes");
            var notification = new Notification
            {
                Id = token.Value<int?>("id"),
                Type = attributes.Value<string>("type"),
                Content = attributes.Value<object>("content"),
                IsRead = attributes.Value<bool>("isRead"),
                CreatedAt = attributes.Value<DateTime?>("createdAt") ?? null,
            };

            return notification;
        }

    }

}
