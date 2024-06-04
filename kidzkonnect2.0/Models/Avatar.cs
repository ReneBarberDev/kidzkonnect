using System;
using System.Collections.Generic;

namespace kidzkonnect2._0.Models
{
    public partial class Avatar
    {
        public Avatar()
        {
            ProfilEnfants = new HashSet<ProfilEnfant>();
        }
        public int IdAvatar { get; set; }
        public string? NomAvatar { get; set; }
        public string? ImagePath { get; set; } 

        public virtual ICollection<ProfilEnfant> ProfilEnfants { get; set; }
    }
}
