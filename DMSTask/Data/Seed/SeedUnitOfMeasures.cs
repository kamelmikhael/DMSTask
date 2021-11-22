using DMSTask.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DMSTask.Data.Seed
{
    public static class SeedUnitOfMeasures
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ApplicationDbContext>();

            if(context.UnitOfMeasures.Count() == 0)
            {
                context.UnitOfMeasures.AddRange(
                    new UnitOfMeasure
                    {
                        UOM = "KG",
                        Description = "KiloGram"
                    },
                    new UnitOfMeasure
                    {
                        UOM = "G",
                        Description = "Gram"
                    },
                    new UnitOfMeasure
                    {
                        UOM = "KM",
                        Description = "KiloMeter"
                    },
                    new UnitOfMeasure
                    {
                        UOM = "M",
                        Description = "Meter"
                    },
                    new UnitOfMeasure
                    {
                        UOM = "Pk",
                        Description = "Pakage"
                    }
                );

                context.SaveChanges();
            }
            
        }
    }
}
