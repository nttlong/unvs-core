using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using unvs.actor.skills;
using static UnityEngine.GraphicsBuffer;

namespace unvs.editor.actorskill
{
    [CustomEditor(typeof(UnvsActorSkills))]
    public class UnvsActorBaseSkillEditor : Editor
    {
        private Type[] _implementations;
        private int _selectedIndex;

        private void OnEnable()
        {
            // Tìm tất cả các class kế thừa từ ActorBaseSkill
            _implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ActorBaseSkill).IsAssignableFrom(p) && !p.IsAbstract)
                .ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // Vẽ mảng hiện tại

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add New Skill", EditorStyles.boldLabel);

            // Hiển thị danh sách các Skill thực tế (như Skill Đấm, Skill Đá...)
            string[] typeNames = _implementations.Select(t => t.Name).ToArray();
            _selectedIndex = EditorGUILayout.Popup("Skill Type", _selectedIndex, typeNames);

            if (GUILayout.Button("Add Skill Instance"))
            {
                var targetScript = (UnvsActorSkills)target;
                Type selectedType = _implementations[_selectedIndex];

                // Tạo instance mới
                ActorBaseSkill newSkill = (ActorBaseSkill)Activator.CreateInstance(selectedType);
                newSkill.name = "New " + selectedType.Name;

                // Thêm vào mảng
                Array.Resize(ref targetScript.skills, (targetScript.skills?.Length ?? 0) + 1);
                targetScript.skills[targetScript.skills.Length - 1] = newSkill;

                EditorUtility.SetDirty(target);
            }
        }
    }
}