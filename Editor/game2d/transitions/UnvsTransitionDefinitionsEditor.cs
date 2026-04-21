#if UNITY_EDITOR

using UnityEditor;
using unvs.game2d.transitions;

[CustomEditor(typeof(UnvsTransitionDefinitions))]
public class UnvsTransitionDefinitionsEditor : Editor
{
    private EditorDataConfig localdata;

    private void OnEnable()
    {
        localdata = this.OnEnableByTypes("actions", typeof(UnvsTransitionBase));
    }

    public override void OnInspectorGUI()
    {
        EditorExtension.OnInspectorGUI(this, localdata);
    }
}

#endif
