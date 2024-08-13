﻿// <auto-generated />
using System;
using PirateConquest.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace PirateConquest.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240719133705_AdjacentSeas")]
    partial class AdjacentSeas
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.20");

            modelBuilder.Entity("PirateConquest.Models.AppConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AppConfigs");
                });

            modelBuilder.Entity("PirateConquest.Models.Flag", b =>
                {
                    b.Property<int>("FlagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SeasHeld")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("FlagId");

                    b.ToTable("Flags");
                });

            modelBuilder.Entity("PirateConquest.Models.Guess", b =>
                {
                    b.Property<int>("GuessId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Correct")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MastermindSuspectId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimeGuessed")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GuessId");

                    b.HasIndex("MastermindSuspectId");

                    b.HasIndex("UserId");

                    b.ToTable("Guesses");
                });

            modelBuilder.Entity("PirateConquest.Models.Hint", b =>
                {
                    b.Property<int>("HintId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RequiredPoints")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("HintId");

                    b.ToTable("Hints");
                });

            modelBuilder.Entity("PirateConquest.Models.Suspect", b =>
                {
                    b.Property<int>("SuspectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCulprit")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OperatingSystem")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Skills")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("SuspectId");

                    b.ToTable("Suspects");
                });

            modelBuilder.Entity("PirateConquest.Models.UnlockedIntel", b =>
                {
                    b.Property<int>("UnlockedIntelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("SuspectId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimeUnlocked")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UnlockedIntelId");

                    b.HasIndex("SuspectId");

                    b.HasIndex("UserId");

                    b.ToTable("UnlockedIntels");
                });

            modelBuilder.Entity("PirateConquest.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SeasHeld")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

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

                    b.ToTable("AdjacentSeasAsync");
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

                    b.Property<int>("ShipsAfter")
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

                    b.Property<int>("SeasHeld")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeaId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ShipsAfter")
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

                    b.Property<string>("Key")
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

                    b.Property<string>("ColourHexCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PlainTextPassword")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("StartingSeaId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("StartingSeaId");

                    b.ToTable("Teams", (string)null);
                });

            modelBuilder.Entity("PirateConquest.Models.Guess", b =>
                {
                    b.HasOne("PirateConquest.Models.Suspect", "Mastermind")
                        .WithMany()
                        .HasForeignKey("MastermindSuspectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Mastermind");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PirateConquest.Models.UnlockedIntel", b =>
                {
                    b.HasOne("PirateConquest.Models.Suspect", "Suspect")
                        .WithMany()
                        .HasForeignKey("SuspectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PirateConquest.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Suspect");

                    b.Navigation("User");
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
