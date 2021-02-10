﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using builk_uploads_api.DataContext;

namespace builk_uploads_api.Migrations
{
    [DbContext(typeof(DataConfigContext))]
    partial class DataConfigContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("builk_uploads_api.DataContext.Entites.ColumnBySource", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("DataTypeid")
                        .HasColumnType("int");

                    b.Property<int?>("SourceConfigurationid")
                        .HasColumnType("int");

                    b.Property<int?>("Validationid")
                        .HasColumnType("int");

                    b.Property<string>("columnName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("filecolumnName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("lastModificationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("lastModificationUser")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("DataTypeid");

                    b.HasIndex("SourceConfigurationid");

                    b.HasIndex("Validationid");

                    b.ToTable("tb_ColumnBySource");
                });

            modelBuilder.Entity("builk_uploads_api.DataContext.Entites.DataType", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("lastModificationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("lastModificationUser")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("tb_DataType");
                });

            modelBuilder.Entity("builk_uploads_api.DataContext.Entites.Source", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("lastModificationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("lastModificationUser")
                        .HasColumnType("int");

                    b.Property<string>("type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("tb_Source");
                });

            modelBuilder.Entity("builk_uploads_api.DataContext.Entites.SourceConfiguration", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("Sourceid")
                        .HasColumnType("int");

                    b.Property<string>("alias")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("conectionString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("lastModificationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("lastModificationUser")
                        .HasColumnType("int");

                    b.Property<string>("sharePointListName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("sharePointSiteUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tableName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("Sourceid");

                    b.ToTable("tb_SourceConfiguration");
                });

            modelBuilder.Entity("builk_uploads_api.DataContext.Entites.Validation", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("lastModificationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("lastModificationUser")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("validation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("tb_Validation");
                });

            modelBuilder.Entity("builk_uploads_api.DataContext.Entites.ColumnBySource", b =>
                {
                    b.HasOne("builk_uploads_api.DataContext.Entites.DataType", "DataType")
                        .WithMany()
                        .HasForeignKey("DataTypeid");

                    b.HasOne("builk_uploads_api.DataContext.Entites.SourceConfiguration", "SourceConfiguration")
                        .WithMany()
                        .HasForeignKey("SourceConfigurationid");

                    b.HasOne("builk_uploads_api.DataContext.Entites.Validation", "Validation")
                        .WithMany()
                        .HasForeignKey("Validationid");

                    b.Navigation("DataType");

                    b.Navigation("SourceConfiguration");

                    b.Navigation("Validation");
                });

            modelBuilder.Entity("builk_uploads_api.DataContext.Entites.SourceConfiguration", b =>
                {
                    b.HasOne("builk_uploads_api.DataContext.Entites.Source", "Source")
                        .WithMany()
                        .HasForeignKey("Sourceid");

                    b.Navigation("Source");
                });
#pragma warning restore 612, 618
        }
    }
}
