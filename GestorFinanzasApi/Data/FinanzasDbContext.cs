using GestorFinanzasAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorFinanzasAPI.Data

{
    public class FinanzasDbContext : DbContext
    {
        public FinanzasDbContext(DbContextOptions<FinanzasDbContext> options)
: base(options)
        {
        }

        public DbSet<Dispositivo> Dispositivos => Set<Dispositivo>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Movimiento> Movimientos => Set<Movimiento>();
        public DbSet<Presupuesto> Presupuestos => Set<Presupuesto>();
        public DbSet<Abono> Abonos => Set<Abono>();
        public DbSet<Configuracion> Configuraciones => Set<Configuracion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dispositivo>(e =>
            {
                e.ToTable("dispositivo");
                e.HasKey(x => x.IdDispositivo);
                e.Property(x => x.IdDispositivo).HasColumnName("id_dispositivo").UseIdentityByDefaultColumn();
                e.Property(x => x.Identificador).HasColumnName("identificador").HasMaxLength(255).IsRequired();
                e.HasIndex(x => x.Identificador).IsUnique();
            });

            modelBuilder.Entity<Categoria>(e =>
            {
                e.ToTable("categoria");
                e.HasKey(x => x.IdCategoria);
                e.Property(x => x.IdCategoria).HasColumnName("id_categoria").UseIdentityByDefaultColumn();
                e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                e.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(30).IsRequired();
                e.Property(x => x.Color).HasColumnName("color").HasMaxLength(30);
                e.Property(x => x.Icono).HasColumnName("icono").HasMaxLength(80);
                e.Property(x => x.Personalizada).HasColumnName("personalizada");
                e.Property(x => x.IdDispositivo).HasColumnName("id_dispositivo");

                e.HasIndex(x => new { x.IdDispositivo, x.Nombre }).IsUnique();

                e.HasOne(x => x.Dispositivo)
                    .WithMany(x => x.Categorias)
                    .HasForeignKey(x => x.IdDispositivo)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Movimiento>(e =>
            {
                e.ToTable("movimiento");
                e.HasKey(x => x.IdMovimiento);
                e.Property(x => x.IdMovimiento).HasColumnName("id_movimiento").UseIdentityByDefaultColumn();
                e.Property(x => x.IdCategoria).HasColumnName("id_categoria");
                e.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(20).IsRequired();
                e.Property(x => x.Monto).HasColumnName("monto").HasPrecision(14, 2);
                e.Property(x => x.Fecha).HasColumnName("fecha").HasColumnType("date");
                e.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(500).IsRequired();
                e.Property(x => x.Creado).HasColumnName("creado").HasColumnType("timestamp without time zone");
                e.Property(x => x.Actualizado).HasColumnName("actualizado").HasColumnType("timestamp without time zone");
                e.Property(x => x.IdDispositivo).HasColumnName("id_dispositivo");

                e.HasIndex(x => new { x.IdDispositivo, x.Fecha });
                e.HasIndex(x => new { x.IdDispositivo, x.Tipo, x.Fecha });

                e.HasOne(x => x.Categoria)
                    .WithMany(x => x.Movimientos)
                    .HasForeignKey(x => x.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Dispositivo)
                    .WithMany(x => x.Movimientos)
                    .HasForeignKey(x => x.IdDispositivo)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Presupuesto>(e =>
            {
                e.ToTable("presupuesto");
                e.HasKey(x => x.IdPresupuesto);
                e.Property(x => x.IdPresupuesto).HasColumnName("id_presupuesto").UseIdentityByDefaultColumn();
                e.Property(x => x.IdCategoria).HasColumnName("id_categoria");
                e.Property(x => x.Monto).HasColumnName("monto").HasPrecision(14, 2);
                e.Property(x => x.Periodo).HasColumnName("periodo").HasMaxLength(50).IsRequired();
                e.Property(x => x.FechaInicio).HasColumnName("fecha_inicio").HasColumnType("date");
                e.Property(x => x.FechaFin).HasColumnName("fecha_fin").HasColumnType("date");
                e.Property(x => x.IdDispositivo).HasColumnName("id_dispositivo");

                e.HasOne(x => x.Categoria)
                    .WithMany(x => x.Presupuestos)
                    .HasForeignKey(x => x.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Dispositivo)
                    .WithMany(x => x.Presupuestos)
                    .HasForeignKey(x => x.IdDispositivo)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Abono>(e =>
            {
                e.ToTable("abono");
                e.HasKey(x => x.IdAbono);
                e.Property(x => x.IdAbono).HasColumnName("id_abono").UseIdentityByDefaultColumn();
                e.Property(x => x.IdPresupuesto).HasColumnName("id_presupuesto");
                e.Property(x => x.Monto).HasColumnName("monto").HasPrecision(14, 2);
                e.Property(x => x.Fecha).HasColumnName("fecha").HasColumnType("date");
                e.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(500).IsRequired();

                e.HasOne(x => x.Presupuesto)
                    .WithMany(x => x.Abonos)
                    .HasForeignKey(x => x.IdPresupuesto)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Configuracion>(e =>
            {
                e.ToTable("configuracion");
                e.HasKey(x => x.IdConfiguracion);
                e.Property(x => x.IdConfiguracion).HasColumnName("id_configuracion").UseIdentityByDefaultColumn();
                e.Property(x => x.Moneda).HasColumnName("moneda").HasMaxLength(10).IsRequired();
                e.Property(x => x.Tema).HasColumnName("tema").HasMaxLength(30).IsRequired();
                e.Property(x => x.Idioma).HasColumnName("idioma").HasMaxLength(10).IsRequired();
                e.Property(x => x.IdDispositivo).HasColumnName("id_dispositivo");

                e.HasIndex(x => x.IdDispositivo).IsUnique();

                e.HasOne(x => x.Dispositivo)
                    .WithOne(x => x.Configuracion)
                    .HasForeignKey<Configuracion>(x => x.IdDispositivo)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}