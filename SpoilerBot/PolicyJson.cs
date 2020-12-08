using Newtonsoft.Json;

namespace SpoilerBot
{
    public struct PolicyJson
    {
        [JsonProperty("book_policies")]
        public BookPolicyJson[] policies { get; private set; }
    }

    public struct BookPolicyJson
    {
        [JsonProperty("book_name")]
        public string name { get; private set; }

        [JsonProperty("aliases")]
        public string[] aliases { get; private set; }

        [JsonProperty("allowed_full")]
        public string[] fullChannels { get; private set; }

        [JsonProperty("allowed_tagged")]
        public string[] taggedChannels { get; private set; }
    }
}
