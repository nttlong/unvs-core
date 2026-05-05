using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
namespace unvs.editor.utils
{
    public static class TypeCacheHelper
    {
        private static readonly Dictionary<Type, Type[]> _typeCache = new Dictionary<Type, Type[]>();

        public static Type[] GetDerivedTypes(Type baseType)
        {
            if (_typeCache.TryGetValue(baseType, out var cachedTypes))
            {
                return cachedTypes;
            }

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => {
                    try {
                        return s.GetTypes();
                    } catch (ReflectionTypeLoadException e) {
                        return e.Types.Where(t => t != null);
                    } catch (Exception) {
                        return Enumerable.Empty<Type>();
                    }
                })
                .Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract && p.IsClass)
                .ToArray();

            _typeCache[baseType] = types;
            return types;
        }

        public static void ClearCache()
        {
            _typeCache.Clear();
        }
    }
    

public class ForceZWriteTool
    {
        [MenuItem("UNVS/Force Enable ZWrite for all Materials")]
        public static void ForceEnableZWrite()
        {
            unvs.editor.utils.UnvsEditorUtils.ForceEnableZWriteAllMaterial();
        }
    }
}
