﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApi.Database;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20250205125233_Inital")]
    partial class Inital
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("WebApi.StudyInfo.Degree", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentPoints")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalPoints")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Degree");
                });

            modelBuilder.Entity("WebApi.StudyInfo.Subject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("DegreeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Points")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Semester")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DegreeId");

                    b.ToTable("Subject");
                });

            modelBuilder.Entity("WebApi.StudyInfo.Subject", b =>
                {
                    b.HasOne("WebApi.StudyInfo.Degree", "Degree")
                        .WithMany()
                        .HasForeignKey("DegreeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Degree");
                });
#pragma warning restore 612, 618
        }
    }
}
