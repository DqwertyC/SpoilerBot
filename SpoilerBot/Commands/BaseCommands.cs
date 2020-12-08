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

            // If there are no arguments, give the spoiler policy
            if (args.Length == 0)
            {
                await PostPolicyForChannel(ctx);
            }
            else
            {
                // Get the bookname from the args, then show policy for that book
                StringBuilder bookName = new StringBuilder();

                foreach (string s in args)
                {
                    bookName.Append(s + " ");
                }

                bookName.Length = bookName.Length - 1;

                await PostPolicyForBook(ctx, bookName.ToString());
            }
        }

        // Post an embed containing the spoiler policy for the current channel
        private async Task PostPolicyForChannel(CommandContext ctx)
        {
            string channelName = ctx.Channel.Name;
 
            DiscordEmbedBuilder responseBuilder = new DiscordEmbedBuilder();
            responseBuilder.Title = "Spoiler policy for **#" + channelName + "**";

            // Get list of books that are allowed tagged/untagged
            var booksUntagged = Bot.masterPolicy.FullSpoilersInChannel(channelName);
            var booksTagged = Bot.masterPolicy.TaggedSpoilersInChannel(channelName);

            bool untaggedAllowed = booksUntagged.Count > 0;
            bool taggedAllowed = booksTagged.Count > 0;

            // If there's no books allowed, show a shorter message
            if (!untaggedAllowed && !taggedAllowed)
            {
                responseBuilder = responseBuilder.AddField("No spoilers allowed!", "Tagged **or** Untagged!");
            }

            // Compile the list of allowed untagged spoilers
            if (untaggedAllowed)
            {
                StringBuilder untaggedList = new StringBuilder();

                foreach (string s in booksUntagged)
                {
                    untaggedList.Append("- " + s + "\n");
                }

                untaggedList.Length = untaggedList.Length - 1;
                responseBuilder = responseBuilder.AddField("Untagged spoilers for the following books are allowed:", untaggedList.ToString());
            }

            // Compile the list of allowed tagged spoilers
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

            // Build and post the embed
            await ctx.Channel.SendMessageAsync(embed: responseBuilder.Build()).ConfigureAwait(false);
        }

        // Post an embed containing the spoiler policy for the current channel
        private async Task PostPolicyForBook(CommandContext ctx, string bookName)
        {
            // Check to see if the book exists, and get the full name
            if (Bot.masterPolicy.TryGetBookName(bookName, out string cleanedBookName))
            {
                DiscordEmbedBuilder responseBuilder = new DiscordEmbedBuilder();

                responseBuilder.Title = "Spoiler policy for **" + cleanedBookName + "**";

                // Get lists of channels that tagged/untagged discussion is allowed in
                Bot.masterPolicy.TryGetChannelsFull(cleanedBookName, out var channelsUntagged);
                Bot.masterPolicy.TryGetChannelsTagged(cleanedBookName, out var channelsTagged);

                bool untaggedAllowed = channelsUntagged.Count > 0;
                bool taggedAllowed = channelsTagged.Count > 0;

                // If there's no channels where discussion is allowed, show a shorter message
                if (!untaggedAllowed && !taggedAllowed)
                {
                    responseBuilder = responseBuilder.AddField("**No discussion** of this book is allowed on this server.", "Tagged **or** Untagged!");
                }

                // Compile a list of channels where full/untagged discussion is allowed
                if (untaggedAllowed)
                {
                    StringBuilder untaggedList = new StringBuilder();

                    // Include a pointer indicating the current channel
                    foreach (string s in channelsUntagged)
                    {
                        untaggedList.Append("#" + s);
                        if (s.Equals(ctx.Channel.Name)) untaggedList.Append(" **<---You are here**");
                        untaggedList.Append("\n");
                    }

                    untaggedList.Length = untaggedList.Length - 1;
                    responseBuilder = responseBuilder.AddField("Untagged discussion is allowed in these channels:", untaggedList.ToString());
                }

                // Compile a list of channels where full/untagged discussion is allowed
                if (taggedAllowed)
                {
                    StringBuilder taggedList = new StringBuilder();

                    // Include a pointer indicating the current channel
                    foreach (string s in channelsTagged)
                    {
                        taggedList.Append("#" + s);
                        if (s.Equals(ctx.Channel.Name)) taggedList.Append(" **<---You are here**");
                        taggedList.Append("\n");
                    }

                    taggedList.Length = taggedList.Length - 1;
                    responseBuilder = responseBuilder.AddField("*Tagged* discussion is allowed in these channels:", taggedList.ToString());
                }

                // Post the embedded message
                await ctx.Channel.SendMessageAsync(embed: responseBuilder.Build()).ConfigureAwait(false);
            }
            else
            {
                // If we didn't recognize the book they requested
                await ctx.Channel.SendMessageAsync("I'm sorry, I don't recognize that title.");
            }
        }
    }
}
