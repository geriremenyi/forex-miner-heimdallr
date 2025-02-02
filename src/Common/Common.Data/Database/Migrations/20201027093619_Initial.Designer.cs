﻿// <auto-generated />
using System;
using ForexMiner.Heimdallr.Common.Data.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ForexMiner.Heimdallr.Common.Data.Database.Migrations
{
    [DbContext(typeof(ForexMinerHeimdallrDbContext))]
    [Migration("20201027093619_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ForexMiner.Heimdallr.Common.Data.Database.Models.Connection.Connection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Broker")
                        .HasColumnType("int");

                    b.Property<string>("ExternalAccountId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument.Instrument", b =>
                {
                    b.Property<int>("Name")
                        .HasColumnType("int");

                    b.HasKey("Name");

                    b.ToTable("Instruments");
                });

            modelBuilder.Entity("ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument.InstrumentGranularity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Granularity")
                        .HasColumnType("int");

                    b.Property<int?>("InstrumentName")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InstrumentName");

                    b.ToTable("InstrumentGranularity");
                });

            modelBuilder.Entity("ForexMiner.Heimdallr.Common.Data.Database.Models.Trade.TradeSignal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Confidence")
                        .HasColumnType("float");

                    b.Property<int>("Direction")
                        .HasColumnType("int");

                    b.Property<int>("Instrument")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("TradeSignals");
                });

            modelBuilder.Entity("ForexMiner.Heimdallr.Common.Data.Database.Models.User.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ForexMiner.Heimdallr.Common.Data.Database.Models.Connection.Connection", b =>
                {
                    b.HasOne("ForexMiner.Heimdallr.Common.Data.Database.Models.User.User", "Owner")
                        .WithMany("Connections")
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument.InstrumentGranularity", b =>
                {
                    b.HasOne("ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument.Instrument", "Instrument")
                        .WithMany("Granularities")
                        .HasForeignKey("InstrumentName");
                });
#pragma warning restore 612, 618
        }
    }
}
