using System;
namespace Kademlia.Core
{
    public class Constants
    {

        public int K { get; set; }
        public int M { get; set; }
        public int N { get; set; }
        public int Alpha { get; set; }

        public Constants(int n, int m, int k, int alpha)
        {
            this.K = k;
            this.M = m;
            this.N = n;
            this.Alpha = alpha;
        }
    }
}
