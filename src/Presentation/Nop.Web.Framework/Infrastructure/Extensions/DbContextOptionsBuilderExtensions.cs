﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Data;

namespace Nop.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of DbContextOptionsBuilder
    /// </summary>
    public static class DbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// SQL Server specific extension method for Microsoft.EntityFrameworkCore.DbContextOptionsBuilder
        /// </summary>
        /// <param name="optionsBuilder">Database context options builder</param>
        /// <param name="services">Collection of service descriptors</param>
        public static void UseSqlServerWithLazyLoading(this DbContextOptionsBuilder optionsBuilder, IServiceCollection services)
        {
            var nopConfig = services.BuildServiceProvider().GetRequiredService<NopConfig>();

            var dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            var dp = new EfDataProviderManager().DataProvider;
            var typeFinder = new WebAppTypeFinder();
            var dbContextType = typeFinder.FindClassesOfType<IDbContextOptionsBuilderHelper>()
                                .FirstOrDefault(p => p.Assembly == dp.GetType().Assembly);
            var dbContext = (IDbContextOptionsBuilderHelper)Activator.CreateInstance(dbContextType);
            dbContext.Configure(optionsBuilder, services, nopConfig, dataSettings);
        }
    }
}
