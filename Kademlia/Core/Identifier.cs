using System.Numerics;

namespace Kademlia.Core
{
    /// <summary>
    /// The identifier of a node.
    /// </summary>
    public class Identifier
    {
        #region Fields & Properties

        /// <summary>
        /// The identifier.
        /// </summary>
        private readonly BigInteger id;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kademlia.Core.Identifier"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public Identifier(BigInteger id)
        {
            this.id = id;
        }

        #endregion

        #region Public APIs

        /// <summary>
        /// Gets the XOR distance to the target.
        /// </summary>
        /// <returns>The distance to.</returns>
        /// <param name="target">Target.</param>
        public BigInteger GetDistanceTo(Identifier target)
        {
            return id ^ target.id;
        }

        /// <summary>
        /// Determines whether the specified <see cref="long"/> is equal to the current <see cref="T:Kademlia.Core.Identifier"/>.
        /// </summary>
        /// <param name="id">The <see cref="long"/> to compare with the current <see cref="T:Kademlia.Core.Identifier"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="long"/> is equal to the current
        /// <see cref="T:Kademlia.Core.Identifier"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(long id)
        {
            return id == this.id;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:Kademlia.Core.Identifier"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:Kademlia.Core.Identifier"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="T:Kademlia.Core.Identifier"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Identifier target))
                return false;

            return target.id == id;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="T:Kademlia.Core.Identifier"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Kademlia.Core.Identifier"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Kademlia.Core.Identifier"/>.</returns>
        public override string ToString()
        {
            return id.ToString();
        }

        #endregion
    }
}
