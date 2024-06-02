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

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<OrdenDetalle> OrdenDetalles { get; set; }

    public virtual DbSet<Ordene> Ordenes { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Plato> Platos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Reseña> Reseñas { get; set; }

    public virtual DbSet<Restaurante> Restaurantes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source= Obligatorio ;Initial Catalog= ObligatorioProgramacion3;Integrated Security=True; TrustServerCertificate=True");

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clientes__3214EC275421FBFD");

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

        });

        modelBuilder.Entity<Clima>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clima__3214EC274174D6B3");

            entity.ToTable("Clima");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DescripciónClima)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Temperatura).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mesas__3214EC27D6DDDACD");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OrdenDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrdenDet__3214EC27EDE039AF");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OrdenId).HasColumnName("OrdenID");
            entity.Property(e => e.PlatoId).HasColumnName("PlatoID");

            entity.HasOne(d => d.Orden).WithMany(p => p.OrdenDetalles)
                .HasForeignKey(d => d.OrdenId)
                .HasConstraintName("FK__OrdenDeta__Orden__4AB81AF0");

            entity.HasOne(d => d.Plato).WithMany(p => p.OrdenDetalles)
                .HasForeignKey(d => d.PlatoId)
                .HasConstraintName("FK__OrdenDeta__Plato__4BAC3F29");
        });

        modelBuilder.Entity<Ordene>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ordenes__3214EC27780884F0");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ReservaId).HasColumnName("ReservaID");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Reserva).WithMany(p => p.Ordenes)
                .HasForeignKey(d => d.ReservaId)
                .HasConstraintName("FK__Ordenes__Reserva__47DBAE45");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pagos__3214EC27C5FBD521");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ReservaId).HasColumnName("ReservaID");

            entity.HasOne(d => d.Reserva).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.ReservaId)
                .HasConstraintName("FK__Pagos__ReservaID__4E88ABD4");
        });

        modelBuilder.Entity<Plato>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Plato__3214EC274A51F064");

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
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservas__3214EC27C6B5A3DF");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaReserva).HasColumnType("datetime");
            entity.Property(e => e.MesaId).HasColumnName("MesaID");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Reservas__Client__412EB0B6");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.MesaId)
                .HasConstraintName("FK__Reservas__MesaID__4222D4EF");
        });

        modelBuilder.Entity<Reseña>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reseñas__3214EC27FD417909");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Comentario)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaReseña).HasColumnType("datetime");
            entity.Property(e => e.RestauranteId).HasColumnName("RestauranteID");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reseñas)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Reseñas__Cliente__52593CB8");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Reseñas)
                .HasForeignKey(d => d.RestauranteId)
                .HasConstraintName("FK__Reseñas__Restaur__534D60F1");
        });

        modelBuilder.Entity<Restaurante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Restaura__3214EC27FA5ED3A8");

            entity.Property(e => e.Id).HasColumnName("ID");
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

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC270286101A");

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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
