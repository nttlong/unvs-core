using System;
using System.Collections;
using UnityEngine;

namespace unvs.interfaces.sys
{
    
    public interface IRealtimeStats
    {
        
        void SaveTrack(string name, object value);
    }
}