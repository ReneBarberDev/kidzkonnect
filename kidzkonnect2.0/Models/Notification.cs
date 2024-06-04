using System;
using System.Collections.Generic;

namespace kidzkonnect2._0.Models
{
    public partial class Notification
    {
        public int IdNotification { get; set; }
        public int? IdUtilisateur { get; set; }
        public string? Message { get; set; }
        public DateTime? DateEnvoi { get; set; }
        public bool? EtatLecture { get; set; }
        public int? IdDemande { get; set; }
        public virtual Utilisateur? IdUtilisateurNavigation { get; set; }
    }
}
