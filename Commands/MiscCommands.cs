﻿using DiscordButlerBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;


namespace DiscordButlerBot.Commands
{
    public class MiscCommands : CommandBase
    {
        //Owner Only commands----------------------------------------------------------

        [Command("ownercommands")]
        [RequireOwner]
        public async Task OwnerCommands()
        {
            string msg =
                "\n These are your commands father " + Context.User.Mention + " : \n" +
                "```" +
                "!setTopicLength\n\n" +
                "!setTopicLength \n\n" +
                "!addThisVChannel \n\n" +
                "!removeThisVChannel \n\n" +
                "```";
            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("welcome")]
        [RequireOwner]
        public async Task Welcome(string s = "") {

            await Context.Channel.SendMessageAsync("Good to be back master " + Context.User.Mention + " <3");

                   
        }
        [Command("setTopicLength")]
        [RequireOwner]
        public async Task SetTopicLength(string num = "") {
            int newLength = 0;
            var guildUser = Context.User as IGuildUser;
            if (Int32.TryParse(num, out newLength))
            {
                Config.serverData[guildUser.GuildId].maxTopicNameLength_ = newLength;
                Config.SaveServerData();
                await Context.Channel.SendMessageAsync(String.Format("I have set the topic length to {0} master.", Config.serverData[guildUser.GuildId].maxTopicNameLength_));
            }
            else {
                await Context.Channel.SendMessageAsync("Could not set topic new length Parse failed");
            }

        }
        //[Command("setRemoveFirstWord")]
        //[RequireOwner]
        //public async Task SetBoolFirstWord(string p = "") {
        //    var guildUser = Context.User as IGuildUser;
        //    if (p.ToLower().Contains("true"))
        //    {
        //        Config.serverData[guildUser.GuildId].removeFirstWord_ = true;
        //        await Context.Channel.SendMessageAsync("Removal of first word in topic vc is now " + Config.serverData[guildUser.GuildId].removeFirstWord_);
        //        Config.SaveServerData();
        //    }
        //    else if (p.ToLower().Contains("false"))
        //    {
        //        Config.serverData[guildUser.GuildId].removeFirstWord_ = false;
        //        await Context.Channel.SendMessageAsync("Removal of first word in topic vc is now " + Config.serverData[guildUser.GuildId].removeFirstWord_);
        //        Config.SaveServerData();
        //    }
        //    else {
        //        await Context.Channel.SendMessageAsync("Could not set bool. \"!setRemoveFirstWord (bool)\"");
        //    }            
        //}
        [Command("addThisVChannel")]
        [RequireOwner]
        public async Task AddChannel() {
            var guildUser = Context.User as IGuildUser;
            var voiceChannelUserIn = guildUser.VoiceChannel;
            if (voiceChannelUserIn != null)
            {
                if (Config.serverData[guildUser.GuildId].voiceChannelIds_.Contains(voiceChannelUserIn.Id))
                {
                    await Context.Channel.SendMessageAsync("This channel is already in my list master");
                }
                else {
                    Config.serverData[guildUser.GuildId].voiceChannelIds_.Add(voiceChannelUserIn.Id);
                    await Context.Channel.SendMessageAsync("I have added " + voiceChannelUserIn.Name + " voice channel into my list");
                    Config.SaveServerData();                    
                }
            }
            else {
                await Context.Channel.SendMessageAsync("Sorry master you need to be in a voice channel for this work");
            }
        }
        [Command("removeThisVChannel")]
        [RequireOwner]
        public async Task RemoveChannel()
        {
            var guildUser = Context.User as IGuildUser;
            var voiceChannelUserIn = guildUser.VoiceChannel;
            if (voiceChannelUserIn != null)
            {
                if (Config.serverData[guildUser.GuildId].voiceChannelIds_.Contains(voiceChannelUserIn.Id))
                {
                    Config.serverData[guildUser.GuildId].voiceChannelIds_.Remove(voiceChannelUserIn.Id);
                    await Context.Channel.SendMessageAsync("I have removed " + voiceChannelUserIn.Name + " from my list");
                    Config.SaveServerData();
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Master the voice your in is already not in my list");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Sorry master you need to be in a voice channel for this work");
            }
        }

        //------------------------------------------------------------------

        [Command("commands")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task Commands() {
            string msg = 
                "\n These are your commands master " + Context.User.Mention + " : \n" +
                "```" +
                "!hi - Say hi.\n\n" +                
                "!getdrink (name) - Will give you the drink of your liking\n\n" +
                "!maketeams (#) - Orgainzes a team from voice chat, and can escort teams to different voice channels.\n\n" +
                "!topic (topic) - Adds a topic to the voice channel your currently in. LIMITED use 10mins\n\n" +
                "!rmtopic - Removes the topic to the voice channel your currently in. RATE LIMITED use 10mins\n\n" +
                "!listvoice - list all users in your current voice channel.\n\n" +
                "!stepout (@mention) - disconnects the target user from voice chat\n\n" +
                "!m (keyword) - Will give you picture based on word. Warning: chance of NSFW content!" +
                "```";
            await Context.Channel.SendMessageAsync(msg);
        }
        [Command("hi")]
        public async Task Speak()
        {
            var user = Context.User as IGuildUser;
            string username = Context.User.Username;

            await Context.Channel.SendMessageAsync(String.Format("Greetings master {0}.", GetName() + " <3"));

        }
        [Command("getDrink")]
        public async Task AskDrink([Remainder]string drink) {
            if (drink != "" && drink != null) {
                await Context.Channel.SendMessageAsync(String.Format("*Hands you a {0}*", drink));
            }
            else {
                await Context.Channel.SendMessageAsync(String.Format("Master {0}, what drink would you like? (!getdrink (name)) ", GetName()));
            }            
        }
        [Command("lewd")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task BadTimes()
        {
            var user = Context.User as IGuildUser;

            bool hasNickName = user.Nickname == null ? false : true;
            bool hasName = false;
            string username = user.Username.ToLower();

            if (hasNickName) {
                string nickname = user.Nickname.ToLower();
                hasName = nickname.Contains("nemu");
            }
                      
            if (!(username.Contains("nemu") || hasName))
            {
                await Context.Channel.SendMessageAsync("*slaps you* ");
            }
            else {
                await Context.Channel.SendMessageAsync(String.Format("AAAAHH~!! YES <3~~~ {0}", Context.User.Mention));
            }            
        }
        [Command("topic")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task SetVCTopic([Remainder] string newTopic = "") {
            var guildUser = Context.User as IGuildUser;
            var voiceChannelUserIn = guildUser.VoiceChannel;
            if (voiceChannelUserIn != null)
            {
                if (newTopic != null && newTopic != "")
                {
                    if (newTopic.Length <= Config.serverData[guildUser.GuildId].maxTopicNameLength_)
                    {
                        string vcName;

                        //remove first word if true
                        if (Config.serverData[guildUser.GuildId].removeFirstWord_ && Config.serverData[guildUser.GuildId].firstWordTitle_ == "")
                        {
                            vcName = RemoveChannelNameFirstWord(voiceChannelUserIn.Name, guildUser.GuildId);
                        }
                        else {
                            vcName = voiceChannelUserIn.Name;
                        }                       

                        string vcNameNoTopic = FuncHelpers.RemoveTopicBracket(ref vcName) == false ? voiceChannelUserIn.Name : vcName;

                        vcName += String.Format(" ({0})", newTopic);
                        await guildUser.VoiceChannel.ModifyAsync(x =>
                        {
                            x.Name = vcName;
                        });

                        FuncHelpers.AddTopicTracking(new VCTopicData(DateTime.Now, voiceChannelUserIn));

                        await Context.Channel.SendMessageAsync(String.Format("Master {0}, the topic is now set to \"{1}\" in {2}", 
                            guildUser.Mention, newTopic, String.Format("{0}{1}", Config.serverData[guildUser.GuildId].firstWordTitle_, vcNameNoTopic)));
                    }
                    else {
                        await Context.Channel.SendMessageAsync(String.Format("Sorry master {0}, the topic name is too long it needs to be less than {1} characters.", guildUser.Mention, Config.serverData[guildUser.GuildId].maxTopicNameLength_));
                    }
                }
                else
                {
                    await ClearVCTopic();
                }
            }
            else {
                await Context.Channel.SendMessageAsync(String.Format("Master {0}, you need to be in a voice channel to set a topic!", guildUser.Mention));
            }
        }
        [Command("rmtopic")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task ClearVCTopic(){
            
            var callingUser = Context.User as IGuildUser;
            var voiceChannelUserIn = callingUser.VoiceChannel;
            string vcName = voiceChannelUserIn.Name;
            if (voiceChannelUserIn != null)
            {
                if (FuncHelpers.RemoveTopicBracket(ref vcName))
                {
                    if (Config.serverData[callingUser.GuildId].removeFirstWord_) {
                        vcName = String.Format("{0}{1}", Config.serverData[callingUser.GuildId].firstWordTitle_, vcName);
                        Config.serverData[callingUser.GuildId].firstWordTitle_ = "";
                    }

                    await callingUser.VoiceChannel.ModifyAsync(x =>
                    {                        
                        x.Name = vcName;
                    });
                    await Context.Channel.SendMessageAsync( String.Format("Master {0}, topic was removed in {1}", callingUser.Mention, vcName));
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Master, no topic was set in " + vcName);
                }
            }
            else {
                await Context.Channel.SendMessageAsync(String.Format("Master {0}, you need to be in a voice channel to clear a topic", callingUser.Mention));
            }
        }
        [Command("stepout")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task KickUser(string mentionName = "") {
            if (mentionName != "")
            {
                var guildUser = Context.User as SocketGuildUser;
                var guild = Context.Guild;

                SocketGuildUser removingUser = null;
                foreach (var channel in guild.VoiceChannels)
                {
                    removingUser = channel.Users.FirstOrDefault(u => u.Mention.Substring(3).Contains(mentionName.Substring(2)));
                    if (removingUser != null)
                    {
                        break;
                    }
                }
                if (removingUser != null)
                {
                    var vc = await guildUser.Guild.CreateVoiceChannelAsync("Expelling...");
                    await removingUser.ModifyAsync(x => x.ChannelId = vc.Id);
                    await vc.DeleteAsync();
                    await Context.Channel.SendMessageAsync(String.Format("The user is now gone"));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(String.Format("Master {0}, I could not find the user. \"!stepout (@mention)\" ", Context.User.Mention));
                }
            }
            else {
                await Context.Channel.SendMessageAsync(String.Format("Sorry master {0}, you need to metion a name. \"!stepout (@mention)\" ", Context.User.Mention));
            }

        }
        [Command("listvoice")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task ListVoice()
        {
            var guildUser = Context.User as SocketGuildUser;
            var voiceChannelUserIn = guildUser.VoiceChannel;

            if (guildUser.VoiceChannel != null)
            {
                string listingReplyMsg = "";

                var users = voiceChannelUserIn.Users;
                string vcName = voiceChannelUserIn.Name;
                FuncHelpers.RemoveTopicBracket(ref vcName);
                EmbedBuilder em = new EmbedBuilder();
                em.WithTitle("Members in Voice " + vcName);
                em.WithColor(new Color(10, 10, 10));
                em.WithFooter("");
                foreach (var user in users)
                {

                    listingReplyMsg += "-" + user.Mention;
                    listingReplyMsg += "\n";
                }
                em.WithDescription(listingReplyMsg);
                await Context.Channel.SendMessageAsync("Master " + guildUser.Mention + ", here is your list: \n", false, em.Build());
            }
            else {
                await Context.Channel.SendMessageAsync(String.Format("Sorry master {0}, you need to be in a voice channel to use this command.", Context.User.Mention));
            }

        }
        [Command("meme")]
        public async Task GetMeme([Remainder] string keyword = "")
        {
            if (keyword == "")
            {
                await Context.Channel.SendMessageAsync("Please give me a keyword. \" !m (keyword) \" ");
            }
            else {
                using (Context.Channel.EnterTypingState()) {
                    string output = Run_Python("MemeGetter.py", keyword);
                    if (output == "")
                    {
                        await Context.Channel.SendMessageAsync("Sorry Master, Nothing Found :(");
                    }
                    else
                    {
                        EmbedBuilder embed = new EmbedBuilder();
                        embed.ImageUrl = output;
                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                }

            }                        
        }

        [Command("m")]
        public async Task GetMeme_m([Remainder] string keyword = "")
        {
            await GetMeme(keyword);
        }
    }
}
