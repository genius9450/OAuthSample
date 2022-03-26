using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq.Expressions;

namespace OAuth.Sample.EF.Helper
{
    public static class EntityHelper
    {

        /// <summary>
        /// 添加索引
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="builder"></param>
        /// <param name="indexExpression"></param>
        /// <returns></returns>
        public static EntityTypeBuilder<TEntity> AddIndex<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, object>> indexExpression) where TEntity : class
        {
            builder.HasIndex(indexExpression);
            return builder;
        }

    }
}

