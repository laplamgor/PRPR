using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PRPR.Common;

namespace PRPR.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20180204092257_AddPersonalizationRecordTables")]
    partial class AddPersonalizationRecordTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5");

            modelBuilder.Entity("PRPR.BooruViewer.Models.LockScreenRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("PostId");

                    b.Property<string>("PostPreviewUrl");

                    b.HasKey("Id");

                    b.ToTable("LockScreenRecords");
                });

            modelBuilder.Entity("PRPR.BooruViewer.Models.WallpaperRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("PostId");

                    b.Property<string>("PostPreviewUrl");

                    b.HasKey("Id");

                    b.ToTable("WallpaperRecords");
                });

            modelBuilder.Entity("PRPR.ExReader.Models.ExSearchRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Keyword");

                    b.HasKey("Id");

                    b.ToTable("ExSearchRecords");
                });
        }
    }
}
