using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalrApi.TokenHelper
{
    public class TokenManagement
    {
        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        /// <summary>
        /// 过期时间(单位秒)
        /// </summary>
        [JsonProperty("accessTokenExpiresSeconds")]
        public int AccessTokenExpiresSeconds { get; set; }
    }
}
