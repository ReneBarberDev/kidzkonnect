using System;
using System.Collections.Generic;

namespace kidzkonnect2._0.Models
{
    public partial class Activite
    {
        public Activite()
        {
            Participations = new HashSet<Participation>();
        }
        public int IdActivite { get; set; }
        public int? IdUtilisateurCreateur { get; set; }
        public string? TitreActivite { get; set; }
        public string? DescriptionActivite { get; set; }
        public DateTime? DateActivite { get; set; }
        public string? Emplacement { get; set; }
        public string? DetailsAdditionnel { get; set; }
        public virtual Utilisateur? IdUtilisateurCreateurNavigation { get; set; }
        public virtual ICollection<Participation> Participations { get; set; }
    }
}
