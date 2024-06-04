using System;
using System.Collections.Generic;

namespace kidzkonnect2._0.Models
{
    public partial class Participation
    {
        public int IdParticipation { get; set; }
        public int? IdActivite { get; set; }
        public int? IdProfil { get; set; }
        public virtual Activite? IdActiviteNavigation { get; set; }
        public virtual ProfilEnfant? IdProfilNavigation { get; set; }
    }
}
