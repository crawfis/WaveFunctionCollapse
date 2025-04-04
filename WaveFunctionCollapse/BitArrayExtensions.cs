using System.Collections;

namespace CrawfisSoftware.Collections
{
    internal static class BitArrayExtensions
    {
        public static int SetBitsCounts(this BitArray bitArray)
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
    }
}