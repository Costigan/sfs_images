using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace sfs_images
{
    public class LitPatches
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Length { get; set; }

        public UInt32[] Data = null;

        public LitPatches(int width, int height)
        {
            Length = width * height;
            Data = new uint[Length];
            Width = width;
            Height = height;
        }

        public bool this[int r, int c]
        {
            get
            {
                return this[r * Width + c];
            }
            set
            {
                this[r * Width + c] = value;
            }
        }

        public bool this[int i]
        {
            get
            {
                var i1 = Math.DivRem(i, 32, out int r);
                return (Data[i1] & (1 << r)) != 0;
            }
            set
            {
                var i1 = Math.DivRem(i, 32, out int r);
                Data[i1] |= (uint)(1 << r);
            }
        }

        public int Count
        {
            get
            {
                var sum = 0;
                var len = Length;
                for (var i = 0; i < len; i++)
                    sum += BitCount(Data[i]);
                return sum;
            }
        }

        public void Copy(LitPatches other)
        {
            Debug.Assert(Length == other.Length && Width == other.Width && Height == other.Height);
            var len = Length;
            var other_data = other.Data;
            for (var i = 0; i < len; i++)
                Data[i] = other_data[i];
        }

        public void Or(LitPatches other)
        {
            Debug.Assert(Length == other.Length && Width == other.Width && Height == other.Height);
            var len = Length;
            var other_data = other.Data;
            for (var i = 0; i < len; i++)
                Data[i] |= other_data[i];
        }

        public bool IsEqual(LitPatches other)
        {
            if (Length != other.Length || Width != other.Width || Height != other.Height) return false;
            var len = Length;
            var other_data = other.Data;
            for (var i = 0; i < len; i++)
                if (Data[i] != other_data[i]) return false;
            return true;
        }

        public void Clear()
        {
            for (var i = 0; i < Data.Length; i++)
                Data[i] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitCount(UInt32 i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            i = (i + (i >> 4)) & 0x0f0f0f0f;
            i = i + (i >> 8);
            i = i + (i >> 16);
            return (int)(i & 0x3f);
        }

        public int SlowBitCount(int i)
        {
            int count = 0;
            for (int j = 0; j < 32; j++)
            {
                if (i < 0) count++;
                i <<= 1;
            }
            return count;
        }

        public void Test1()
        {
            for (var i = 0; i < 1000000; i++)
                Debug.Assert(BitCount((UInt32)i) == SlowBitCount(i));
        }
    }
}
