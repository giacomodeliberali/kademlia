using System.Numerics;

namespace Kademlia.Core
{
    public class Identifier
    {
        private readonly BigInteger id;

        public Identifier(BigInteger id)
        {
            this.id = id;
        }

        public BigInteger GetDistanceTo(Identifier target)
        {
            return id ^ target.id;
        }

        public bool Equals(long id)
        {
            return id == this.id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Identifier target))
                return false;

            return target.id == id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override string ToString()
        {
            return id.ToString();
        }
    }
}
