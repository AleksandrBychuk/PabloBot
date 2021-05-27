﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PabloBot.Services.Models;

namespace PabloBot.Migrations
{
    [DbContext(typeof(PabloContext))]
    partial class PabloContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("PabloBot.Services.Models.Badword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Word")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Badwords");
                });

            modelBuilder.Entity("PabloBot.Services.Models.Profiles.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("Warns")
                        .HasColumnType("integer");

                    b.Property<int>("Xp")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Profiles");
                });
#pragma warning restore 612, 618
        }
    }
}
