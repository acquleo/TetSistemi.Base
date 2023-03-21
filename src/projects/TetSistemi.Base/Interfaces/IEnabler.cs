using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Interfaces
{
    public interface IEnabler
    {
        void Enable();
        void Disable();
        bool IsEnabled();
    }
}
