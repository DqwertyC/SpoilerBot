using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpoilerBot.Commands
{
    public class BaseCommands : BaseCommandModule
    {
        [Command("spoilers")]
        [Description("Displays spoiler policy for the current channel or a given book")]
        public async Task SpoilerPolicy(CommandContext ctx, params string[] args)
        {
            if (args.Length == 0)
            {
                await PostPolicyForChannel(ctx);
            }
            else
            {
                StringBuilder bookName = new StringBuilder();

                foreach (string s in args)
                {
                    bookName.Append(s + " ");
                }

                bookName.Length = bookName.Length - 1;

                await PostPolicyForBook(ctx, bookName.ToString());
            }
        }

        private async Task PostPolicyForChannel(CommandContext ctx)
        {
            string channelName = ctx.Channel.Name;
 
            DiscordEmbedBuilder responseBuilder = new DiscordEmbedBuilder();
            responseBuilder.Title = "Spoiler policy for **#" + channelName + "**";

            var booksFull = Bot.masterPolicy.FullSpoilersInChannel(channelName);
            var booksTagged = Bot.masterPolicy.TaggedSpoilersInChannel(channelName);

            bool untaggedAllowed = booksFull.Count > 0;
            bool taggedAllowed = booksTagged.Count > 0;

            if (!untaggedAllowed && !taggedAllowed)
            {
                responseBuilder = responseBuilder.AddField("No spoilers allowed!", "Tagged **or** Untagged!");
            }

            if (untaggedAllowed)
            {
                StringBuilder untaggedList = new StringBuilder();

                foreach (string s in booksFull)
                {
                    untaggedList.Append("- " + s + "\n");
                }

                untaggedList.Length = untaggedList.Length - 1;
                responseBuilder = responseBuilder.AddField("Untagged spoilers for the following books are allowed:", untaggedList.ToString());
            }

            if (taggedAllowed)
            {
                StringBuilder taggedList = new StringBuilder();

                foreach (string s in booksTagged)
                {
                    taggedList.Append("- " + s + "\n");
                }

                taggedList.Length = taggedList.Length - 1;
                responseBuilder = responseBuilder.AddField("*Tagged* spoilers for the following books are allowed:", taggedList.ToString());

            }

            await ctx.Channel.SendMessageAsync(embed: responseBuilder.Build()).ConfigureAwait(false);
        }

        private async Task PostPolicyForBook(CommandContext ctx, string bookName)
        {
            if (Bot.masterPolicy.TryGetBookName(bookName, out string cleanedBookName))
            {
                DiscordEmbedBuilder responseBuilder = new DiscordEmbedBuilder();

                responseBuilder.Title = "Spoiler policy for **" + cleanedBookName + "**";

                var channelsFull = Bot.masterPolicy.FullSpoilersInChannel(cleanedBookName);
                var channelsTagged = Bot.masterPolicy.TaggedSpoilersInChannel(cleanedBookName);

                bool untaggedAllowed = channelsFull.Count > 0;
                bool taggedAllowed = channelsTagged.Count > 0;

                if (!untaggedAllowed && !taggedAllowed)
                {
                    responseBuilder = responseBuilder.AddField("**No discussion** of this book is allowed on this server.", "Tagged **or** Untagged!");
                }

                if (untaggedAllowed)
                {
                    StringBuilder untaggedList = new StringBuilder();

                    foreach (string s in channelsFull)
                    {
                        untaggedList.Append("#" + s + "\n");
                    }

                    untaggedList.Length = untaggedList.Length - 1;
                    responseBuilder = responseBuilder.AddField("Untagged discussion is allowed in these channels:", untaggedList.ToString());
                }

                if (taggedAllowed)
                {
                    StringBuilder taggedList = new StringBuilder();

                    foreach (string s in channelsTagged)
                    {
                        taggedList.Append("#" + s + "\n");
                    }

                    taggedList.Length = taggedList.Length - 1;
                    responseBuilder = responseBuilder.AddField("*Tagged* discussion  is allowed in these channels:", taggedList.ToString());
                }

                await ctx.Channel.SendMessageAsync(embed: responseBuilder.Build()).ConfigureAwait(false);
            }
        }
    }
}
