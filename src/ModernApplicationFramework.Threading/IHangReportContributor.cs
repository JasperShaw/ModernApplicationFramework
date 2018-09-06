namespace ModernApplicationFramework.Threading
{
    /// <summary>
    /// Provides a facility to produce reports that may be useful when analyzing hangs.
    /// </summary>
    public interface IHangReportContributor
    {
        /// <summary>
        /// Contributes data for a hang report.
        /// </summary>
        /// <returns>The hang report contribution. Null values should be ignored.</returns>
        HangReportContribution GetHangReport();
    }
}