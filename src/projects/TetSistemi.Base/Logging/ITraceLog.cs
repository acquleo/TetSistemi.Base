using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TetSistemi.Base.Logger;

namespace TetSistemi.Base.Logging
{
    #region Enum


    #endregion

    /// <summary>
    /// Trace communication direction
    /// </summary>
    public enum TraceDirections
    {
        /// <summary>
        /// Input data
        /// </summary>
        Input = 0,
        /// <summary>
        /// Data output
        /// </summary>
        Output = 1,

    }

    public interface ITraceLog
    {

        /// <summary>
        /// Writes a data trace log
        /// </summary>
        /// <param name="_currentDevice">Local endpoint</param>
        /// <param name="_remoteDevice">Remote endpoint</param>
        /// <param name="_data">Data log</param>
        /// <param name="_direction">Trace direction</param>
        /// <param name="_description">optional description</param>
        /// <param name="_printTypeByteArray">byte array format type</param>
        void Log(string _currentDevice, string _remoteDevice, byte[] _data, TraceDirections _direction,
            string _description, PrintTypeByteArray _printTypeByteArray);

        /// <summary>
        /// Writes a data trace log
        /// </summary>
        /// <param name="_currentDevice">Local endpoint</param>
        /// <param name="_remoteDevice">Remote endpoint</param>
        /// <param name="_data">Data log</param>
        /// <param name="_direction">Trace direction</param>
        /// <param name="_description">Optional description</param>
        void Log(string _currentDevice, string _remoteDevice, string _data, TraceDirections _direction,
            string _description);
    }
}
