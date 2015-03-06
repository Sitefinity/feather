using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.Notifications
{
    /// <summary>
    /// This class contains helpers for working with email templates.
    /// </summary>
    public static class EmailTemplateHelper
    {
        /// <summary>
        /// Gets the email templates.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <returns></returns>
        public static IDictionary<Guid, string> GetEmailTemplates(string filterExpression)
        {
            var pageManager = PageManager.GetManager();
            IQueryable<ControlPresentation> allTemplates;
            allTemplates = pageManager.GetPresentationItems<ControlPresentation>();
            var layoutTemplates = allTemplates.Where(tmpl => tmpl.DataType == Presentation.EmailTemplate);
            int? totalCount = 0;
            var filteredTemplates = DataProviderBase.SetExpressions(layoutTemplates, filterExpression, string.Empty, 0, 0, ref totalCount);

            IDictionary<Guid, string> templateViewModel = new Dictionary<Guid, string>();
            foreach (var template in filteredTemplates)
            {
                templateViewModel.Add(template.Id, template.Name);
            }

            return templateViewModel;
        }
    }
}
