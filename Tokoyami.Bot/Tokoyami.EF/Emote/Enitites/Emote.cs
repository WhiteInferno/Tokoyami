using System.ComponentModel.DataAnnotations.Schema;

namespace Tokoyami.EF
{
    [Table("Emote",Schema ="discord")]
    public class Emote
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
