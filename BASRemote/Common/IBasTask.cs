namespace BASRemote.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBasTask
    {
        /// <summary>
        /// 
        /// </summary>
        int StartedAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int? BrowserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int? TaskId { get; set; }
    }
}