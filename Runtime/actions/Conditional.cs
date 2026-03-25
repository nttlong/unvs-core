
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
namespace unvs.actions
{
    [Serializable]
    public abstract class Conditional : ActionBase
    {
        [SerializeReference] public ActionBase ifTrue;
        [SerializeReference] public ActionBase ifFalse;

        public abstract UniTask<bool> CheckAsync(MonoBehaviour source, MonoBehaviour target, CancellationToken ct = default);

        // Khi lệnh IF được thực thi, nó kiểm tra điều kiện rồi mới chạy nhánh con
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            bool result = await CheckAsync(Sender.Source, Sender.Target);
            if (result)
            {
                if (ifTrue != null) await ifTrue.ExecuteAsync(Sender);
            }
            else
            {
                if (ifFalse != null) await ifFalse.ExecuteAsync(Sender);
            }
            
        }
    }
}