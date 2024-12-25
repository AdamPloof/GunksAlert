﻿// <auto-generated />
using System;
using GunksAlert.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GunksAlert.Migrations
{
    [DbContext(typeof(GunksDbContext))]
    partial class GunksDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GunksAlert.Models.AlertCriteria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AlertPeriodId")
                        .HasColumnType("integer");

                    b.Property<int>("ClimbableConditionsId")
                        .HasColumnType("integer");

                    b.Property<int>("CragId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AlertPeriodId");

                    b.HasIndex("ClimbableConditionsId");

                    b.HasIndex("CragId");

                    b.ToTable("AlertCriteria", (string)null);
                });

            modelBuilder.Entity("GunksAlert.Models.AlertPeriod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("AlertPeriod", (string)null);
                });

            modelBuilder.Entity("GunksAlert.Models.ClimbableConditions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Clouds")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Humidity")
                        .HasColumnType("integer");

                    b.Property<int>("Pop")
                        .HasColumnType("integer");

                    b.Property<double>("Rain")
                        .HasColumnType("double precision");

                    b.Property<double>("Snow")
                        .HasColumnType("double precision");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("TempMax")
                        .HasColumnType("integer");

                    b.Property<int>("TempMin")
                        .HasColumnType("integer");

                    b.Property<int>("WindDegree")
                        .HasColumnType("integer");

                    b.Property<int>("WindGust")
                        .HasColumnType("integer");

                    b.Property<int>("WindSpeed")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ClimbableConditions", (string)null);
                });

            modelBuilder.Entity("GunksAlert.Models.Crag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StateProvince")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Crag", (string)null);
                });

            modelBuilder.Entity("GunksAlert.Models.DailyCondition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("IconDay")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("IconNight")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("Main")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.ToTable("DailyCondition", (string)null);
                });

            modelBuilder.Entity("GunksAlert.Models.Forecast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Clouds")
                        .HasColumnType("integer");

                    b.Property<int>("DailyConditionId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Humidity")
                        .HasColumnType("integer");

                    b.Property<int>("Pop")
                        .HasColumnType("integer");

                    b.Property<double>("Rain")
                        .HasColumnType("double precision");

                    b.Property<double>("Snow")
                        .HasColumnType("double precision");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("TempFeelsLike")
                        .HasColumnType("integer");

                    b.Property<int>("TempHigh")
                        .HasColumnType("integer");

                    b.Property<int>("TempLow")
                        .HasColumnType("integer");

                    b.Property<int>("WindDegree")
                        .HasColumnType("integer");

                    b.Property<int>("WindGust")
                        .HasColumnType("integer");

                    b.Property<int>("WindSpeed")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DailyConditionId");

                    b.ToTable("Forecast", (string)null);
                });

            modelBuilder.Entity("GunksAlert.Models.WeatherHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Clouds")
                        .HasColumnType("integer");

                    b.Property<int>("DailyConditionId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Humidity")
                        .HasColumnType("integer");

                    b.Property<double>("Rain")
                        .HasColumnType("double precision");

                    b.Property<double>("Snow")
                        .HasColumnType("double precision");

                    b.Property<int>("Temp")
                        .HasColumnType("integer");

                    b.Property<int>("WindDegree")
                        .HasColumnType("integer");

                    b.Property<int>("WindSpeed")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DailyConditionId");

                    b.ToTable("WeatherHistory", (string)null);
                });

            modelBuilder.Entity("GunksAlert.Models.AlertCriteria", b =>
                {
                    b.HasOne("GunksAlert.Models.AlertPeriod", "AlertPeriod")
                        .WithMany()
                        .HasForeignKey("AlertPeriodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GunksAlert.Models.ClimbableConditions", "ClimbableConditions")
                        .WithMany()
                        .HasForeignKey("ClimbableConditionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GunksAlert.Models.Crag", "Crag")
                        .WithMany()
                        .HasForeignKey("CragId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AlertPeriod");

                    b.Navigation("ClimbableConditions");

                    b.Navigation("Crag");
                });

            modelBuilder.Entity("GunksAlert.Models.Forecast", b =>
                {
                    b.HasOne("GunksAlert.Models.DailyCondition", "DailyCondition")
                        .WithMany()
                        .HasForeignKey("DailyConditionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DailyCondition");
                });

            modelBuilder.Entity("GunksAlert.Models.WeatherHistory", b =>
                {
                    b.HasOne("GunksAlert.Models.DailyCondition", "DailyCondition")
                        .WithMany()
                        .HasForeignKey("DailyConditionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DailyCondition");
                });
#pragma warning restore 612, 618
        }
    }
}
