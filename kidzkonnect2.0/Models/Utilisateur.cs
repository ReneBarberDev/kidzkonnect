using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace kidzkonnect2._0.Models
{
    public partial class Utilisateur
    {
        public Utilisateur()
        {
            Activites = new HashSet<Activite>();
            Notifications = new HashSet<Notification>();
            ProfilEnfants = new HashSet<ProfilEnfant>();
            TracesUtilisateurs = new HashSet<TracesUtilisateur>();
            UserPicture = string.Empty;
        }

        public class LoginViewModel
        {
            [Required]
            [EmailAddress]
            public string Courriel { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public int IdUtilisateur { get; set; }

        [Required(ErrorMessage = "*")]
        public string? Nom { get; set; }

        [Required(ErrorMessage = "*")]
        public string? Prenom { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Date)]
        public DateTime? DateNaissance { get; set; }

        [Required(ErrorMessage = "*")]
        public string? Adresse { get; set; }

        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Courriel { get; set; }

        [Required(ErrorMessage = "*")]
        public string? Password { get; set; }
        public string? EtatConnexion { get; set; }
        public string? UserPicture { get; set; }
        public virtual ICollection<Activite> Activites { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<ProfilEnfant> ProfilEnfants { get; set; }
        public virtual ICollection<TracesUtilisateur> TracesUtilisateurs { get; set; }
    }
}
