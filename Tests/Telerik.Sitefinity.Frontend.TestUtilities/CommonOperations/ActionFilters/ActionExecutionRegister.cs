using System.Collections.Generic;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations.ActionFilters
{
    /// <summary>
    /// This class represents register for executed actions.
    /// </summary>
    public static class ActionExecutionRegister
    {
        /// <summary>
        /// Gets the executed action information.
        /// </summary>
        /// <value>
        /// The executed action information.
        /// </value>
        public static IList<ActionInfo> ExecutedActionInfos
        {
            get
            {
                return ActionExecutionRegister.executedActionInfos;
            }
        }

        private static IList<ActionInfo> executedActionInfos = new List<ActionInfo>();
    }
}
