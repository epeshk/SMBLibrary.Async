using System;

namespace Utilities
{
	public class Conversion
	{
        public static short ToInt16(object obj)
        {
            return ToInt16(obj, 0);
        }

        public static short ToInt16(object obj, short defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToInt16(obj);
                }
                catch
                { }
            }
            return result;
        }

        public static int ToInt32(object obj)
        {
            return ToInt32(obj, 0);
        }

		public static int ToInt32(object obj, int defaultValue)
		{
            var result = defaultValue;
			if (obj != null)
			{
				try
				{
					result = Convert.ToInt32(obj);
				}
				catch
				{}
			}
			return result;
		}

        public static long ToInt64(object obj)
        {
            return ToInt64(obj, 0);
        }

        public static long ToInt64(object obj, long defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToInt64(obj);
                }
                catch
                { }
            }
            return result;
        }

        public static ushort ToUInt16(object obj)
        {
            return ToUInt16(obj, 0);
        }

        public static ushort ToUInt16(object obj, ushort defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToUInt16(obj);
                }
                catch
                { }
            }
            return result;
        }

        public static uint ToUInt32(object obj)
        {
            return ToUInt32(obj, 0);
        }

        public static uint ToUInt32(object obj, uint defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToUInt32(obj);
                }
                catch
                { }
            }
            return result;
        }

        public static ulong ToUInt64(object obj)
        {
            return ToUInt64(obj, 0);
        }

        public static ulong ToUInt64(object obj, ulong defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToUInt64(obj);
                }
                catch
                { }
            }
            return result;
        }

        public static float ToFloat(object obj)
        {
            return ToFloat(obj, 0);
        }

		public static float ToFloat(object obj, float defaultValue)
		{
            var result = defaultValue;
			if (obj != null)
			{
				try
				{
					result = Convert.ToSingle(obj);
				}
				catch
				{}
			}
			return result;
		}

        public static double ToDouble(object obj)
        {
            return ToDouble(obj, 0);
        }

        public static double ToDouble(object obj, double defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToDouble(obj);
                }
                catch
                { }
            }
            return result;
        }

        public static decimal ToDecimal(object obj)
        {
            return ToDecimal(obj, 0);
        }

        public static decimal ToDecimal(object obj, decimal defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToDecimal(obj);
                }
                catch
                { }
            }
            return result;
        }

        public static bool ToBoolean(object obj)
        {
            return ToBoolean(obj, false);
        }

        public static bool ToBoolean(object obj, bool defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToBoolean(obj);
                }
                catch
                { }
            }
            return result;
        }

        public static string ToString(object obj)
        {
            var result = String.Empty;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToString(obj);
                }
                catch
                {}
            }
            return result;
        }

        public static char ToChar(object obj)
        {
            return ToChar(obj, new char());
        }

        public static char ToChar(object obj, char defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToChar(obj);
                }
                catch
                { }
            }
            return result;
        }

        public static DateTime ToDateTime(object obj)
        {
            return ToDateTime(obj, DateTime.MinValue);
        }

        public static DateTime ToDateTime(object obj, DateTime defaultValue)
        {
            var result = defaultValue;
            if (obj != null)
            {
                try
                {
                    result = Convert.ToDateTime(obj);
                }
                catch
                { }
            }
            return result;
        }
	}
}