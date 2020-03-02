using System;
using System.Collections.Generic;
using System.Text;
using Tokoyami.Bot.Common;
using Tokoyami.EF.Hangman.Entities;

namespace Tokoyami.Bot.Dto
{
    public class HangmanDto
    {

        public HangmanDto()
        {
            State = HangmanState.STOPPED;
            CurrentWord = null;
            Letters = new List<char>();
            FoundLetters = new List<char>();
            WrongLetters = new List<char>();
            Errors = 0;
            Found = false;
            TempWord = string.Empty;
        }

        public HangmanState State { get; set; }

        public Word CurrentWord { get; set; }

        public IEnumerable<char> Letters { get; set; }

        public IEnumerable<char> FoundLetters { get; set; }

        public IEnumerable<char> WrongLetters { get; set; }

        public int Errors { get; set; }

        public bool Found { get; set; }

        public string TempWord { get; set; }
    }
}
