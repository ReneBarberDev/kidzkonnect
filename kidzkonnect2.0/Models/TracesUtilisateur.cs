using System;
using System.Collections.Generic;

namespace kidzkonnect2._0.Models
{
    public partial class TracesUtilisateur
    {
        public int IdTrace { get; set; }
        public int? IdUtilisateur { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }

        public virtual Utilisateur? IdUtilisateurNavigation { get; set; }
    }
}
