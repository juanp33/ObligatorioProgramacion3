using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ObligatorioProgramacion3.Models;

public partial class ObligatorioProgramacion3Context : DbContext
{
    public ObligatorioProgramacion3Context()
    {
    }

    public ObligatorioProgramacion3Context(DbContextOptions<ObligatorioProgramacion3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Clima> Climas { get; set; }

    public virtual DbSet<Cotizacion> Cotizacions { get; set; }

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<OrdenDetalle> OrdenDetalles { get; set; }

    public virtual DbSet<Ordene> Ordenes { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Plato> Platos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Reseña> Reseñas { get; set; }

    public virtual DbSet<Restaurante> Restaurantes { get; set; }

    public virtual DbSet<RolPermiso> RolPermisos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioRole> UsuarioRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source= DESKTOP-AFV45M7;Initial Catalog= ObligatorioProgramacion3;Integrated Security=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clientes__3214EC27F7A21606");

            entity.HasIndex(e => e.IdUsuarios, "UQ_Cliente_IdUsuarios").IsUnique();

            entity.HasIndex(e => e.Email, "uEmail").IsUnique();

            entity.HasIndex(e => e.Nombre, "uNombre").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoCliente)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuariosNavigation).WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(d => d.IdUsuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkUsuarios");
        });

        modelBuilder.Entity<Clima>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clima__3214EC278A3C3F2B");

            entity.ToTable("Clima");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DescripciónClima)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Temperatura).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Cotizacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cotizaci__3213E83F6B581E3E");

            entity.ToTable("Cotizacion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cotizacion1).HasColumnName("cotizacion");
            entity.Property(e => e.FechaCotizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaCotizacion");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mesas__3214EC27B8C60686");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdRestaurante).HasColumnName("idRestaurante");

            entity.HasOne(d => d.IdRestauranteNavigation).WithMany(p => p.Mesas)
                .HasForeignKey(d => d.IdRestaurante)
                .HasConstraintName("FK__Mesas__idRestaur__5DCAEF64");
        });

        modelBuilder.Entity<OrdenDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrdenDet__3214EC2739CA38AC");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OrdenId).HasColumnName("OrdenID");
            entity.Property(e => e.PlatoId).HasColumnName("PlatoID");

            entity.HasOne(d => d.Orden).WithMany(p => p.OrdenDetalles)
                .HasForeignKey(d => d.OrdenId)
                .HasConstraintName("FK__OrdenDeta__Orden__5EBF139D");

            entity.HasOne(d => d.Plato).WithMany(p => p.OrdenDetalles)
                .HasForeignKey(d => d.PlatoId)
                .HasConstraintName("FK__OrdenDeta__Plato__5FB337D6");
        });

        modelBuilder.Entity<Ordene>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ordenes__3214EC27464DCD71");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ReservaId).HasColumnName("ReservaID");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Reserva).WithMany(p => p.Ordenes)
                .HasForeignKey(d => d.ReservaId)
                .HasConstraintName("FK__Ordenes__Reserva__60A75C0F");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pagos__3214EC27D41AA689");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.IdClima).HasColumnName("idClima");
            entity.Property(e => e.IdCotizacion).HasColumnName("idCotizacion");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ReservaId).HasColumnName("ReservaID");

            entity.HasOne(d => d.IdClimaNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdClima)
                .HasConstraintName("FK__Pagos__idClima__628FA481");

            entity.HasOne(d => d.IdCotizacionNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdCotizacion)
                .HasConstraintName("FK__Pagos__idCotizac__6383C8BA");

            entity.HasOne(d => d.Reserva).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.ReservaId)
                .HasConstraintName("FK__Pagos__ReservaID__619B8048");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.PermisoId).HasName("PK__Permisos__96E0C7238EBC47B9");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Plato>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Plato__3214EC273E64F0D1");

            entity.ToTable("Plato");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Descripción)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Imagen)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.NombrePlato)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdRestauranteNavigation).WithMany(p => p.Platos)
                .HasForeignKey(d => d.IdRestaurante)
                .HasConstraintName("fkidrestaurante");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservas__3214EC27D88CD92E");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaReserva).HasColumnType("datetime");
            entity.Property(e => e.MesaId).HasColumnName("MesaID");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Reservas__Client__6754599E");

            entity.HasOne(d => d.IdRestauranteNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdRestaurante)
                .HasConstraintName("FK__Reservas__IdRest__66603565");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.MesaId)
                .HasConstraintName("FK__Reservas__MesaID__68487DD7");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Reservas__Usuari__693CA210");
        });

        modelBuilder.Entity<Reseña>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reseñas__3214EC2742091B23");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Comentario)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaReseña).HasColumnType("datetime");
            entity.Property(e => e.RestauranteId).HasColumnName("RestauranteID");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reseñas)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Reseñas__Cliente__6477ECF3");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Reseñas)
                .HasForeignKey(d => d.RestauranteId)
                .HasConstraintName("FK__Reseñas__Restaur__656C112C");
        });

        modelBuilder.Entity<Restaurante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Restaura__3214EC27EB44BD48");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Dirección)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Teléfono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RolPermiso>(entity =>
        {
            entity.HasKey(e => e.RolPermisoId).HasName("PK__RolPermi__A80C54740D822FB9");

            entity.HasIndex(e => new { e.RolId, e.PermisoId }, "UQ__RolPermi__D04D0E82908A9FB7").IsUnique();

            entity.HasOne(d => d.Permiso).WithMany(p => p.RolPermisos)
                .HasForeignKey(d => d.PermisoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RolPermis__Permi__6B24EA82");

            entity.HasOne(d => d.Rol).WithMany(p => p.RolPermisos)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RolPermis__RolId__6A30C649");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Roles__F92302F1823FD39F");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC27C1CA34D2");

            entity.HasIndex(e => e.Email, "UQ_Usuarios_Email").IsUnique();

            entity.HasIndex(e => e.Nombre, "UQ_Usuarios_Nombre").IsUnique();

            entity.HasIndex(e => e.Nombre, "uEmailUsuarios").IsUnique();

            entity.HasIndex(e => e.Email, "uNombreUsuarios").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Contraseña)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RolId).HasColumnName("RolID");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .HasConstraintName("FK__Usuarios__RolID__6E01572D");
        });

        modelBuilder.Entity<UsuarioRole>(entity =>
        {
            entity.HasKey(e => e.UsuarioRolId).HasName("PK__UsuarioR__C869CDCAE86B1116");

            entity.HasIndex(e => new { e.UsuarioId, e.RolId }, "UQ__UsuarioR__24AFD7960B1360EB").IsUnique();

            entity.HasOne(d => d.Rol).WithMany(p => p.UsuarioRoles)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsuarioRo__RolId__6D0D32F4");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioRoles)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsuarioRo__Usuar__6C190EBB");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
