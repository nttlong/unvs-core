using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using unvs.actions;

namespace unvs.ext
{

    public static class ActionEntryExtension
    {

        private static readonly Dictionary<System.Type, Dictionary<string, System.Reflection.MethodInfo>> methodCache =
        new Dictionary<System.Type, Dictionary<string, System.Reflection.MethodInfo>>();

        public static async UniTask ExecuteAllActions(this List<ActionEntry> actions, MonoBehaviour source, MonoBehaviour target, System.Threading.CancellationTokenSource cts)
        {
            if (source == null)
            {
                Debug.LogError("ExecuteAllActions: Sender không được để trống!");
                return;
            }
            var arg = new ActionSender()
            {
                Cts = cts,
                Source = source,
                Target = target,
                Ok = true,
            };
            foreach (var action in actions)
            {
                // 1. Kiểm tra tính hợp lệ của Action
                if (!string.IsNullOrEmpty(action.scriptClassName) && !string.IsNullOrEmpty(action.functionName))
                {
                    System.Type scriptType = System.Type.GetType(action.scriptClassName);
                    if (scriptType == null) continue;

                    // 2. TÌM HOẶC TẠO INSTANCE (Đã sửa theo yêu cầu của bạn)
                    var instance = source.gameObject.GetComponent(scriptType);
                    if (instance == null)
                    {
                        instance = source.gameObject.AddComponent(scriptType);
                    }

                    // 3. LẤY METHOD TỪ CACHE (Để không phải dùng Reflection liên tục)
                    if (!methodCache.TryGetValue(scriptType, out var methodsInType))
                    {
                        methodsInType = new Dictionary<string, System.Reflection.MethodInfo>();
                        methodCache[scriptType] = methodsInType;
                    }

                    if (!methodsInType.TryGetValue(action.functionName, out System.Reflection.MethodInfo method))
                    {
                        // 1. Lấy method dựa trên tên
                        method = scriptType.GetMethod(action.functionName,
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic);

                        if (method != null)
                        {
                            // 2. Kiểm tra các tham số (Parameters)
                            var parameters = method.GetParameters();
                            bool isValid = parameters.Length == 1 &&
                                           //typeof(MonoBehaviour).IsAssignableFrom(parameters[0].ParameterType) &&
                                           //typeof(MonoBehaviour).IsAssignableFrom(parameters[1].ParameterType) &&
                                           parameters[0].ParameterType == typeof(ActionSender);

                            // 3. Kiểm tra kiểu trả về (Return Type) phải là UniTask
                            bool isUniTask = method.ReturnType == typeof(Cysharp.Threading.Tasks.UniTask) || method.ReturnType == typeof(UniTask<ActionSender>);

                            if (!isValid || !isUniTask)
                            {
                                Debug.LogError($"[ActionError] Hàm '{action.functionName}' trong '{scriptType.Name}' sai Signature!\n" +
                                               "Yêu cầu: public async UniTask TênHàm(MonoBehaviour sender, CancellationTokenSource cts)");
                                method = null; // Đánh dấu null để không thực thi sai
                            }
                        }

                        // Lưu vào cache (kể cả null để lần sau không quét lại hàm lỗi)
                        methodsInType[action.functionName] = method;
                    }

                    // 4. THỰC THI HÀM
                    if (method != null)
                    {
                        // Truyền 'sender' vào tham số (MonoBehaviour Sender) của hàm đíchvar

                        var result = method.Invoke(instance, new object[] { arg });

                        if (result != null)
                        {
                            // Kiểm tra xem method return type có phải là UniTask<ActionSender> không
                            if (method.ReturnType == typeof(UniTask<ActionSender>))
                            {
                                // Chuyển đổi và await để lấy kết quả
                                var task = (UniTask<ActionSender>)result;
                                ActionSender newSender = await task;

                                // Bây giờ bạn đã có kết quả trả về từ hàm SayHello


                                // Bạn có thể cập nhật lại biến arg hoặc sender gốc nếu cần
                                if (!newSender.Ok)
                                {
                                    return;
                                }
                                arg = newSender;
                            }
                            // Trường hợp hàm chỉ là UniTask (không có <T>)
                            else if (result is UniTask plainTask)
                            {
                                await plainTask;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[ActionSystem] Không tìm thấy hàm '{action.functionName}' trong script '{scriptType.Name}'");
                    }
                }
            }
        }
    }
}