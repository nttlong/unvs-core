using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace unvs.ext
{
    public static class CancellationTokenSourceExt
    {
        public static CancellationTokenSource Refresh(this CancellationTokenSource _cts)
        {

            _cts?.Cancel();
            // _cts?.Dispose();
            _cts = new CancellationTokenSource();
            return _cts;
        }
        public static CancellationTokenSource Stop(this CancellationTokenSource _cts)
        {

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            return _cts;
        }
    }
}
