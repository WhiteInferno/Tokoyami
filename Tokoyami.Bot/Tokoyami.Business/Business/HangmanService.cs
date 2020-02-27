using System;
using System.Linq;
using System.Threading.Tasks;
using Tokoyami.Business.Contract;
using Tokoyami.Context;
using Tokoyami.Context.Configuration;
using Tokoyami.EF.Hangman.Entities;

namespace Tokoyami.Business.Business
{
    public class HangmanService : IHangmanService
    {
        private readonly UnitOfWork unitOfWork;

        private readonly TokoyamiRepository<Word> wordRepository;

        public HangmanService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            wordRepository = unitOfWork.wordRepository;
        }

        public async Task Create(string word)
        {
            Word entity = new Word() { Descripcion = word };
            this.wordRepository.Insert(entity);
            await this.unitOfWork.Save();
        }

        public Word GetRandom()
        {
            var random = new Random();
            var words = this.wordRepository.Get().ToList();
            var index = random.Next(words.Count);
            return words[index];
        }

        public async Task Remove(string description)
        {
            var word = this.wordRepository.Get().Where(x => x.Descripcion.ToUpper() == description.ToUpper()).FirstOrDefault();
            if (word != null)
            {
                this.wordRepository.Delete(word.Id);
                await this.unitOfWork.Save();
            }
        }
    }
}
