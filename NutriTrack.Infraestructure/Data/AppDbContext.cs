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
        public DbSet<PlanAlimenticio> PlanesAlimenticios { get; set; }
        public DbSet<PlanAlimenticioDetalle> PlanAlimenticioDetalles { get; set; }
        public DbSet<PlanRodeoAsignacion> PlanRodeoAsignacions { get; set; }

        public DbSet<Rodeo> Rodeos { get; set; }
        public DbSet<Animal> Animales { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegistroPeso>(entity =>
            {
                entity.ToTable("controldepeso");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_control_de_peso").ValueGeneratedOnAdd();
                entity.Property(e => e.FechaPesaje).HasColumnName("fecha_pesaje");
                entity.Property(e => e.PesoKg).HasColumnName("peso_kg");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.IdAnimal).HasColumnName("id_animal");
            });
            
            modelBuilder.Entity<PlanAlimenticio>(entity =>
            {
                entity.ToTable("planalimenticio");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_plan_alimenticio").ValueGeneratedOnAdd();
                entity.Property(e => e.NombrePlan).HasColumnName("nombre_plan");
                entity.Property(e => e.Categoria).HasColumnName("categoria");
                entity.Property(e => e.PesoVivoInicialPromedio).HasColumnName("peso_vivo_inicial_promedio");
                entity.Property(e => e.PesoObjetivo).HasColumnName("peso_objetivo");
                entity.Property(e => e.GananciaPesoEsperada).HasColumnName("ganancia_peso_esperada");
                entity.Property(e => e.TipoAlimentacion).HasColumnName("tipo_alimentacion");
                entity.Property(e => e.TiempoAlimentacion).HasColumnName("tiempo_alimentacion");
                entity.Property(e => e.KgMsDiariaPorAnimal).HasColumnName("kg_ms_diaria_por_animal");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            });

            modelBuilder.Entity<PlanAlimenticioDetalle>(entity =>
            {
                entity.ToTable("planalimenticiodetalle");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_plan_alimenticio_detalle");
                entity.Property(e => e.PorcentajeInclusionMs).HasColumnName("porcentaje_inclusion_ms");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.IdPlanAlimenticio).HasColumnName("id_plan_alimenticio");
                entity.Property(e => e.IdIngrediente).HasColumnName("id_ingrediente");
                entity.HasOne(d => d.PlanAlimenticio)
                       .WithMany(p => p.Detalles)
                       .HasForeignKey(d => d.IdPlanAlimenticio)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PlanRodeoAsignacion>(entity =>
            {
                entity.ToTable("planrodeoasignacion");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_asignacion_rodeo");
                entity.Property(e => e.VigenciaDesde).HasColumnName("vigencia_desde");
                entity.Property(e => e.VigenciaHasta).HasColumnName("vigencia_hasta");
                entity.Property(e => e.Activo).HasColumnName("activo");
                entity.Property(e => e.IdPlanAlimenticio).HasColumnName("id_plan_alimenticio");
                entity.Property(e => e.IdRodeo).HasColumnName("id_rodeo");

                entity.HasOne(d => d.PlanAlimenticio)
                    .WithMany()
                    .HasForeignKey(d => d.IdPlanAlimenticio);

                //falta relacion con Rodeo.
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
                entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
                entity.Property(e => e.PesoAlNacer).HasColumnName("peso_al_nacer");
                entity.Property(e => e.Raza).HasColumnName("raza");
                entity.Property(e => e.MadreId).HasColumnName("id_madre");
                entity.Property(e => e.PadreId).HasColumnName("id_padre");

                entity.Property(e => e.Sexo)
                    .HasColumnName("sexo")
                    .HasColumnType("sexo_animal")
                    .HasConversion(
                        v => v.ToString().ToLower(),
                        v => Enum.Parse<Sexo>(v, ignoreCase: true)
                     );
                    

                entity.Property(e => e.ColorPelaje).HasColumnName("color_pelaje");
                entity.Property(e => e.FechaAlta).HasColumnName("fecha_alta");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.RodeoId).HasColumnName("id_rodeo");
                
                entity.HasOne(a => a.Rodeo)
                    .WithMany(r => r.Animales)
                    .HasForeignKey(a => a.RodeoId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(a => a.Madre)
                    .WithMany()
                    .HasForeignKey(a => a.MadreId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(a => a.Padre)
                    .WithMany()
                    .HasForeignKey(a => a.PadreId)
                    .OnDelete(DeleteBehavior.SetNull);
                
            }
            );
        }
    }
}


