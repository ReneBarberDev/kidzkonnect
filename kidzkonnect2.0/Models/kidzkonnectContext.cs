using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace kidzkonnect2._0.Models
{
    public partial class kidzkonnectContext : DbContext
    {
        public kidzkonnectContext()
        {
        }

        public kidzkonnectContext(DbContextOptions<kidzkonnectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Activite> Activites { get; set; } = null!;
        public virtual DbSet<Avatar> Avatars { get; set; } = null!;
        public virtual DbSet<Connection> Connections { get; set; } = null!;
        public virtual DbSet<Defi> Defis { get; set; } = null!;
        public virtual DbSet<Interet> Interets { get; set; } = null!;
        public virtual DbSet<Messagekk> Messages { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Participation> Participations { get; set; } = null!;
        public virtual DbSet<ProfilEnfant> ProfilEnfants { get; set; } = null!;
        public virtual DbSet<TracesUtilisateur> TracesUtilisateurs { get; set; } = null!;
        public virtual DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //            //if (!optionsBuilder.IsConfigured)
            //            {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            //                optionsBuilder.UseSqlServer("Server=LAPTOP;Database=kidzkonnect;Trusted_Connection=True;TrustServerCertificate=True;");
            //  
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activite>(entity =>
            {
                entity.HasKey(e => e.IdActivite)
                    .HasName("PK__Activite__C34866D49083F506");

                entity.ToTable("Activite");

                entity.Property(e => e.IdActivite).HasColumnName("idActivite");

                entity.Property(e => e.DateActivite)
                    .HasColumnType("date")
                    .HasColumnName("dateActivite");

                entity.Property(e => e.DescriptionActivite)
                    .HasMaxLength(255)
                    .HasColumnName("descriptionActivite");

                entity.Property(e => e.DetailsAdditionnel)
                    .HasMaxLength(255)
                    .HasColumnName("detailsAdditionnel");

                entity.Property(e => e.Emplacement)
                    .HasMaxLength(255)
                    .HasColumnName("emplacement");

                entity.Property(e => e.IdUtilisateurCreateur).HasColumnName("idUtilisateurCreateur");

                entity.Property(e => e.TitreActivite)
                    .HasMaxLength(255)
                    .HasColumnName("titreActivite");

                entity.HasOne(d => d.IdUtilisateurCreateurNavigation)
                    .WithMany(p => p.Activites)
                    .HasForeignKey(d => d.IdUtilisateurCreateur)
                    .HasConstraintName("FK__Activite__idUtil__45F365D3");
            });

            modelBuilder.Entity<Avatar>(entity =>
            {
                entity.HasKey(e => e.IdAvatar)
                    .HasName("PK__Avatar__AE238FAD792591B0");

                entity.ToTable("Avatar");

                entity.Property(e => e.IdAvatar).HasColumnName("idAvatar");

                entity.Property(e => e.NomAvatar)
                    .HasMaxLength(255)
                    .HasColumnName("nomAvatar");

                entity.Property(e => e.ImagePath)
                   .HasMaxLength(255)
                   .HasColumnName("imagePath");
            });

            modelBuilder.Entity<Connection>(entity =>
            {
                entity.HasKey(e => e.IdDemande)
                    .HasName("PK__Connecti__8CE9A8CA2B81AD2F");

                entity.ToTable("Connection");

                entity.Property(e => e.IdDemande).HasColumnName("idDemande");

                entity.Property(e => e.EtatConnexion)
                    .HasMaxLength(50)
                    .HasColumnName("etatConnexion");

                entity.Property(e => e.IdProfilEnvoyeur).HasColumnName("idProfilEnvoyeur");

                entity.Property(e => e.IdProfilReceveur).HasColumnName("idProfilReceveur");

                entity.HasOne(d => d.IdProfilEnvoyeurNavigation)
                    .WithMany(p => p.ConnectionIdProfilEnvoyeurNavigations)
                    .HasForeignKey(d => d.IdProfilEnvoyeur)
                    .HasConstraintName("FK__Connectio__idPro__5441852A");

                entity.HasOne(d => d.IdProfilReceveurNavigation)
                    .WithMany(p => p.ConnectionIdProfilReceveurNavigations)
                    .HasForeignKey(d => d.IdProfilReceveur)
                    .HasConstraintName("FK__Connectio__idPro__5535A963");
            });

            modelBuilder.Entity<Defi>(entity =>
            {
                entity.HasKey(e => e.IdDefi)
                    .HasName("PK__Defi__DFA077A268D641DE");

                entity.ToTable("Defi");

                entity.Property(e => e.IdDefi).HasColumnName("idDefi");

                entity.Property(e => e.NomDefi)
                    .HasMaxLength(255)
                    .HasColumnName("nomDefi");
            });

            modelBuilder.Entity<Interet>(entity =>
            {
                entity.HasKey(e => e.IdInteret)
                    .HasName("PK__Interet__650CDE94CFEA0216");

                entity.ToTable("Interet");

                entity.Property(e => e.IdInteret).HasColumnName("idInteret");

                entity.Property(e => e.NomInteret)
                    .HasMaxLength(255)
                    .HasColumnName("nomInteret");
            });

            modelBuilder.Entity<Messagekk>(entity =>
            {
                entity.HasKey(e => e.IdMessage)
                    .HasName("PK__Message__8D0E911DABFBE231");

                entity.ToTable("Message");

                entity.Property(e => e.IdMessage).HasColumnName("idMessage");

                entity.Property(e => e.Contenu).HasColumnName("contenu");

                entity.Property(e => e.DateEnvoi)
                    .HasColumnType("date")
                    .HasColumnName("dateEnvoi");

                entity.Property(e => e.EtatLecture).HasColumnName("etatLecture");

                entity.Property(e => e.IdProfilEnvoyeur).HasColumnName("idProfilEnvoyeur");

                entity.Property(e => e.IdProfilReceveur).HasColumnName("idProfilReceveur");

                entity.HasOne(d => d.IdProfilEnvoyeurNavigation)
                    .WithMany(p => p.MessageIdProfilEnvoyeurNavigations)
                    .HasForeignKey(d => d.IdProfilEnvoyeur)
                    .HasConstraintName("FK__Message__idProfi__4D94879B");

                entity.HasOne(d => d.IdProfilReceveurNavigation)
                    .WithMany(p => p.MessageIdProfilReceveurNavigations)
                    .HasForeignKey(d => d.IdProfilReceveur)
                    .HasConstraintName("FK__Message__idProfi__4CA06362");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.IdNotification)
                    .HasName("PK__Notifica__22C02321F4240796");

                entity.ToTable("Notification");

                entity.Property(e => e.IdNotification).HasColumnName("idNotification");

                entity.Property(e => e.DateEnvoi)
                    .HasColumnType("date")
                    .HasColumnName("dateEnvoi");

                entity.Property(e => e.EtatLecture).HasColumnName("etatLecture");

                entity.Property(e => e.IdUtilisateur).HasColumnName("idUtilisateur");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.HasOne(d => d.IdUtilisateurNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.IdUtilisateur)
                    .HasConstraintName("FK__Notificat__idUti__5070F446");
            });

            modelBuilder.Entity<Participation>(entity =>
            {
                entity.HasKey(e => e.IdParticipation)
                    .HasName("PK__Particip__5DD3D7E564557F28");

                entity.ToTable("Participation");

                entity.Property(e => e.IdParticipation).HasColumnName("idParticipation");

                entity.Property(e => e.IdActivite).HasColumnName("idActivite");

                entity.Property(e => e.IdProfil).HasColumnName("idProfil");

                entity.HasOne(d => d.IdActiviteNavigation)
                    .WithMany(p => p.Participations)
                    .HasForeignKey(d => d.IdActivite)
                    .HasConstraintName("FK__Participa__idAct__48CFD27E");

                entity.HasOne(d => d.IdProfilNavigation)
                    .WithMany(p => p.Participations)
                    .HasForeignKey(d => d.IdProfil)
                    .HasConstraintName("FK__Participa__idPro__49C3F6B7");
            });

            modelBuilder.Entity<ProfilEnfant>(entity =>
            {
                entity.HasKey(e => e.IdProfil)
                    .HasName("PK__ProfilEn__2389837B0924946F");

                entity.ToTable("ProfilEnfant");

                entity.Property(e => e.IdProfil).HasColumnName("idProfil");

                entity.Property(e => e.AvatarFk).HasColumnName("avatarFK");

                entity.Property(e => e.DateNaissance)
                    .HasColumnType("date")
                    .HasColumnName("dateNaissance");

                entity.Property(e => e.Defis)
                    .HasMaxLength(255)
                    .HasColumnName("defis");

                entity.Property(e => e.Genre)
                    .HasMaxLength(50)
                    .HasColumnName("genre");

                entity.Property(e => e.IdUtilisateur).HasColumnName("idUtilisateur");

                entity.Property(e => e.Interets)
                    .HasMaxLength(255)
                    .HasColumnName("interets");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom");

                entity.Property(e => e.OptionsPriv)
                    .HasMaxLength(50)
                    .HasColumnName("optionsPriv");

                entity.Property(e => e.Prenom)
                    .HasMaxLength(255)
                    .HasColumnName("prenom");

                entity.HasOne(d => d.AvatarFkNavigation)
                    .WithMany(p => p.ProfilEnfants)
                    .HasForeignKey(d => d.AvatarFk)
                    .HasConstraintName("FK__ProfilEnf__avata__4316F928");

                entity.HasOne(d => d.IdUtilisateurNavigation)
                    .WithMany(p => p.ProfilEnfants)
                    .HasForeignKey(d => d.IdUtilisateur)
                    .HasConstraintName("FK__ProfilEnf__idUti__4222D4EF");
            });

            modelBuilder.Entity<TracesUtilisateur>(entity =>
            {
                entity.HasKey(e => e.IdTrace)
                    .HasName("PK__tracesUt__02936B8604001A89");

                entity.ToTable("tracesUtilisateur");

                entity.Property(e => e.IdTrace).HasColumnName("idTrace");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IdUtilisateur).HasColumnName("idUtilisateur");

                entity.HasOne(d => d.IdUtilisateurNavigation)
                    .WithMany(p => p.TracesUtilisateurs)
                    .HasForeignKey(d => d.IdUtilisateur)
                    .HasConstraintName("FK__tracesUti__idUti__5812160E");
            });

            modelBuilder.Entity<Utilisateur>(entity =>
            {
                entity.HasKey(e => e.IdUtilisateur)
                    .HasName("PK__Utilisat__5366DB194CC12306");

                entity.ToTable("Utilisateur");

                entity.Property(e => e.IdUtilisateur).HasColumnName("idUtilisateur");

                entity.Property(e => e.Adresse)
                    .HasMaxLength(255)
                    .HasColumnName("adresse");

                entity.Property(e => e.Courriel)
                    .HasMaxLength(255)
                    .HasColumnName("courriel");

                entity.Property(e => e.DateNaissance)
                    .HasColumnType("date")
                    .HasColumnName("dateNaissance");

                entity.Property(e => e.EtatConnexion)
                    .HasMaxLength(50)
                    .HasColumnName("etatConnexion");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Prenom)
                    .HasMaxLength(255)
                    .HasColumnName("prenom");

                entity.Property(e => e.UserPicture)
                    .HasMaxLength(255) // Adjust the maximum length as needed
                    .HasColumnName("userPicture");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
