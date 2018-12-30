﻿using DiscordButlerBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using DiscordButlerBot.Commands.CommandCompoments;

namespace DiscordButlerBot.Commands
{
    public class TeamMakerCommands : CommandBase
    {
        [Command("MakeTeams")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task MakeTeams(string numOfTeams = ""){
            int numberOFTeams = 0;
            if (Int32.TryParse(numOfTeams, out numberOFTeams))
            {
                Config.teamMakerInfo.numberOFTeams_ = numberOFTeams;

                var callingUser = Context.User as IGuildUser;                
                var guild = Context.Client.Guilds.FirstOrDefault(g => g.Id == Config.bot.serverID);
                var voiceChannelUserIn = guild.Channels.FirstOrDefault(c => c.Id == callingUser.VoiceChannel.Id);

                //Populate users
                Config.teamMakerInfo.PopulateUsersInVoice(voiceChannelUserIn.Users);
                
                //check if there is enough users for the number of teams
                if (Config.teamMakerInfo.guildUsersInVoice_.Count >= numberOFTeams)
                {
                    //Output and list
                    string msg = "Master, here is your list of users that will be place into teams: \n";
                    EmbedBuilder embed = Config.teamMakerInfo.ListUsersInVoiceEmbed();
                    embed.WithTitle("Members to be in teams");
                    embed.WithFooter("Would you like to \"!MakeRandom\" the teams, or \"!exclude #\" a user?");
                    Config.teamMakerInfo.currentStage_ = TeamMakingStages.listing;

                    await Context.Channel.SendMessageAsync(msg, false, embed);
                }
                else {
                    await Context.Channel.SendMessageAsync("There are far too few users for the number of teams you like master!");
                }
                         
            }
            else {
                await Context.Channel.SendMessageAsync(String.Format("Sorry master {0}, i need to know the number of teams first. (!MakeTeams #)", Context.User.Mention));
            }            
        }

        [Command("exclude")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task ExcludeUser(string idx="") {           

            if (Config.teamMakerInfo.currentStage_ == TeamMakingStages.listing)
            {
                int index = 0;
                if (Int32.TryParse(idx, out index))
                {
                    index--;

                    var removingUser = Config.teamMakerInfo.guildUsersInVoice_.ElementAt(index);

                    if (removingUser == null)
                    {
                        await Context.Channel.SendMessageAsync(String.Format("Sorry master {0}, could not find the user based on the number you gave.", Context.User.Mention ));
                    }
                    else
                    {
                        if (Config.teamMakerInfo.guildUsersInVoice_.Count - 1 >= Config.teamMakerInfo.numberOFTeams_)
                        {
                            Config.teamMakerInfo.guildUsersInVoice_.Remove(removingUser);
                            string name = removingUser.Username;

                            if (removingUser.Nickname != "" && removingUser.Nickname != null)
                            {
                                name += " ( " + removingUser.Nickname + " )" + "\n";
                            }

                            string msg = String.Format("I have removed \"{0}\" from the list. The new list is now: \n", name);
                            EmbedBuilder embed = Config.teamMakerInfo.ListUsersInVoiceEmbed();
                            embed.WithFooter("Would you like to \"!MakeRandom\" the teams, or again \"!exclude #\" a user?");
                            await Context.Channel.SendMessageAsync(msg, false, embed);
                        }
                        else {
                            await Context.Channel.SendMessageAsync(String.Format("Master {0}, there are now too few users to be in the teams you like now. Remake (!MakeTeams #).", Context.User.Mention));
                            Config.teamMakerInfo.currentStage_ = TeamMakingStages.empty;
                        }
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync(String.Format("Sorry master {0}, you need to give me a valid number based on my list (!exclude #)", Context.User.Mention));
                }
            }
        }
        [Command("makerandom")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task MakeRandom() {
            if (Config.teamMakerInfo.currentStage_ == TeamMakingStages.listing || Config.teamMakerInfo.currentStage_ == TeamMakingStages.move || Config.teamMakerInfo.currentStage_ == TeamMakingStages.made) {

                Config.teamMakerInfo.ShuffleUsersInVoice();
                Config.teamMakerInfo.AssignTeams();

                string msg = "";
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithColor(200, 0, 0);
                embed.WithThumbnailUrl("https://cdn0.iconfinder.com/data/icons/sea-nautical-pirate-maritime/35/9-512.png");
                foreach (var team in Config.teamMakerInfo.teams_) {
                    msg += team.GetStringMembersFormatted();

                }
                embed.WithDescription(msg);

                Config.teamMakerInfo.currentStage_ = TeamMakingStages.made;

                if (Config.voiceChannelIds.Count >= Config.teamMakerInfo.teams_.Count) {
                    embed.WithFooter( "Would you like to \"!moveteams\" to different voice channels?");
                    Config.teamMakerInfo.currentStage_ = TeamMakingStages.move;
                }
                

                await Context.Channel.SendMessageAsync("These are the teams: \n", false, embed);
            }

        }
        [Command("moveteams")]
        [RequireUserPermission(Discord.GuildPermission.MoveMembers)]
        public async Task MoveTeams() {
            if (Config.teamMakerInfo.currentStage_ == TeamMakingStages.move) {
                int counter = 0;
                foreach (var team in Config.teamMakerInfo.teams_) {
                    await team.MoveUsers(Config.voiceChannelIds[counter]);
                    counter++;
                }
                await Context.Channel.SendMessageAsync("Teams are now in their channels.");
            }
        }
    }
}
