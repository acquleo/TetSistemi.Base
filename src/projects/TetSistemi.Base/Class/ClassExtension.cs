using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Class
{
    /// <summary>
    /// Generic message definition
    /// </summary>
    public static class ClassExtension
    {
        public static bool Is<Tclass>(this object msg)
            where Tclass : class
        {
            return msg is Tclass;
        }

        public static Tclass As<Tclass>(this object msg)
            where Tclass : class
        {
            return (Tclass)msg;
        }
    }
}
