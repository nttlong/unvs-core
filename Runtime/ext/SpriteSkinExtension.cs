
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;

namespace unvs.ext
{
    public static class SpriteSkinExtension
    {
        /// <summary>
        /// Tạo IK cho SpriteSkin
        /// Bằng cách goi ham BoneTravel để bảo đảm duyệt đúng thứ tự
        /// Chỉ tạo FabrikSolver2D nếu xương hiện tại cách xương root ít nhất 1 xương
        /// Ví dũ: Root->A (không tạo), Rooot-A-B (Tạo)
        /// </summary>
        /// <param name="sprites"></param>
        /// <param name="root"></param>
        public static void CreateIK(this SpriteSkin[] sprites,Transform root)
        {
            if (Application.isPlaying) return;
            var bones = sprites.SelectMany(p => p.boneTransforms);
            
            Transform rootBone = TransformExt.GetRoot(bones);

            var ik = rootBone.AddComponentIfNotExist<IKManager2D>();
            rootBone.gameObject.SetActive(false);

            
            var leafs = bones.GetAllLeafBones();
            //foreach (var bone in leafs)
            //{
            //    Debug.Log($"ExtractAllSpriteSkins.GetAllLeafBones {bone.name}");
            //}

            //var IKChainList = new Collection<IKChain2D>();
            //var FabrikSolver2DList= new Collection<FabrikSolver2D>();
            var bonesArray = bones.ToArray();
            var targets = root.CreateIfNoExist<Transform>("targets");
            var solver = root.CreateIfNoExist<Transform>("solver");
            root.gameObject.SetActive(false);
            BoneTravel(rootBone, targets, bonesArray, (parent, bone) =>
            {
                if (bone == null || bone == rootBone || bone.parent == rootBone) return parent;

                var nameOfSover = $"{bone.name}-solver";
                var nameOfTarget = $"{bone.name}";
                //if (ik.solvers.FirstOrDefault(p => p.name == nameOfTarget) != null) continue;
                if (root.GetComponentInChildrenByName<FabrikSolver2D>(nameOfSover) != null) 
                {
                    // Trả về target cũ nếu solver đã tồn tại, hoặc trả về parent nếu không tìm thấy target
                    var existingTarget = root.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == nameOfTarget);
                    return existingTarget != null ? existingTarget : parent;
                }
                
                var ret = solver.AddChildIfNotExist<FabrikSolver2D>(nameOfSover);// vi sao solver bi null
                if (ret == null)
                {
                    throw new System.Exception($"ik.AddChildIfNotExist<FabrikSolver2D>({nameOfSover}) fail");
                }
                ret.enabled = false;
                //FabrikSolver2DList.Add(ret);
                if (ik.solvers.Where(p=>p!=null).FirstOrDefault(p => p.name == nameOfSover) == null)
                {
                    ik.AddSolver(ret);
                }
                //ik.AddSolver(ret);
                
                var ch = ret.GetChain(0);
                if (ch == null)
                {
                    Debug.LogError($"{ret.name} GetChain is null");
                    return null;
                }
                ch.target = ret.AddChildIfNotExist<Transform>(nameOfTarget);
                //ch.target.transform.position = bone.transform.position;
                ch.effector = bone;
                ch.target.position=new Vector3(bone.position.x,bone.position.y,bone.position.z);
                ch.transformCount = 2;
                ch.target.SetParent(parent, true);
               // IKChainList.Add(ch);
                return ch.target;


            });
           
            rootBone.gameObject.SetActive(true);
            root.gameObject.SetActive(true);
        }
        public static void BoneTravel(Transform root,Transform parent, Transform[] bones, System.Func<Transform, Transform,Transform> OnScan)
        {
            if (root == null || bones == null) return;

            foreach (Transform child in root)
            {
                // Kiểm tra xem child có phải là xương hợp lệ không
                // Sử dụng System.Linq để kiểm tra nhanh
                bool isBone = System.Array.Exists(bones, b => b == child);

                if (isBone)
                {
                    // Thực hiện logic quét (Cha, Con)
                    var current=OnScan?.Invoke(parent, child);

                    // Chỉ tiếp tục đệ quy nếu nó là xương, tránh đi sâu vào các object linh tinh
                    BoneTravel(child, current, bones, OnScan);
                }
            }
        }
    }
}