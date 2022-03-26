using OAuth.Sample.EF.Entity;
using Microsoft.EntityFrameworkCore;
using OAuth.Sample.EF.Helper;

namespace OAuth.Sample.EF
{
    public class OAuthSampleDBContext : DbContext
    {
        public OAuthSampleDBContext(DbContextOptions<OAuthSampleDBContext> options) : base(options) { }

        /// <summary>
        /// 取得單一DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DbSet<T> GetDbSet<T>() where T : class
        {
            return this.Set<T>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // sample
            //modelBuilder
            //    .Entity<SystemSetting>()
            //    .AddIndex(x => x.SourceID)
            //    .AddIndex(x => new { x.SourceID, x.Key });

            base.OnModelCreating(modelBuilder);
        }

    }

}

