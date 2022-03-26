using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth.Sample.Api.Helper
{
    public static class EnumHelper
    {
		/// <summary>
		/// 轉成數字。
		/// </summary>
		/// <param name="source">資料來源。</param>
		/// <returns></returns>
		public static int ToInt<TSource>(this TSource source)
		{
			return Convert.ToInt32(source);
		}

		/// <summary>
		/// 從數字轉成字串。
		/// </summary>
		/// <param name="source">資料來源。</param>
		/// <returns></returns>
		public static string FromIntToString<TSource>(this TSource source)
		{
			return Convert.ToInt32(source).ToString();
		}

		/// <summary>
		/// 取得屬性中的預設值。
		/// </summary>
		/// <param name="source">資料來源</param>
		/// <returns></returns>
		public static object DefaultValue<TSource>(this TSource source)
		{
			var descriptionAttribute = (DefaultValueAttribute)source.GetType()
					.GetField(source.ToString())
					.GetCustomAttributes(false)
					.Where(a => a is DefaultValueAttribute)
					.FirstOrDefault();

			return descriptionAttribute != null ? descriptionAttribute.Value : source.ToString();
		}

		/// <summary>
		/// 取得屬性中的描述。
		/// </summary>
		/// <param name="source">資料來源</param>
		/// <returns></returns>
		public static string Description<TSource>(this TSource source)
		{
			var descriptionAttribute = (DescriptionAttribute)source.GetType()
					.GetField(source.ToString())
					.GetCustomAttributes(false)
					.Where(a => a is DescriptionAttribute)
					.FirstOrDefault();

			return descriptionAttribute != null ? descriptionAttribute.Description : source.ToString();
		}


	}
}

