using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using unvs.shares;
using unvs.types;

namespace unvs.components
{

    public abstract class UnvsBaseComponent : MonoBehaviour
    {

        public virtual void InitProperties()
        {
            var fields = Commons.GetAllGenericFields(this, typeof(UnvsProperty<>));
            foreach (var field in fields)
            {

                var propertyInstance = field.GetValue(this);
                var ownerField = Commons.GetFlatternField(field, "Owner");
                var component = this.GetComponent(field.FieldType.GenericTypeArguments[0]);


                if (ownerField != null && component!=null)
                {
                    ownerField.SetValue(propertyInstance, component);


                }
            }
        }

#if UNITY_EDITOR
        public Action<string> OnEditorError;
       

        public void RaiseEditorError(string error)
        {
            OnEditorError?.Invoke(error);
        }
        public virtual void OnDrawGizmos()
        {
            InitProperties();


        }
        public virtual void OnValidate()
        {
            InitProperties();
        }
#endif


    }
}