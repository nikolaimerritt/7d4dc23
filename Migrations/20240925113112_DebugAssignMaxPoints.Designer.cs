﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PirateConquest.Database;

#nullable disable

namespace PirateConquest.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240925113112_DebugAssignMaxPoints")]
    partial class DebugAssignMaxPoints
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.20");

            modelBuilder.Entity("PirateConquest.Models.AdjacentSea", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AdjacentToId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeaId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AdjacentToId");

                    b.HasIndex("SeaId");

                    b.ToTable("AdjacentSeas");
                });

            modelBuilder.Entity("PirateConquest.Models.Configuration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CooldownMinutes")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CtfId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("DebugAssignMaxPoints")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FirstRoundStartUtc")
                        .HasColumnType("TEXT");

                    b.Property<int>("MaxMessageCharacters")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxMessagesPerTeam")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlanningMinutes")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlaygroundLeaderboardUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PointsPerShip")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundsCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamStartingShips")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("PirateConquest.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ReadByRecipient")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RecipientId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SenderId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("PirateConquest.Models.Move", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("TEXT");

                    b.Property<int>("FromSeaId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ShipCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ToSeaId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FromSeaId");

                    b.HasIndex("RoundId");

                    b.HasIndex("TeamId");

                    b.HasIndex("ToSeaId");

                    b.ToTable("Moves");
                });

            modelBuilder.Entity("PirateConquest.Models.Outcome", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeaId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ShipsAfter")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ShipsBefore")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RoundId");

                    b.HasIndex("SeaId");

                    b.HasIndex("TeamId");

                    b.ToTable("Outcomes");
                });

            modelBuilder.Entity("PirateConquest.Models.Purchase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("TEXT");

                    b.Property<int>("Points")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeaId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ShipCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RoundId");

                    b.HasIndex("SeaId");

                    b.HasIndex("TeamId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("PirateConquest.Models.Round", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("End")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsInitial")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartCooldown")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartPlanning")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Rounds");
                });

            modelBuilder.Entity("PirateConquest.Models.Sea", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Seas");
                });

            modelBuilder.Entity("PirateConquest.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PlainTextPassword")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("StartingSeaId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("StartingSeaId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("PirateConquest.Models.AdjacentSea", b =>
                {
                    b.HasOne("PirateConquest.Models.Sea", "AdjacentTo")
                        .WithMany()
                        .HasForeignKey("AdjacentToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.Sea", "Sea")
                        .WithMany()
                        .HasForeignKey("SeaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AdjacentTo");

                    b.Navigation("Sea");
                });

            modelBuilder.Entity("PirateConquest.Models.Message", b =>
                {
                    b.HasOne("PirateConquest.Models.Team", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.Team", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipient");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("PirateConquest.Models.Move", b =>
                {
                    b.HasOne("PirateConquest.Models.Sea", "FromSea")
                        .WithMany()
                        .HasForeignKey("FromSeaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.Round", "Round")
                        .WithMany()
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.Sea", "ToSea")
                        .WithMany()
                        .HasForeignKey("ToSeaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FromSea");

                    b.Navigation("Round");

                    b.Navigation("Team");

                    b.Navigation("ToSea");
                });

            modelBuilder.Entity("PirateConquest.Models.Outcome", b =>
                {
                    b.HasOne("PirateConquest.Models.Round", "Round")
                        .WithMany()
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.Sea", "Sea")
                        .WithMany()
                        .HasForeignKey("SeaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Round");

                    b.Navigation("Sea");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("PirateConquest.Models.Purchase", b =>
                {
                    b.HasOne("PirateConquest.Models.Round", "Round")
                        .WithMany()
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.Sea", "Sea")
                        .WithMany()
                        .HasForeignKey("SeaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Round");

                    b.Navigation("Sea");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("PirateConquest.Models.Team", b =>
                {
                    b.HasOne("PirateConquest.Models.Sea", "StartingSea")
                        .WithMany()
                        .HasForeignKey("StartingSeaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StartingSea");
                });
#pragma warning restore 612, 618
        }
    }
}
