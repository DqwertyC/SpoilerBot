using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpoilerBot
{
    public static class SpoilerPolicy
    {
        // Get a list of books that have full spoilers enabled in the given channel
        public static List<string> FullSpoilersInChannel(this PolicyJson policy, string channelName)
        {
            var booksAllowed = new List<string>();

            foreach (BookPolicyJson bookPolicy in policy.policies)
            {
                if (bookPolicy.fullChannels.Contains(channelName))
                {
                    booksAllowed.Add(bookPolicy.name);
                }
            }

            return booksAllowed;
        }

        // Get a list of books that have tagged spoilers enabled in the given channel
        public static List<string> TaggedSpoilersInChannel(this PolicyJson policy, string channelName)
        {
            var booksAllowed = new List<string>();

            foreach (BookPolicyJson bookPolicy in policy.policies)
            {
                if (bookPolicy.taggedChannels.Contains(channelName))
                {
                    booksAllowed.Add(bookPolicy.name);
                }
            }

            return booksAllowed;
        }

        // Try to get a list of channels that allow full spoilers for a given book
        public static bool TryGetChannelsFull(this PolicyJson policy, string bookName, out List<string> channelsAllowed)
        {
            channelsAllowed = null;

            if (policy.TryGetBookPolicy(bookName, out BookPolicyJson bookPolicy))
            {
                channelsAllowed = new List<string>();
                channelsAllowed.AddRange(bookPolicy.fullChannels);
                return true;
            }

            return false;
        }

        // Try to get a list of channels that allow full spoilers for a given book. Checks aliases as well.
        public static bool TryGetChannelsTagged(this PolicyJson policy, string bookName, out List<string> channelsAllowed)
        {
            channelsAllowed = null;

            if (policy.TryGetBookPolicy(bookName, out BookPolicyJson bookPolicy))
            {
                channelsAllowed = new List<string>();
                channelsAllowed.AddRange(bookPolicy.taggedChannels);
                return true;
            }

            return false;
        }

        // Try to get a book policy given a book name. Checks aliases as well.
        public static bool TryGetBookPolicy(this PolicyJson policy, string bookName, out BookPolicyJson outBookPolicy)
        {
            foreach (BookPolicyJson bookPolicy in policy.policies)
            {
                if (bookPolicy.name.ToLower().Equals(bookName.ToLower()) || bookPolicy.aliases.Contains(bookName))
                {
                    outBookPolicy = bookPolicy;
                    return true;
                }
            }

            outBookPolicy = new BookPolicyJson();

            return false;
        }

        public static bool TryGetBookName(this PolicyJson policy, string bookName, out string cleanedName)
        {
            if (policy.TryGetBookPolicy(bookName, out BookPolicyJson bookPolicy))
            {
                cleanedName = bookPolicy.name;
                return true;
            }

            cleanedName = "";
            return false;
        }

        private static bool Contains(this string[] haystack, string needle)
        {
            foreach (string s in haystack)
            {
               if (s.ToLower().Equals(needle.ToLower())) return true;
            }

            return false;
        }
    }

    
}
