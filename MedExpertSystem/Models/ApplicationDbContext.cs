using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MedExpertSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("ApplicationDbContext")
        {
            
        }

        public DbSet<SiteContentDefinitionModel> SiteContent { get; set; }
        public DbSet<QuestionsDataDefinitionModel> QuestionsBase { get; set; }

    }
}