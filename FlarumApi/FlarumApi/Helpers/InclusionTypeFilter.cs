using FlarumApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarumApi.Helpers

{
    internal class InclusionTypeFilter
    {
        /// <summary>
        /// 筛选并序列化included中的内容
        /// <para>1Post,2User,3Tag,4Discussion</para>
        /// </summary>
        /// <param name="inclusions">included</param>
        /// <returns></returns>
        public static Tuple<ObservableCollection<Post>, ObservableCollection<User>, ObservableCollection<Tag>, ObservableCollection<Discussion>> FiltTypes(JToken inclusions)
        {
            var posts = new ObservableCollection<Post>();
            var users = new ObservableCollection<User>();
            var tags = new ObservableCollection<Tag>();
            var discussions= new ObservableCollection<Discussion>();

            foreach (var inclusion in inclusions)
            {
                switch (inclusion.Value<string>("type"))
                {
                    case "posts":
                        posts.Add(Post.CreateFromJson(inclusion));
                        break;
                    case "users":
                        var user = User.CreateFromJson(inclusion);
                        if (user.AvatarUrl == null)
                            user.AvatarUrl = "https://img.moegirl.org.cn/common/thumb/b/b7/Transparent_Akkarin.jpg/280px-Transparent_Akkarin.jpg";
                        users.Add(user);
                        break;
                    case "tags":
                        tags.Add(Tag.CreateFromJson(inclusion));
                        break;
                    case "discussions":
                        discussions.Add(Discussion.CreateFromJson(inclusion));
                        break;
                }
            }
            return new Tuple<ObservableCollection<Post>, ObservableCollection<User>, ObservableCollection<Tag>,ObservableCollection<Discussion>>(posts, users, tags,discussions);
        }
    }
}
