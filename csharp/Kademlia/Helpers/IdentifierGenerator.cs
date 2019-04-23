using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Kademlia.Core;
using System.Numerics;
using System.Security.Cryptography;

namespace Kademlia.Helpers
{
    public class IdentifierGenerator
    {

        private IList<Identifier> extractedIdentifiers = new List<Identifier>();
        private RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

        public static IdentifierGenerator Instance { get; } = new IdentifierGenerator();

        private IdentifierGenerator()
        {

        }

        public BigInteger GetRandomInRange(BigInteger min, BigInteger max)
        {
            if (min > max)
            {
                var buff = min;
                min = max;
                max = buff;
            }

            // offset to set min = 0
            BigInteger offset = -min;
            max += offset;

            return GetRandomInRangeFromZeroTo(max) - offset;
        }

        private BigInteger GetRandomInRangeFromZeroTo(BigInteger max)
        {
            BigInteger value;
            var bytes = max.ToByteArray();

            // count how many bits of the most significant byte are 0
            // NOTE: sign bit is always 0 because `max` must always be positive
            byte zeroBitsMask = 0b00000000;

            var mostSignificantByte = bytes[bytes.Length - 1];

            // we try to set to 0 as many bits as there are in the most significant byte, starting from the left (most significant bits first)
            // NOTE: `i` starts from 7 because the sign bit is always 0
            for (var i = 7; i >= 0; i--)
            {
                // we keep iterating until we find the most significant non-0 bit
                if ((mostSignificantByte & (0b1 << i)) != 0)
                {
                    var zeroBits = 7 - i;
                    zeroBitsMask = (byte)(0b11111111 >> zeroBits);
                    break;
                }
            }

            do
            {
                randomNumberGenerator.GetBytes(bytes);

                // set most significant bits to 0 (because `value > max` if any of these bits is 1)
                bytes[bytes.Length - 1] &= zeroBitsMask;

                value = new BigInteger(bytes);

                // `value > max` 50% of the times, in which case the fastest way to keep the distribution uniform is to try again
            } while (value > max);

            return value;
        }


        public Identifier GenerateIdentifier()
        {
            var limit = BigInteger.Pow(2, Coordinator.Constants.M) - 1;

            Identifier identifier;
            do
            {
                identifier = new Identifier(
                    GetRandomInRange(BigInteger.Zero, limit)
                );
            } while (extractedIdentifiers.Contains(identifier));

            extractedIdentifiers.Add(identifier);

            return identifier;
        }

        public Identifier GetRandomExistingId()
        {
            return extractedIdentifiers[new Random().Next(extractedIdentifiers.Count)];
        }

        public Identifier GetUniqueRandomInRange(BigInteger min, BigInteger max)
        {
            BigInteger random;
            do
            {
                random = GetRandomInRange(min, max);
            } while (extractedIdentifiers.FirstOrDefault(id => id.Equals(random)) != null);

            return new Identifier(random);
        }

        public Identifier GenerateRandomInBucket(int bucketIndex)
        {
            BigInteger low = BigInteger.Pow(2, bucketIndex);
            BigInteger high = BigInteger.Pow(2, bucketIndex + 1) - 1;
            return GetUniqueRandomInRange(low, high);
        }
    }
}
