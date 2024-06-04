using System;
using System.Collections.Generic;

namespace kidzkonnect2._0.Models
{
    public partial class Messagekk
    {
        public int IdMessage { get; set; }
        public int? IdProfilReceveur { get; set; }
        public int? IdProfilEnvoyeur { get; set; }
        public string? Contenu { get; set; }
        public DateTime? DateEnvoi { get; set; }
        public bool? EtatLecture { get; set; }

        public virtual ProfilEnfant? IdProfilEnvoyeurNavigation { get; set; }
        public virtual ProfilEnfant? IdProfilReceveurNavigation { get; set; }
    }
}
