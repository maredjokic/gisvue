using Backend_App.EntityCore.Models;
using Backend_App.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_App.EntityCore
{
    public class PostgreDbContext : DbContext
    {
        public PostgreDbContext(DbContextOptions<PostgreDbContext> options) : base(options)
        {
        }

        public DbSet<SensorData> SensorDbSet { get; set; }
        public DbSet<VehiclePoint> VehiclePointDbSet { get; set; }
        public DbSet<DistancePointData> DistancePoint { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("VehiclePointNumbers").StartsAt(1).IncrementsBy(1);
            modelBuilder.HasSequence<int>("SensorDataNumbers").StartsAt(1).IncrementsBy(1);


            modelBuilder.Entity<SensorData>().ToTable("SensorData")
                .Property(x => x.Id).HasDefaultValueSql("nextval('\"SensorDataNumbers\"')");
            modelBuilder.Entity<VehiclePoint>().ToTable("VehiclePoint")
                .Property(x => x.Id).HasDefaultValueSql("nextval('\"VehiclePointNumbers\"')");
            modelBuilder.Entity<DistancePointData>(eb => 
            {
                eb.HasNoKey();
                eb.Property(x => x.VID).HasColumnName("VID");
                eb.Property(x => x.DateTime).HasColumnName("DateTime");
                eb.Property(x => x.Lat).HasColumnName("Lat");
                eb.Property(x => x.Lon).HasColumnName("Lon");
                eb.Property(x => x.Speed).HasColumnName("Speed");
                eb.Property(x => x.Distance).HasColumnName("Distance");
                eb.Property(x => x.Value).HasColumnName("Value");
            });
        }
    }
}
