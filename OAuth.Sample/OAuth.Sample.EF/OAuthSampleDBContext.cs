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
            modelBuilder
                .Entity<User>();

            modelBuilder
                .Entity<UserOAuthSetting>()
                .AddIndex(x => new { x.ProviderType, x.Key });

            base.OnModelCreating(modelBuilder);
        }

    }

}

