using System;
using System.Collections.Generic;

namespace kidzkonnect2._0.Models
{
    public partial class Connection
    {
        public int IdDemande { get; set; }
        public int? IdProfilEnvoyeur { get; set; }
        public int? IdProfilReceveur { get; set; }
        public string? EtatConnexion { get; set; }
        public virtual ProfilEnfant? IdProfilEnvoyeurNavigation { get; set; }
        public virtual ProfilEnfant? IdProfilReceveurNavigation { get; set; }
    }
}
