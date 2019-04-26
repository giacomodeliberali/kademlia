namespace Kademlia.Core
{
    /// <summary>
    /// Contains the constants used during the current run
    /// </summary>
    public class Constants
    {
        #region Fields & Properties

        /// <summary>
        /// Gets or sets the number of entries in each buckets.
        /// </summary>
        /// <value>The k.</value>
        public int K { get; set; }

        /// <summary>
        /// Gets or sets the number of buckets.
        /// </summary>
        /// <value>The m.</value>
        public int M { get; set; }

        /// <summary>
        /// Gets or sets the number of nodes that will join the network.
        /// </summary>
        /// <value>The n.</value>
        public int N { get; set; }

        /// <summary>
        /// Gets or sets the degree of fake parallelism of the lookup procedure.
        /// </summary>
        /// <value>The alpha.</value>
        public int Alpha { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kademlia.Core.Constants"/> class.
        /// </summary>
        /// <param name="n">N.</param>
        /// <param name="m">M.</param>
        /// <param name="k">K.</param>
        /// <param name="alpha">Alpha.</param>
        public Constants(int n, int m, int k, int alpha)
        {
            Update(n, m, k, alpha);
        }

        #endregion

        #region Public APIs

        /// <summary>
        /// Update the specified n, m, k and alpha.
        /// </summary>
        /// <param name="n">N.</param>
        /// <param name="m">M.</param>
        /// <param name="k">K.</param>
        /// <param name="alpha">Alpha.</param>
        public void Update(int n, int m, int k, int alpha)
        {
            K = k;
            M = m;
            N = n;
            Alpha = alpha;
        }

        #endregion
    }
}
