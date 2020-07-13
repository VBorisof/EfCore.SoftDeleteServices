﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using DataLayer.CascadeEfCode;
using DataLayer.Interfaces;
using DataLayer.SingleEfClasses;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.SingleEfCode
{
    public class SingleSoftDelDbContext : DbContext, IUserId
    {
        /// <summary>
        /// This holds the current userId, or GUID.Empty if not given
        /// </summary>
        public Guid UserId { get; private set; }

        public SingleSoftDelDbContext(DbContextOptions<SingleSoftDelDbContext> options, Guid userId = default)
            : base(options)
        {
            UserId = userId;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //This automatically configures the two types of soft deletes
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISingleSoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    if (typeof(IUserId).IsAssignableFrom(entityType.ClrType))
                        entityType.SetSingleQueryFilter(SingleQueryFilterTypes.SingleSoftDeleteAndUserId, this);
                    else
                        entityType.SetSingleQueryFilter(SingleQueryFilterTypes.SingleSoftDelete, this);
                }
            }
        }
    }
}