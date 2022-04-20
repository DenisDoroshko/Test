using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.DataAccess.EFRepositories
{
    /// <summary>
    /// Provides access to database.
    /// </summary>
    internal class TicketManagementContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TicketManagementContext"/> class.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public TicketManagementContext(DbContextOptions<TicketManagementContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Provides to venue table.
        /// </summary>
        public DbSet<Venue> Venue { get; set; }

        /// <summary>
        /// Provides access to layout table.
        /// </summary>
        public DbSet<Layout> Layout { get; set; }

        /// <summary>
        /// Provides access to area table.
        /// </summary>
        public DbSet<Area> Area { get; set; }

        /// <summary>
        /// Provides access tp seat table.
        /// </summary>
        public DbSet<Seat> Seat { get; set; }

        /// <summary>
        /// Provides acces to event table.
        /// </summary>
        public DbSet<Event> Event { get; set; }

        /// <summary>
        /// Provides access tp event area table.
        /// </summary>
        public DbSet<EventArea> EventArea { get; set; }

        /// <summary>
        /// Provides access tp event seat table.
        /// </summary>
        public DbSet<EventSeat> EventSeat { get; set; }
    }
}
