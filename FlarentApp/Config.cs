using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarentApp
{
    /// <summary>
    /// 配置文件，你可以更改此处的内容以将Flarent改成论坛的专属客户端
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 默认论坛
        /// </summary>
        public static string Forum = "discuss.flarum.org.cn";
        /// <summary>
        /// 专属客户端模式。开启后“切换论坛”将被自动隐藏，同时会更改一些内容
        /// </summary>
        public static bool IsClientModeEnabled = true;
    }
}
