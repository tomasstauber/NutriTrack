using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Core.Entities;

namespace NutriTrack.Infraestructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<RegistroPeso> RegistrosPeso { get; set; }
        public DbSet<Rodeo> Rodeos { get; set; }
        public DbSet<Animal> Animales { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegistroPeso>(entity =>
            {
                entity.ToTable("control_de_peso");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_control_de_peso").ValueGeneratedOnAdd();
                entity.Property(e => e.FechaPesaje).HasColumnName("fecha_pesaje");
                entity.Property(e => e.PesoKg).HasColumnName("peso_kg");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.IdAnimal).HasColumnName("id_animal");
            });

            modelBuilder.Entity<Rodeo>(entity =>
            {
                entity.ToTable("rodeo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_rodeo").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.HasIndex(e => e.Nombre).IsUnique();
            });


            modelBuilder.Entity<Animal>(entity =>
            {
                entity.ToTable("animal");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_animal").ValueGeneratedOnAdd();
                entity.Property(e => e.CaravanaCuig).HasColumnName("caravana_cuig");
                entity.Property(e => e.CaravanaNroManejo).HasColumnName("caravana_nro_manejo");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.RodeoId).HasColumnName("id_rodeo");
                entity.HasOne(a => a.Rodeo)
                      .WithMany(r => r.Animales)
                      .HasForeignKey(a => a.RodeoId)
                      .OnDelete(DeleteBehavior.SetNull);
            }
            );
        }
    }
}


