using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OAuth.Sample.Api.Helper
{
    public static class TimestampHelper
    {
		/// <summary>
		/// 將目前的 System.DateTime 物件的值轉換為其對等的時間戳記字串表示。
		/// </summary>		
		/// <returns>字串，內含目前 System.DateTime 物件的時間戳記字串表示。</returns>
		public static long Generate()
		{
			return ((long)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
		}

		/// <summary>
		/// 將指定的 System.DateTime 物件的值轉換為其對等的時間戳記字串表示。
		/// </summary>
		/// <param name="dateTime">表示時間的瞬間，通常以一天的日期和時間表示。</param>
		/// <returns>字串，內含目前 System.DateTime 物件的時間戳記字串表示。</returns>
		public static long Generate(DateTime dateTime)
		{
			return ((long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
		}

		/// <summary>
		/// 將時間戳記的值轉換為其對等的 System.DateTime。
		/// </summary>
		/// <param name="timestamp">時間戳記。</param>
		/// <returns></returns>
		public static DateTime ToUTCDateTime(long timestamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds((double)timestamp);
		}
	}
}

