﻿using FlarumApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using User = FlarumApi.Models.User;

namespace FlarumApi.Helpers

{
    internal class InclusionTypeFilter
    {
        /// <summary>
        /// 筛选并序列化included中的内容
        /// <para>1Post,2User,3Tag,4Discussion,5UserGroup</para>
        /// </summary>
        /// <param name="inclusions">included</param>
        /// <returns></returns>
        public static Tuple<ObservableCollection<Post>, ObservableCollection<User>, ObservableCollection<Tag>, ObservableCollection<Discussion>, ObservableCollection<UserGroup>> FiltTypes(JToken inclusions)
        {
            var posts = new ObservableCollection<Post>();
            var users = new ObservableCollection<User>();
            var tags = new ObservableCollection<Tag>();
            var discussions= new ObservableCollection<Discussion>();
            var userGroups = new ObservableCollection<UserGroup>();

            foreach (var inclusion in inclusions)
            {
                switch (inclusion.Value<string>("type"))
                {
                    case "posts":
                        posts.Add(Post.CreateFromJson(inclusion));
                        break;
                    case "users":
                        var user = new User().CreateFromJson(inclusion);
                        if (user.AvatarUrl == null)
                            user.AvatarUrl = "https://img.moegirl.org.cn/common/thumb/b/b7/Transparent_Akkarin.jpg/280px-Transparent_Akkarin.jpg";
                        users.Add(user);
                        break;
                    case "tags":
                        tags.Add(new Tag().CreateFromJson(inclusion));
                        break;
                    case "discussions":
                        discussions.Add(new Discussion().CreateFromJson(inclusion));
                        break;
                    case "groups":
                        userGroups.Add(new UserGroup().CreateFromJson(inclusion));
                        break;
                }
            }
            return new Tuple<ObservableCollection<Post>, ObservableCollection<User>, ObservableCollection<Tag>,ObservableCollection<Discussion>, ObservableCollection<UserGroup>>(posts, users, tags,discussions,userGroups);
        }
        public static List<T> GetInclusions<T>(string type, JToken token) where T : IFlarumModel<T>
        {
            var inclusions = token["included"];
            var list = inclusions.Where(p => p.Value<string>("type") == type).ToList();
            var data = new List<T>();
            foreach (var item in list)
            {
                IFlarumModel<T> datum = Activator.CreateInstance(typeof(T)) as IFlarumModel<T>;
                data.Add(datum.CreateFromJson(item));
            }
            return data;
        }


    }
}
