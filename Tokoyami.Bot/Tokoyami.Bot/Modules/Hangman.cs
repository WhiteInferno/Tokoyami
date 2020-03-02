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
        private readonly IHangmanService _service;

        public Hangman(UnitOfWork unitOfWork)
        {
            _service = new HangmanService(unitOfWork);
        }

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
                    Program.Hangman.State = HangmanState.STARTED;
                    Random r = new Random();
                    int ran = r.Next(0, words.Count());
                    Program.Hangman.CurrentWord = words.ElementAt(ran);
                    StringBuilder hiddenLetter = new StringBuilder();
                    for (int i = 0; i < Program.Hangman.CurrentWord.Descripcion.Length; i++)
                    {
                        hiddenLetter.Append("?");
                    }
                    await ReplyAsync($"Current word : {hiddenLetter.ToString()}");

                    foreach (var v in Program.Hangman.CurrentWord.Descripcion)
                    {
                        if (!Program.Hangman.Letters.Contains(v))
                        {
                            (Program.Hangman.Letters as List<char>).Add(v);
                        }
                    }
                    break;
                case "stop":
                    await ReplyAsync("This game of hangman stopped!");
                    ResetHangman();
                    break;
                default:
                    if (comm == Program.Hangman.CurrentWord.Descripcion)
                    {
                        await ReplyAsync($"You guys won!");
                        ResetHangman();
                    }
                    break;
            }
        }

        [Command("hangman"), Alias("hm")]
        public async Task CharReceived(char val)
        {
           StringBuilder wrongLetters = new StringBuilder("Wrong : ");
            val = char.ToLower(val);
            if (Program.Hangman.State == HangmanState.STARTED)
            {
                StringBuilder hiddenLetter = new StringBuilder(string.Empty);

                foreach (char c in Program.Hangman.CurrentWord.Descripcion)
                {
                    if (char.ToUpper(c) == char.ToUpper(val))
                    {
                        hiddenLetter.Append(c);
                        Program.Hangman.Found = true;
                        if (!Program.Hangman.FoundLetters.Contains(c))
                        {
                            (Program.Hangman.FoundLetters as List<char>).Add(c);
                        }
                    }
                    else
                    {
                        if (Program.Hangman.FoundLetters.Contains(c))
                            hiddenLetter.Append(c);
                        else
                            hiddenLetter.Append("?");
                    }
                }

                Program.Hangman.TempWord = hiddenLetter.ToString();

                if (Program.Hangman.Found == false && Program.Hangman.Errors < 10)
                {
                    if (!Program.Hangman.WrongLetters.Contains(val))
                    {
                        Program.Hangman.Errors++;
                        (Program.Hangman.WrongLetters as List<char>).Add(val);
                    }

                }

                Program.Hangman.Found = false;
                StringBuilder hangman = new StringBuilder(string.Empty);

                switch (Program.Hangman.Errors)
                {
                    case 1:
                        hangman.Append(TemplateMsj.HANGMAN_ATTEMPT_1);
                        break;

                    case 2:
                        hangman.Append(TemplateMsj.HANGMAN_ATTEMPT_2);
                        break;

                    case 3:
                        hangman.Append(TemplateMsj.HANGMAN_ATTEMPT_3);
                        break;

                    case 4:
                        hangman.Append(TemplateMsj.HANGMAN_ATTEMPT_4);
                        break;

                    case 5:
                        hangman.Append(TemplateMsj.HANGMAN_ATTEMPT_5);
                        break;

                    case 6:
                        hangman.Append(TemplateMsj.HANGMAN_ATTEMPT_6);
                        break;

                    case 7:
                        hangman.Append(TemplateMsj.HANGMAN_ATTEMPT_7);
                        break;

                    case 8:
                        hangman.Append(TemplateMsj.HANGMAN_ATTEMPT_8);
                        break;

                    case 9:
                        var messages = await Context.Channel.GetMessagesAsync(2).Flatten().ToList();

                        foreach (var item in messages)
                        {
                            await Context.Channel.DeleteMessageAsync(item);
                        }

                        hangman.Append(TemplateMsj.HANGMAN_ATTEMPT_9);
                        
                        await ReplyAsync($"```{wrongLetters.ToString()}\n\n{hangman.ToString()}```\nYou guys lost! The word was {Program.Hangman.CurrentWord}");
                        
                        ResetHangman();
                        break;
                }


                if (Program.Hangman.State == HangmanState.STARTED)
                {
                    if (Program.Hangman.Errors > 0)
                    {

                        foreach (var v in Program.Hangman.WrongLetters)
                        {
                            wrongLetters.Append(v + " ");
                        }

                        var messages = await Context.Channel.GetMessagesAsync(2).Flatten().ToList();

                        foreach(var item in messages)
                        {
                            await Context.Channel.DeleteMessageAsync(item);
                        }

                        await ReplyAsync($"```{wrongLetters.ToString()}\n\n{hangman.ToString()}``` \n\nThe word is : {Program.Hangman.TempWord}\n");
                    }
                    else
                        await ReplyAsync($"\n\nThe word is : {Program.Hangman.TempWord}\n");

                    if (Program.Hangman.FoundLetters.Count() == Program.Hangman.Letters.Count())
                    {
                        await ReplyAsync($"You guys won!");
                        ResetHangman();
                    }
                }
            }
            else
            {
                await ReplyAsync("You can't play since no hangman game is started!\nUse '::hm start' for that!");
            }
        }

        private void ResetHangman()
        {
            Program.Hangman.State = HangmanState.STOPPED;
            (Program.Hangman.FoundLetters as List<char>).Clear();
            (Program.Hangman.Letters as List<char>).Clear();
            (Program.Hangman.WrongLetters as List<char>).Clear();
            Program.Hangman.Errors = 0;
        }
    }
}
