//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using Unity.VisualScripting;
//using UnityEngine;
//using unvs.actions;
//

//namespace unvs.ext
//{
//    public static class InteractionDefinitionExt
//    {
//        public async static UniTask<bool> ExecAsync(this InteractionDefinition data, IInteractableObject source, MonoBehaviour another, CancellationTokenSource ct)
//        {
//            // 1. Kiểm tra sớm để tránh chạy vòng lặp vô ích
//            if (ct.IsCancellationRequested) return false;
//            ct.Token.ThrowIfCancellationRequested();

//            // 2. Kiểm tra dữ liệu đầu vào để tránh NullReferenceException
//            if (data == null || data.actions == null) return false;
//            var sender = new ActionBaseSender
//            {
//                Cts = ct,
//                Source = source as MonoBehaviour,
//                Target = another
//            };
//            try
//            {
//                var srcMono = source as MonoBehaviour;
//                var anotherMono = another;
//                foreach (var action in data.actions)
//                {
//                    if (action == null) continue;
//                    //Debug.Log($"InteractionDefinition.ExecAsync,{action.GetType().Name},source={srcMono.name},{srcMono.GetType().Name},another={anotherMono.name},{anotherMono.GetType().Name}");
//                    // Kiểm tra Token trước mỗi Action trong chuỗi
//                    if (ct.IsCancellationRequested) return false;
//                    var success = false;
//                    // Nếu một action thất bại (return false), dừng toàn bộ chuỗi
//                    await action.ExecuteAsync(sender);
//                    if (sender.IsCancel) return false;

//                }
//            }
//            catch (OperationCanceledException)
//            {
//                // Trả về false thay vì ném lỗi để các hệ thống cha xử lý nhẹ nhàng hơn
//                return false;
//            }


//            return true;
//        }

//        public async static UniTask<bool> ObjectsExecAsync(this InteractionDefinition data, object source, object another, CancellationTokenSource ct)
//        {
//            // 1. Kiểm tra sớm để tránh chạy vòng lặp vô ích
//            if (ct.IsCancellationRequested) return false;
//            ct.Token.ThrowIfCancellationRequested();

//            // 2. Kiểm tra dữ liệu đầu vào để tránh NullReferenceException
//            if (data == null || data.actions == null) return false;
//            var sender = new ActionBaseSender
//            {
//                Cts = ct,
//                Source = source as MonoBehaviour,
//                Target = another as MonoBehaviour
//            };
//            try
//            {
//                var srcMono = source as MonoBehaviour;
//                var anotherMono = another;
//                foreach (var action in data.actions)
//                {
//                    if (action == null) continue;
//                    //Debug.Log($"InteractionDefinition.ExecAsync,{action.GetType().Name},source={srcMono.name},{srcMono.GetType().Name},another={anotherMono.name},{anotherMono.GetType().Name}");
//                    // Kiểm tra Token trước mỗi Action trong chuỗi
//                    if (ct.IsCancellationRequested) return false;
//                    var success = false;
//                    // Nếu một action thất bại (return false), dừng toàn bộ chuỗi
//                    await action.ExecuteAsync(sender);
//                    if (sender.IsCancel) return false;

//                }
//            }
//            catch (OperationCanceledException)
//            {
//                // Trả về false thay vì ném lỗi để các hệ thống cha xử lý nhẹ nhàng hơn
//                return false;
//            }


//            return true;
//        }
//        public async static UniTask<bool> MonoBehaviourExecAsync(this InteractionDefinition data, MonoBehaviour source, MonoBehaviour another, CancellationToken ct)
//        {
//            if (ct.IsCancellationRequested) return false;

//            // 1. Tạo một "Cầu nối" (Linked Source)
//            // Nguồn này sẽ bị Cancel NẾU 'ct' bị Cancel HOẶC NẾU ta gọi 'internalCts.Cancel()'
//            using var internalCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

//            var sender = new ActionBaseSender
//            {
//                Source = source,
//                Target = another,
//                IsCancel = false,
//                Cts = internalCts // Gán Linked Source vào đây
//            };

//            try
//            {
//                foreach (var action in data.actions)
//                {
//                    if (action == null) continue;

//                    // 2. Kiểm tra xem có lệnh hủy nào (từ bên ngoài hoặc từ action trước đó) không
//                    if (internalCts.IsCancellationRequested || sender.IsCancel)
//                    {
//                        return false;
//                    }

//                    // 3. Thực thi Action
//                    // Lưu ý: Bên trong Action, bạn phải dùng sender.Cts.Token cho các hàm Async (như MoveTo)
//                    await action.ExecuteAsync(sender);

//                    // 4. Nếu Action vừa rồi gọi sender.Cancel(), ngắt chuỗi ngay
//                    if (sender.IsCancel) return false;
//                }
//            }
//            catch (OperationCanceledException)
//            {
//                return false;
//            }
//            finally
//            {
//                // Luôn dispose để tránh rò rỉ bộ nhớ
//                // internalCts.Dispose(); // Đã có 'using' ở trên lo việc này
//            }

//            return true;
//        }

//    }
//}