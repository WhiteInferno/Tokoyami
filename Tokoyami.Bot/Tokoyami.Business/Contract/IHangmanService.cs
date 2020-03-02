using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.EF.Hangman.Entities;

namespace Tokoyami.Business.Contract
{
    public interface IHangmanService
    {
        Task Create(string word);
        Word GetRandom();
        Task Remove(string description);
        Task<IEnumerable<Word>> Get();
    }
}
