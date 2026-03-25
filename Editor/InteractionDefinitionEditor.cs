#if UNITY_EDITOR



using UnityEditor;
using unvs.actions;


[CustomEditor(typeof(InteractionDefinition))]
public class InteractionDefinitionEditor : Editor
{
    //private ReorderableList _reorderableList;
    //private Type[] _actionTypes;
    private EditorDataConfig localdata;

    private void OnEnable()
    {
        localdata = this.OnEnableByTypes("actions", typeof(ActionBase), typeof(Conditional));
       
    }

   
    public override void OnInspectorGUI()
    {
        EditorExtension.OnInspectorGUI(this,this.localdata);
        
    }
    
    
}
#endif