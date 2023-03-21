using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// Flow log Verbose enum
    /// </summary>
    public enum FlowVerbose
    {
        /// <summary>
        /// MACRO
        /// </summary>
        V01,
        /// <summary>
        /// DETAILED
        /// </summary>
        V02,
        /// <summary>
        /// EXTREME DETAILED
        /// </summary>
        V03,
    }

    /// <summary>
    /// Flow log Result enum
    /// </summary>
    public enum FlowResult
    {
        /// <summary>
        /// SUCCESSFULL
        /// </summary>
        R01,
        /// <summary>
        /// WARING
        /// </summary>
        R02,
        /// <summary>
        /// FAILED
        /// </summary>
        R03,
    }

    public interface IFlowLog
    {
        /// <summary>
        /// Writes a flow log
        /// </summary>
        /// <param name="level">Log Level</param>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="verbose">Flow log verbose level</param>
        /// <param name="logid">Flow log identifier</param>
        /// <param name="logsource">Flow log source</param>
        /// <param name="user">Flow log user</param>
        /// <param name="action">Flow log action</param>
        /// <param name="result">Flow log result</param>
        /// <param name="message">Flow log message</param>
        void Log(LogLevels level, object caller, 
            FlowVerbose verbose, string logid, 
            string logsource, string user, 
            string action, FlowResult result, 
            string message);
    }
}
