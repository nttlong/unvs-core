using System;
using System.Globalization;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using unvs.ext;
using unvs.game2d.scenes;

namespace game2d.ext
{
    public static class AppCinemaExt
    {
        public static void EditorSave(this UnityEngine.Object obj)
        {
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }
        public static void EditorSetDirty(this UnityEngine.Object obj)
        {
            EditorUtility.SetDirty(obj);
            
        }
        public static void GenerateCinema2D(this AppCinema cinema)
        {
            cinema.cam = cinema.AddChildComponentIfNotExist<Camera>("Main Camera");
            cinema.cam.tag = "MainCamera";
            cinema.cam.orthographic = true;
            cinema.cam.AddComponentIfNotExist<CinemachineBrain>();
            cinema.cam.AddComponentIfNotExist<AudioListener>();
            cinema.vcam = cinema.AddChildComponentIfNotExist<CinemachineCamera>("VCam");
            cinema.vcam.AddComponentIfNotExist<CinemachineFollow>();
            cinema.compositeCollider2D = cinema.AddChildComponentIfNotExist<CompositeCollider2D>("compositeCollider2D");
           
            cinema.compositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Polygons;
            cinema.confiner= cinema.vcam.AddComponentIfNotExist<CinemachineConfiner2D>();
            cinema.confiner.BoundingShape2D = cinema.compositeCollider2D;
        }
    }
}