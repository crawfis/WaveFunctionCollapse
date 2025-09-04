// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
using System.Collections;
using System.Text;

namespace CrawfisSoftware.Extensions
{
    internal static class BitArrayExtensions
    {
        public static int TrueCount(this BitArray bitArray)
        {
            int bitsSetToTrue = 0;
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                {
                    bitsSetToTrue++;
                }
            }

            return bitsSetToTrue;
        }
        public static int FalseCount(this BitArray bitArray)
        {
            return bitArray.Length - bitArray.TrueCount();
        }

        // Bitwise AND (returns new BitArray)
        public static BitArray AndClone(this BitArray a, BitArray b)
        {
            var result = (BitArray)a.Clone();
            result.And(b);
            return result;
        }

        // Bitwise OR (returns new BitArray)
        public static BitArray OrClone(this BitArray a, BitArray b)
        {
            var result = (BitArray)a.Clone();
            result.Or(b);
            return result;
        }

        // Bitwise XOR (returns new BitArray)
        public static BitArray XorClone(this BitArray a, BitArray b)
        {
            var result = (BitArray)a.Clone();
            result.Xor(b);
            return result;
        }

        // Bitwise NOT (returns new BitArray)
        public static BitArray NotClone(this BitArray a)
        {
            var result = (BitArray)a.Clone();
            result.Not();
            return result;
        }

        // Convert BitArray to bool[]
        public static bool[] ToBoolArray(this BitArray bitArray)
        {
            var array = new bool[bitArray.Count];
            bitArray.CopyTo(array, 0);
            return array;
        }

        // Convert BitArray to int[]
        public static int[] ToIntArray(this BitArray bitArray)
        {
            int intLength = (bitArray.Count + 31) / 32;
            var array = new int[intLength];
            bitArray.CopyTo(array, 0);
            return array;
        }

        // Convert BitArray to byte[]
        public static byte[] ToByteArray(this BitArray bitArray)
        {
            int byteLength = (bitArray.Count + 7) / 8;
            var array = new byte[byteLength];
            bitArray.CopyTo(array, 0);
            return array;
        }

        // Create BitArray from bool[]
        public static BitArray ToBitArray(this bool[] array)
        {
            return new BitArray(array);
        }

        // Create BitArray from int[]
        public static BitArray ToBitArray(this int[] array, int length = -1)
        {
            var bitArray = new BitArray(array);
            if (length > 0 && bitArray.Length != length)
                bitArray.Length = length;
            return bitArray;
        }

        // Create BitArray from byte[]
        public static BitArray ToBitArray(this byte[] array, int length = -1)
        {
            var bitArray = new BitArray(array);
            if (length > 0 && bitArray.Length != length)
                bitArray.Length = length;
            return bitArray;
        }

        // Find the first index of true
        public static int FirstIndexOfTrue(this BitArray bitArray)
        {
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i]) return i;
            }
            return -1;
        }

        // Find the first index of false
        public static int FirstIndexOfFalse(this BitArray bitArray)
        {
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (!bitArray[i]) return i;
            }
            return -1;
        }

        // Find the last index of true
        public static int LastIndexOfTrue(this BitArray bitArray)
        {
            for (int i = bitArray.Length - 1; i >= 0; i--)
            {
                if (bitArray[i]) return i;
            }
            return -1;
        }
        // Find the last index of false
        public static int LastIndexOfFalse(this BitArray bitArray)
        {
            for (int i = bitArray.Length - 1; i >= 0; i--)
            {
                if (!bitArray[i]) return i;
            }
            return -1;
        }

        // Compare two BitArrays for equality
        public static bool SequenceEqual(this BitArray a, BitArray b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        // Convert BitArray to a string of '0' and '1'
        public static string ToBitString(this BitArray bitArray, char trueChar = '1', char falseChar = '0')
        {
            if (bitArray == null) return string.Empty;
            var sb = new StringBuilder(bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                sb.Append(bitArray[i] ? trueChar : falseChar);
            }
            return sb.ToString();
        }

        // Set all bits to a value (fluent)
        public static BitArray SetAllBits(this BitArray bitArray, bool value)
        {
            bitArray.SetAll(value);
            return bitArray;
        }

        // Clone BitArray (deep copy)
        public static BitArray CloneBits(this BitArray bitArray)
        {
            return (BitArray)bitArray.Clone();
        }
    }
}