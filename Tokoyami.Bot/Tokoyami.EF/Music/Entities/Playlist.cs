using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tokoyami.EF.Music
{
    [Table("Playlist", Schema = "music")]
    public class Playlist
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string Author { get; set; }

        public string Urls { get; set; }
    }
}
