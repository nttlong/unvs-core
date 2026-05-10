using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using unvs.ext;
using unvs.game2d.objects.editor;
using unvs.shares;
using unvs.types;

namespace unvs.components
{

    public abstract class UnvsBaseComponent : MonoBehaviour
    {
        public event Action OnDestroying;
        public virtual void InitProperties()
        {
            var fields = Commons.GetAllGenericFields(this, typeof(UnvsProperty<>));
            foreach (var field in fields)
            {

                var propertyInstance = field.GetValue(this);
                var ownerField = Commons.GetFlatternField(field, "Owner");
                var component = this.GetComponent(field.FieldType.GenericTypeArguments[0]);


                if (ownerField != null && component!=null && propertyInstance!=null)
                {
                    try
                    {
                        ownerField.SetValue(propertyInstance, component);
                    }
                    catch (Exception)
                    {

                       
                    }


                }
            }
        }
        public virtual void OnDestroy()
        {
            if (OnDestroying != null)
            {
                var invocationList = OnDestroying.GetInvocationList();
                foreach (var action in invocationList)
                {
                    try
                    {
                        ((Action)action).Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error when clean component: {ex.Message}");
                    }
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
        [UnvsButton]
        public void AddSoringGroup()
        {
            foreach (var item in GetComponentsInChildren<SpriteRenderer>(true))
            {
                item.AddComponentIfNotExist<SortingGroup>();
            }


        }
        [UnvsButton]
        public void RemoveSortingGroup()
        {
            foreach (var item in GetComponentsInChildren<SpriteRenderer>(true))
            {
                var b=item.GetComponent<SortingGroup>();
                if (b != null)
                {
                    DestroyImmediate(b);
                }
                //item.AddComponentIfNotExist<SortingGroup>();
            }


        }
#endif


    }
}