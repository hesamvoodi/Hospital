using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Hospital.Models;

namespace Hospital.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DoctorGroup> DoctorGroups { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<PhoneNumberToken> PhoneNumberTokens { get; set; }
        public DbSet<DoctorTicket> DoctorTickets { get; set; }
        public DbSet<Pays> Pays { get; set; }
    }
}
