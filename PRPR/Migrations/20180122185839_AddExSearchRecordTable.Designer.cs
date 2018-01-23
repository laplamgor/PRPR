using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PRPR.Common;

namespace PRPR.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20180122185839_AddExSearchRecordTable")]
    partial class AddExSearchRecordTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5");

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
