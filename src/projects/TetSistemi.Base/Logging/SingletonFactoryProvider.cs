using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TetSistemi.Base.Class;
using TetSistemi.Base.Logger;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// IApplicationLog interface
    /// </summary>
    public class SingletonFactoryProvider : FactoryProvider
    {
        static object synch=new object();
        static SingletonFactoryProvider provider;

        static public FactoryProvider Provider
        {
            get
            {
                lock (synch)
                {
                    if (provider == null) { provider = new SingletonFactoryProvider(); }

                    return provider;
                }
            }
        }
    }
}
