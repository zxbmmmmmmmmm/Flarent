using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlarumApi.Helpers
{
    internal interface IFlarumModel<T>
    {
         T CreateFromJson(JToken token);
    }
}
