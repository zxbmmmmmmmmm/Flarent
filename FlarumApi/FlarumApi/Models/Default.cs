using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlarumApi 
{ 
    public class Default
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
                    Bio="此用户已注销",
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
                    AvatarUrl = "https://img.moegirl.org.cn/common/thumb/b/b7/Transparent_Akkarin.jpg/280px-Transparent_Akkarin.jpg",

                };

            }
        }
    }
}
