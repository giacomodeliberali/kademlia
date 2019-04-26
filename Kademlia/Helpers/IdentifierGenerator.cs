using System.Linq;
using System.Collections.Generic;
using Kademlia.Core;
using System.Numerics;
using System.Security.Cryptography;

namespace Kademlia.Helpers
{
    /// <summary>
    /// The <see cref="T:Kademlia.Core.Identifier"/> generator.
    /// </summary>
    public class IdentifierGenerator
    {
        #region Fields & Properties

        public static IdentifierGenerator Instance { get; } = new IdentifierGenerator();

        /// <summary>
        /// The extracted identifiers.
        /// </summary>
        private readonly IList<Identifier> extractedIdentifiers = new List<Identifier>();

        /// <summary>
        /// The random number generator.
        /// </summary>
        private readonly RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

        /// <summary>
        /// The identifiers limit.
        /// </summary>
        private readonly BigInteger identifiersLimit;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kademlia.Helpers.IdentifierGenerator"/> class.
        /// </summary>
        private IdentifierGenerator()
        {
            identifiersLimit = BigInteger.Pow(2, Coordinator.Constants.M) - 1;
        }

        #endregion

        #region Public APIs

        /// <summary>
        /// Generates a random identifier.
        /// </summary>
        /// <returns>The identifier.</returns>
        public Identifier GenerateIdentifier()
        {
            Identifier identifier;
            do
            {
                identifier = new Identifier(
                    GetRandomInRange(BigInteger.Zero, identifiersLimit)
                );
            } while (extractedIdentifiers.Contains(identifier));

            extractedIdentifiers.Add(identifier);

            return identifier;
        }

        /// <summary>
        /// Generates a random identifier in the specified bucket range.
        /// </summary>
        /// <returns>The random in bucket.</returns>
        /// <param name="bucketIndex">Bucket index.</param>
        public Identifier GenerateRandomInBucket(int bucketIndex)
        {
            BigInteger low = BigInteger.Pow(2, bucketIndex);
            BigInteger high = BigInteger.Pow(2, bucketIndex + 1) - 1;
            return GetUniqueRandomInRange(low, high);
        }

        #endregion

        #region Private APIs

        /// <summary>
        /// Gets a unique random in range without adding it to the generated list.
        /// </summary>
        /// <returns>The unique random in range.</returns>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        private Identifier GetUniqueRandomInRange(BigInteger min, BigInteger max)
        {
            BigInteger random;
            do
            {
                random = GetRandomInRange(min, max);
            } while (extractedIdentifiers.FirstOrDefault(id => id.Equals(random)) != null);

            return new Identifier(random);
        }

        /// <summary>
        /// Gets the random in range.
        /// </summary>
        /// <returns>The random in range.</returns>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        private BigInteger GetRandomInRange(BigInteger min, BigInteger max)
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

        /// <summary>
        /// Gets the random in range from zero to max.
        /// </summary>
        /// <returns>The random in range from zero to max.</returns>
        /// <param name="max">Max.</param>
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

        #endregion
    }
}
