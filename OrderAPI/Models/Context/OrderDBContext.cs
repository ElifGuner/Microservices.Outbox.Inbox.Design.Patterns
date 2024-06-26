﻿using Microsoft.EntityFrameworkCore;
using OrderAPI.Models;
using OrderAPI.Models.Entities;

namespace OrderAPI.Context
{
    public class OrderDBContext : DbContext
    {

        public OrderDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<OrderOutbox> OrderOutboxes { get; set; }
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost, 1433; Database:ECommerceDb; User Id=sa; Password=1q2w3e4r+!");
        }
        */

    }

}
