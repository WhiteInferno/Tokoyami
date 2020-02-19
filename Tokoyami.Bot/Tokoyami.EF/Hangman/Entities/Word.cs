using System.ComponentModel.DataAnnotations.Schema;

namespace Tokoyami.EF.Hangman.Entities
{
    [Table("Word", Schema = "hangman")]
    public class Word
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }
    }
}
