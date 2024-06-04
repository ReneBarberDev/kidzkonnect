using System;
using System.Collections.Generic;

namespace kidzkonnect2._0.Models
{
    public partial class ProfilEnfant
    {
        public ProfilEnfant()
        {
            ConnectionIdProfilEnvoyeurNavigations = new HashSet<Connection>();
            ConnectionIdProfilReceveurNavigations = new HashSet<Connection>();
            MessageIdProfilEnvoyeurNavigations = new HashSet<Messagekk>();
            MessageIdProfilReceveurNavigations = new HashSet<Messagekk>();
            Participations = new HashSet<Participation>();
        }
        public int IdProfil { get; set; }
        public int? IdUtilisateur { get; set; }
        public int? AvatarFk { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public DateTime? DateNaissance { get; set; }
        public string? Genre { get; set; }
        public string? Interets { get; set; }
        public string? Defis { get; set; }
        public string? OptionsPriv { get; set; }
        public virtual Avatar? AvatarFkNavigation { get; set; }
        public virtual Utilisateur? IdUtilisateurNavigation { get; set; }
        public virtual ICollection<Connection> ConnectionIdProfilEnvoyeurNavigations { get; set; }
        public virtual ICollection<Connection> ConnectionIdProfilReceveurNavigations { get; set; }
        public virtual ICollection<Messagekk> MessageIdProfilEnvoyeurNavigations { get; set; }
        public virtual ICollection<Messagekk> MessageIdProfilReceveurNavigations { get; set; }
        public virtual ICollection<Participation> Participations { get; set; }
    }
}
