using System.ComponentModel.DataAnnotations.Schema;

namespace Tokoyami.EF.Entities
{
    [Table("Words", Schema = "hangman")]
    public class Words
    {
        public int Id { get; set; }
        public string Word { get; set; }
    }
}
