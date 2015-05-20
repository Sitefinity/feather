using System;
using System.Linq;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Comments;
using Telerik.Sitefinity.Services.Comments.Impl.Notifications;
using Telerik.Sitefinity.Services.Comments.Notifications;

namespace Telerik.Sitefinity.Frontend.Modules.Comments
{
    /// <summary>
    /// Notification strategy that is responsible for notifications for <see cref="IComment"/> objects..
    /// </summary>
    internal class ReviewNotificationStrategy : CommentNotificationsStrategy, ICommentNotificationsStrategy
    {
        /// <summary>
        /// Gets the message template unique identifier for the specified <see cref="ICommentEvent"/> event.
        /// </summary>
        /// <param name="event">The event.</param>
        protected override Guid GetMessageTemplateId(ICommentEvent @event)
        {
            IComment comment = @event.Item;

            ICommentService cs = SystemManager.GetCommentsService();
            IThread thread = cs.GetThread(comment.ThreadKey);

            var ns = SystemManager.GetNotificationService();
            Guid messageTemplateId;

            if (this.IsReviewThread(thread))
            {
                messageTemplateId = ns.GetMessageTemplates(this.ServiceContext, null)
                    .Where(mt => mt.Subject == "A new review was posted")
                    .Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                messageTemplateId = ns.GetMessageTemplates(this.ServiceContext, null)
                    .Where(mt => mt.Subject == "A new comment was posted")
                    .Select(m => m.Id).FirstOrDefault();
            }

            if (messageTemplateId == Guid.Empty)
            {
                messageTemplateId = base.GetMessageTemplateId(@event);
            }

            return messageTemplateId;
        }

        private bool IsReviewThread(IThread thread)
        {
            var isReview = false;

            if (thread.Behavior == null)
            {
                isReview = thread.Key.EndsWith("_review", StringComparison.Ordinal);
            }
            else
            {
                isReview = thread.Behavior == "review";
            }

            return isReview;
        }
    }
}
