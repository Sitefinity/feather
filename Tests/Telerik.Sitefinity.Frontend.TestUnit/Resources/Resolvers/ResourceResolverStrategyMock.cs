using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources.Resolvers
{
    /// <summary>
    /// This class represents a mocked version of ResourceResolverStrategy class meant to
    /// be used for testing purposes when there is no running instance of Siteifnity.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class ResourceResolverStrategyMock : ResourceResolverStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceResolverStrategyMock" /> class.
        /// </summary>
        public ResourceResolverStrategyMock() 
            : base()
        {
        }

        /// <summary>
        /// Removes the <see cref="DatabaseResourceResolver"/> node from the chain, because it requires a running instance of Sitefinity.
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
