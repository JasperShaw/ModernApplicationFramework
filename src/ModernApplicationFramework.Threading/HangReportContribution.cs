using System.Collections.Generic;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    /// <summary>
    /// A contribution to an aggregate hang report.
    /// </summary>
    public class HangReportContribution
    {
        /// <summary>
        /// Gets the content of the hang report.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the MIME type for the content.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets the suggested filename for the content.
        /// </summary>
        public string ContentName { get; }

        /// <summary>
        /// Gets the nested hang reports, if any.
        /// </summary>
        /// <value>A read only collection, or <c>null</c>.</value>
        public IReadOnlyCollection<HangReportContribution> NestedReports { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HangReportContribution"/> class.
        /// </summary>
        /// <param name="content">The content for the hang report.</param>
        /// <param name="contentType">The MIME type of the attached content.</param>
        /// <param name="contentName">The suggested filename of the content when it is attached in a report.</param>
        public HangReportContribution(string content, string contentType, string contentName)
        {
            Validate.IsNotNull(content, nameof(content));
            Content = content;
            ContentType = contentType;
            ContentName = contentName;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ModernApplicationFramework.Threading.HangReportContribution" /> class.
        /// </summary>
        /// <param name="content">The content for the hang report.</param>
        /// <param name="contentType">The MIME type of the attached content.</param>
        /// <param name="contentName">The suggested filename of the content when it is attached in a report.</param>
        /// <param name="nestedReports">Nested reports.</param>
        public HangReportContribution(string content, string contentType, string contentName, params HangReportContribution[] nestedReports)
            : this(content, contentType, contentName)
        {
            NestedReports = nestedReports;
        }
    }
}