using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperStubs.Random
{
    public static partial class Randomizer
    {
        //TODO:  Call all Random Numbers from DataGenerator
        #region Byte

        public static byte RandomByte()
        {
            return RandomByte(byte.MinValue, byte.MaxValue);
        }

        public static byte RandomByte(byte max)
        {
            return RandomByte(0, max);
        }

        public static byte RandomByte(byte min, byte max)
        {
            return (byte)_random.Next(min, max);
        }

        public static sbyte RandomSignedByte()
        {
            return RandomSignedByte(sbyte.MinValue, sbyte.MaxValue);
        }

        public static sbyte RandomSignedByte(sbyte max)
        {
            return RandomSignedByte(0, max);
        }

        public static sbyte RandomSignedByte(sbyte min, sbyte max)
        {
            return (sbyte)_random.Next(min, max);
        }

        #endregion Byte

        #region Int16

        public static short RandomShort()
        {
            return RandomShort(short.MinValue, short.MaxValue);
        }

        public static short RandomShort(short max)
        {
            return RandomShort(0, max);
        }

        public static short RandomShort(short min, short max)
        {
            return (short)_random.Next(min, max);
        }

        #endregion Int16

        #region Int32

        public static int RandomInt()
        {
            return RandomInt(int.MinValue, int.MaxValue);
        }

        public static int RandomInt(int max)
        {
            return RandomInt(0, max);
        }

        public static int RandomInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        #endregion Int32

        #region Int64

        public static long RandomLong()
        {
            return RandomLong(long.MinValue, long.MaxValue);
        }

        public static long RandomLong(long max)
        {
            return RandomLong(0, max);
        }

        public static long RandomLong(long min, long max)
        {
            var buf = new byte[8];
            _random.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);
            var guagedLong = longRand;

            while (guagedLong < min)
            {
                guagedLong = (long)(longRand * (guagedLong > 0 ? 1.1 : -1.1));
            }

            while (guagedLong > max)
            {
                guagedLong = (long)(longRand % 1.1);
            }

            return guagedLong >= min || guagedLong <= max ? guagedLong : (min + max)/2;
        }

        #endregion Int64

        #region IntPtr

        public static IntPtr RandomIntPtr()
        {
            return RandomIntPtr(int.MinValue, int.MaxValue);
        }

        public static IntPtr RandomIntPtr(int max)
        {
            return RandomIntPtr(0, max);
        }

        public static IntPtr RandomIntPtr(int min, int max)
        {
            return new IntPtr(RandomInt(min, max));
        }

        #endregion IntPtr

        #region Decimal

        public static decimal RandomDecimal(short? percision = null)
        {
            return RandomDecimal(0, short.MaxValue, percision);
        }

        public static decimal RandomDecimal(short max, short? percision = null)
        {
            return RandomDecimal(0, max, percision);
        }

        public static decimal RandomDecimal(short min, short max, short? percision = null)
        {
            var left = RandomShort(min, max);
            var right = percision < 5
                    ? RandomShort(0, (short)(percision * 10 - 1))
                    : RandomShort(0, short.MaxValue);
            decimal value;

            unchecked
            {
                value = decimal.Parse($"{left}.{right}");
            }

            return value;
        }

        #endregion Decimal

        #region Double

        public static double RandomDouble()
        {
            return RandomDouble(int.MinValue, int.MaxValue);
        }

        public static double RandomDouble(int max)
        {
            return RandomDouble(0, max);
        }

        public static double RandomDouble(int min, int max)
        {
            return _random.Next(min, max) + _random.NextDouble();
        }

        #endregion Double
    }
}