using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Meziantou.DataGenerator.Utilities
{
    /// <summary>
    /// Extensions for <see cref="System.Random"/> class.
    /// </summary>
    public static class RandomUtilities
    {
        static readonly Random _random = new Random();
        public static Random Random
        {
            get { return _random; }
        }

        public const string LowerAlpha = "abcdefghijklmnopqrstuvwxyz";
        public const string Digits = "0123456789";
        public const string Specials = "&é~\"#'{([-|è`îïôöûüäâeê_\\ç^à@)]=}¨£$¤%ùµ*,?;.:/!§°+€<>";
        public const string UpperAlpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static T NextFromArray<T>(this Random random, T[] array)
        {
            if (random == null) throw new ArgumentNullException("random");
            if (array == null) throw new ArgumentNullException("array");

            if (array.Length == 0)
                throw new ArgumentException("Array is empty.", "array");

            int index = random.NextInt32(0, array.Length);
            return array[index];
        }

        public static T NextFromList<T>(this Random random, IList<T> list)
        {
            if (random == null) throw new ArgumentNullException("random");
            if (list == null) throw new ArgumentNullException("list");

            if (list.Count == 0)
                throw new ArgumentException("List is empty.", "list");

            int index = random.NextInt32(0, list.Count);
            return list[index];
        }

        public static T NextFromList<T>(this Random random, IReadOnlyList<T> list)
        {
            if (random == null) throw new ArgumentNullException("random");
            if (list == null) throw new ArgumentNullException("list");

            if (list.Count == 0)
                throw new ArgumentException("List is empty.", "list");

            int index = random.NextInt32(0, list.Count);
            return list[index];
        }

        public static bool NextBoolean(this System.Random random)
        {
            if (random == null) throw new ArgumentNullException("random");

            return random.Next(0, 2) != 0;
        }

        public static byte NextByte(this System.Random random, byte min = 0, byte max = byte.MaxValue)
        {
            if (random == null) throw new ArgumentNullException("random");

            return (byte)random.Next(min, max);
        }

        public static sbyte NextSByte(this System.Random random, sbyte min = 0, sbyte max = sbyte.MaxValue)
        {
            if (random == null) throw new ArgumentNullException("random");

            return (sbyte)random.Next(min, max);
        }

        public static byte[] NextBytes(this System.Random random, byte[] bytes)
        {
            if (random == null) throw new ArgumentNullException("random");

            random.NextBytes(bytes);

            return bytes;
        }

        public static DateTime NextDateTime(this System.Random random, DateTime min, DateTime max)
        {
            if (random == null) throw new ArgumentNullException("random");

            long diff = max.Ticks - min.Ticks;
            long range = (long)(diff * random.NextDouble());

            return min + new TimeSpan(range);
        }

        public static double NextDouble(this System.Random random, double min = 0D, double max = 1D)
        {
            if (random == null) throw new ArgumentNullException("random");


            return (random.NextDouble() * (max - min)) + min;
        }

        public static short NextInt16(this System.Random random, short min = (short)0, short max = short.MaxValue)
        {
            if (random == null) throw new ArgumentNullException("random");


            return (short)random.Next(min, max);
        }

        public static int NextInt32(this System.Random random, int min = 0, int max = int.MaxValue)
        {
            if (random == null) throw new ArgumentNullException("random");

            if (min == max)
            {
                return min;
            }

            return random.Next(min, max);
        }

        public static long NextInt64(this System.Random random, long min = 0L, long max = long.MaxValue)
        {
            if (random == null) throw new ArgumentNullException("random");

            if (min == max)
            {
                return min;
            }

            return (long)((random.NextDouble() * (max - min)) + min);
        }

        public static float NextSingle(this System.Random random, float min = 0f, float max = 1f)
        {
            if (random == null) throw new ArgumentNullException("random");

            return (float)random.NextDouble(min, max);
        }

        public static string NextString(this System.Random random, int length, string chars = LowerAlpha + UpperAlpha + Digits + Specials)
        {
            return NextString(random, length, length, chars);
        }

        public static string NextString(this System.Random random, int minLength, int maxLength, string chars = LowerAlpha + UpperAlpha + Digits + Specials)
        {
            if (random == null) throw new ArgumentNullException("random");

            int length = minLength + random.Next(0, maxLength - minLength + 1); // length of the string

            int max = chars.Length; // number of available characters
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(0, max)]);
            }

            return sb.ToString();
        }

        [DebuggerDisplay("{Begin}-{End}")]
        public class Range
        {
            public int Begin { get; set; }
            public int End { get; set; }

            public Range(int begin, int end)
            {
                Begin = begin;
                End = end;
            }
        }

        public static string NextUnicodeString(this Random random, int minLength, int maxLength)
        {
            var ranges = new[]
            {
                new Range(0x0, 0xfdcf),
                new Range(0xfdf0 , 0xfffd),
                new Range(0x10000, 0x1fffd),
                new Range(0x20000, 0x2fffd),
                new Range(0x30000, 0x3fffd),
                new Range(0x40000, 0x4fffd),
                new Range(0x50000, 0x5fffd),
                new Range(0x60000, 0x6fffd),
                new Range(0x70000, 0x7fffd),
                new Range(0x80000, 0x8fffd),
                new Range(0x90000, 0x9fffd),
                new Range(0xa0000, 0xafffd),
                new Range(0xb0000, 0xbfffd),
                new Range(0xc0000, 0xcfffd),
                new Range(0xd0000, 0xdfffd),
                new Range(0xe0000, 0xefffd),
                new Range(0xf0000, 0xffffd),
                new Range(0x100000, 0x10fffd)
            };

            return NextUnicodeString(random, minLength, maxLength, ranges);
        }

        public static string NextUnicodeString(this Random random, int minLength, int maxLength, Range range)
        {
            if (range == null) throw new ArgumentNullException("range");
            return NextUnicodeString(random, minLength, maxLength, new[] { range });
        }

        public static string NextUnicodeString(this Random random, int minLength, int maxLength, IReadOnlyList<Range> ranges)
        {
            if (random == null) throw new ArgumentNullException("random");
            if (ranges == null) throw new ArgumentNullException("ranges");

            int length = minLength + random.Next(0, maxLength - minLength + 1); // length of the string

            var builder = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                var rangeIndex = random.Next(ranges.Count);
                var range = ranges[rangeIndex];
                int c = random.NextInt32(range.Begin, range.End);
                builder.Append((char)c);
            }

            return builder.ToString();
        }

        public static ushort NextUInt16(this System.Random random, ushort min = (ushort)0, ushort max = ushort.MaxValue)
        {
            if (random == null) throw new ArgumentNullException("random");

            return (ushort)random.Next(min, max);
        }

        public static uint NextUInt32(this System.Random random, uint min = 0u, uint max = uint.MaxValue)
        {
            if (random == null) throw new ArgumentNullException("random");

            return (uint)random.NextInt64(min, max);
        }

        public static ulong NextUInt64(this System.Random random, ulong min = 0ul, ulong max = ulong.MaxValue)
        {
            if (random == null) throw new ArgumentNullException("random");

            byte[] buffer = new byte[sizeof(long)];
            random.NextBytes(buffer);
            return (BitConverter.ToUInt64(buffer, 0) * (max - min) / ulong.MaxValue) + min;
        }

        public static decimal NextDecimal(this System.Random random, decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
        {
            if (random == null) throw new ArgumentNullException("random");

            return (((decimal)random.NextDouble()) * (max - min)) + min;
        }
    }
}

