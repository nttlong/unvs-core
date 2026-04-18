#if UNITY_EDITOR



using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using unvs.actions;
using unvs.actor.skills;


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