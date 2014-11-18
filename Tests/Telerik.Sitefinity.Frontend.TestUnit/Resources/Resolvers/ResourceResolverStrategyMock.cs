using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources.Resolvers
{
    /// <summary>
    /// This class represents a mocked version of ResourceResolverStrategy class, that is meant to
    /// be used without the requirement of running in Sitefinity context
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class ResourceResolverStrategyMock : ResourceResolverStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceResolverStrategyMock" /> class.
        /// </summary>
        public ResourceResolverStrategyMock() : base()
        {
        }

        /// <summary>
        /// Initializes the chain with the default nodes. Then skipps the DatabaseResourceResolver node,
        /// because DatabaseResourceResolver requires a connection to Sitefinity Database
        /// </summary>
        protected override void InitializeChain()
        {
            base.InitializeChain();

            if (this.First.GetType() == typeof(DatabaseResourceResolver))
            {
                this.SetFirst(this.First.Next);
            }
            else
            {
                var resolver = this.First;

                while (resolver.Next != null)
                {
                    if (resolver.Next.GetType() == typeof(DatabaseResourceResolver))
                        resolver.SetNext(resolver.Next.Next);

                    resolver = resolver.Next;
                }
            }
        }
    }
}
