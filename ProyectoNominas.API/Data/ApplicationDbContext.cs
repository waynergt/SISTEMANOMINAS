using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProyectoNominas.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TipoDocumentoObligatorio> TipoDocumentoObligatorio { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Puesto> Puestos { get; set; }
        public DbSet<DocumentoEmpleado> DocumentosEmpleado { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }
        public DbSet<ConfiguracionExpediente> ConfiguracionesExpediente { get; set; }
        public DbSet<InformacionAcademica> InformacionesAcademicas { get; set; }
        public DbSet<Nomina> Nominas { get; set; }
        public DbSet<DescuentoLegal> DescuentosLegales { get; set; }
        public DbSet<DetalleDescuentoNomina> DetallesDescuentoNomina { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioRol>()
                .HasKey(ur => new { ur.UsuarioId, ur.RolId });

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.UsuarioRoles)
                .HasForeignKey(ur => ur.UsuarioId);

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(ur => ur.RolId);
        }
    }
}
