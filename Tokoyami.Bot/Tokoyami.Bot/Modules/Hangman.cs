using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.Bot.Common;
using Tokoyami.Business.Business;
using Tokoyami.Business.Contract;
using Tokoyami.Context.Configuration;
using Tokoyami.EF.Hangman.Entities;

namespace Tokoyami.Bot.Modules
{
    public class Hangman : ModuleBase<SocketCommandContext>
    {
        private IHangmanService _service = new HangmanService(Program.UnitOfWork);

        [Command("hangman"), Alias("hm")]
        [Summary("::hm add word | ::hm delete word | ::hm start | ::hm stop")]
        public async Task ManageHangman(string comm = null, string val = "a")
        {
            comm = comm.ToLower();
            val = val.ToLower();
            var words = await _service.Get();

            switch (comm)
            {
                case "add":
                    if (words.Where(x => x.Descripcion == val).Count() > 0)
                    {
                        await ReplyAsync("But this word is already in there!");
                    }
                    else
                    {
                        if (val.Length < 25)
                        {
                            int count = 0;
                            for (int i = 1; i < val.Length; i++)
                            {
                                if (val[i - 1] == val[i])
                                {
                                    count++;
                                }
                            }

                            if (count >= val.Length - 1)
                            {
                                await ReplyAsync("Come on that's not a word, that's a keyboard smash!!");
                            }
                            else
                            {
                                await _service.Create(val);
                                await ReplyAsync("The word has been added successfully!");
                            }

                        }
                        else
                        {
                            await ReplyAsync("I didn't know such a long word existed...Can't add it!");
                        }
                    }
                    break;
                case "delete":
                    await _service.Remove(val);
                    break;
                case "start":
                    Program.HangmanState = HangmanState.STARTED;
                    Random r = new Random();
                    int ran = r.Next(0, words.Count());
                    Program.curWord = words.ElementAt(ran);
                    string st = "";
                    for (int i = 0; i < Program.curWord.Descripcion.Length; i++)
                    {
                        st += "?";
                    }
                    await ReplyAsync("Current word : " + st);

                    foreach (var v in Program.curWord.Descripcion)
                    {
                        if (!Program.letters.Contains(v))
                        {
                            Program.letters.Add(v);
                        }
                    }
                    break;
                case "stop":
                    Program.HangmanState = HangmanState.STOPPED;
                    Program.foundLetters.Clear();
                    Program.letters.Clear();
                    Program.wrongLetters.Clear();
                    Program.errors = 0;
                    await ReplyAsync("This game of hangman stopped!");
                    break;
                default:
                    if (comm == Program.curWord.Descripcion)
                    {
                        await ReplyAsync($"You guys won!");
                        Program.HangmanState = HangmanState.STOPPED;
                        Program.foundLetters.Clear();
                        Program.letters.Clear();
                        Program.wrongLetters.Clear();
                        Program.errors = 0;
                    }
                    break;
            }
        }

        [Command("hangman"), Alias("hm")]
        public async Task CharReceived(char val)
        {
            string wrongLetters = "Wrong : ";
            val = char.ToLower(val);
            if (Program.HangmanState == HangmanState.STARTED)
            {

                //User u = Program.UL[Context.User.Id];
                //if (!Program.Participants.Contains(u))
                //{
                //    Program.Participants.Add(u);
                //}

                string s = "";

                foreach (char c in Program.curWord.Descripcion)
                {
                    if (char.ToUpper(c) == char.ToUpper(val))
                    {
                        s += c;
                        Program.Found = true;
                        if (!Program.foundLetters.Contains(c))
                        {
                            Program.foundLetters.Add(c);
                        }
                    }
                    else
                    {

                        if (Program.foundLetters.Contains(c))
                        {
                            s += c;
                        }
                        else
                            s += "?";
                    }
                }

                Program.tempword = s;

                if (Program.Found == false && Program.errors < 10)
                {
                    if (!Program.wrongLetters.Contains(val))
                    {
                        Program.errors++;
                        Program.wrongLetters.Add(val);
                    }

                }

                Program.Found = false;
                string s2 = "";

                switch (Program.errors)
                {
                    case 1:
                        s2 = "|___";
                        break;

                    case 2:
                        s2 = "|\n|\n|\n|\n|\n|___";
                        break;

                    case 3:
                        s2 = "\n______\n|\n|\n|\n|\n|\n|___";
                        break;

                    case 4:
                        s2 = "\n______\n|  |\n|\n|\n|\n|\n|___";
                        break;

                    case 5:
                        s2 = "\n______\n|  |\n|  0\n|\n|\n|\n|___";
                        break;

                    case 6:
                        s2 = "\n______\n|  |\n|  0\n| /|\n|\n|\n|___";
                        break;

                    case 7:
                        s2 = "\n______\n|  |\n|  0\n| /|\\\n|\n|\n|___";
                        break;

                    case 8:
                        s2 = "\n______\n|  |\n|  0\n| /|\\\n| / \n|\n|___";
                        break;

                    case 9:
                        var messages = await Context.Channel.GetMessagesAsync(2).Flatten().First();

                        await Context.Channel.DeleteMessageAsync(messages);
                        s2 = "\n______\n|  |\n|  0\n| /|\\\n| / \\\n|\n|___";
                        await ReplyAsync("```" + wrongLetters + "\n\n" + s2 + "```\nYou guys lost! The word was " + Program.curWord);

                        Program.HangmanState = HangmanState.STOPPED;
                        Program.foundLetters.Clear();
                        Program.letters.Clear();
                        Program.wrongLetters.Clear();
                        Program.errors = 0;
                        break;
                }


                if (Program.HangmanState == HangmanState.STARTED)
                {
                    if (Program.errors > 0)
                    {

                        foreach (var v in Program.wrongLetters)
                        {
                            wrongLetters += v + " ";
                        }

                        var messages = await Context.Channel.GetMessagesAsync(2).Flatten().First();

                        await Context.Channel.DeleteMessageAsync(messages);

                        await ReplyAsync("```" + wrongLetters + "\n\n" + s2 + "``` \n\nThe word is : " + Program.tempword + "\n");
                    }
                    else
                        await ReplyAsync("\n\nThe word is : " + Program.tempword + "\n");

                    if (Program.foundLetters.Count == Program.letters.Count)
                    {
                        //int res = (int)(Program.letters.Count / 2) * Program.CookieCoef;
                        await ReplyAsync($"You guys won!");
                        //foreach (var v in Program.Participants)
                        //{
                        //    v.gainCookies(res);
                        //    Program.SaveUs(v);
                        //    //v.plays++;
                        //}
                        Program.HangmanState = HangmanState.STOPPED;
                        Program.foundLetters.Clear();
                        Program.letters.Clear();
                        Program.wrongLetters.Clear();
                        Program.errors = 0;
                    }
                }
            }
            else
            {
                await ReplyAsync("You can't play since no hangman game is started!\nUse '!hm start' for that!");
            }
        }
    }
}
