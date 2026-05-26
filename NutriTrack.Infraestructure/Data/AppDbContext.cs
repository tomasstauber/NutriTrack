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
        }
    }
}