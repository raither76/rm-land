using System;
using System.Collections.Generic;
using System.Linq;
using Rmis.Domain;

namespace Rmis.Persistence
{
    public class DataInitializer
    {
        public static void Initialize(RmisDbContext context)
        {
            var initializer = new DataInitializer();
            initializer.Seed(context);
        }

        public void Seed(RmisDbContext context)
        {
            context.Database.EnsureCreated();

            EnsureStationsCreated(context);
        }
        
        private void EnsureStationsCreated(RmisDbContext context)
        {
            if (context.StationRepository.Any())
                return;

            context.StationRepository.AddRange(new List<Station>()
            {
                new Station {
                    YaCode = "s9602494",
                    DisplayName = "Санкт-Петербург (Московский вокзал)",
                    CreateDate = DateTimeOffset.Now,
                    ModifyDate = DateTimeOffset.Now },
                new Station {
                    YaCode = "s2006004",
                    DisplayName = "Москва (Ленинградский вокзал)",
                    CreateDate = DateTimeOffset.Now,
                    ModifyDate = DateTimeOffset.Now }
            });

            context.SaveChanges();
        }
    }
}