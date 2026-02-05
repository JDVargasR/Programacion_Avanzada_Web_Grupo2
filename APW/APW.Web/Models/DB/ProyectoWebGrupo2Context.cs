using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APW.Web.Models.DB;

public partial class ProyectoWebGrupo2Context : DbContext
{
    public ProyectoWebGrupo2Context()
    {
    }

    public ProyectoWebGrupo2Context(DbContextOptions<ProyectoWebGrupo2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Source> Sources { get; set; }

    public virtual DbSet<SourceItem> SourceItems { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost; Database=Proyecto_Web_Grupo2; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Estados__3214EC07705CB63B");

            entity.HasIndex(e => e.Nombre, "UQ__Estados__75E3EFCF51F587CB").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07551BD347");

            entity.HasIndex(e => e.Nombre, "UQ__Roles__75E3EFCFADB4F329").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(50);

            entity.HasOne(d => d.Estado).WithMany(p => p.Roles)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Roles_Estados");
        });

        modelBuilder.Entity<Source>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sources__3214EC078872228F");

            entity.Property(e => e.ComponentType).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<SourceItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SourceIt__3214EC07674415A2");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Source).WithMany(p => p.SourceItems)
                .HasForeignKey(d => d.SourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SourceItems_Sources");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC071BEDB317");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__60695A19496BF510").IsUnique();

            entity.Property(e => e.Contrasena).HasMaxLength(255);
            entity.Property(e => e.Correo).HasMaxLength(150);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(150);

            entity.HasOne(d => d.Estado).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Estados");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
