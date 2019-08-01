using System;
using System.Linq;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Comments;
using Telerik.Sitefinity.Services.Comments.Impl.Notifications;
using Telerik.Sitefinity.Services.Comments.Notifications;
using Telerik.Sitefinity.Services.Notifications;

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
        protected override IMessageTemplateRequest GetMessageTemplate(ICommentEvent @event)
        {
            if (@event == null || @event.Item == null)
                return null;

            IMessageTemplateRequest messageTemplate;
            IComment comment = @event.Item;
            ICommentService cs = SystemManager.GetCommentsService();
            IThread thread = cs.GetThread(comment.ThreadKey);

            if (this.IsReviewThread(thread))
            {
                messageTemplate = this.GetNewReviewMessageTemplate();
            }
            else
            {
                messageTemplate = this.GetNewCommentMessageTemplate();
            }

            return messageTemplate;
        }

        private bool IsReviewThread(IThread thread)
        {
            var isReview = false;

            if (thread.Behavior.IsNullOrEmpty())
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
