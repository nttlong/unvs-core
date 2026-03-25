using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unvs.interfaces.sys;
using unvs.shares;

namespace unvs.sys
{
    public class RealtimeStats : MonoBehaviour, IRealtimeStats
    {
        Dictionary<string,object> trackInfo=new Dictionary<string, object>();
        float deltaTime = 0.0f;
        private void Awake()
        {
            GlobalApplication.RealtimeStatsInstance = this as IRealtimeStats;
            QualitySettings.vSyncCount = 0; // Tắt VSync
            Application.targetFrameRate =120; // Khoá ở 60 FPS (hoặc thả rông bằng -1)
        }
        void Update()
        {
            // Tính toán dựa trên thời gian thực tế của phần cứng, không bị ảnh hưởng bởi Editor
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(20, 20, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 40; // Chỉnh cỡ chữ to dễ nhìn
            style.normal.textColor = Color.green; // Màu xanh cho dễ phân biệt

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;

            string text = string.Format("Real Logic: {0:0.0} ms ({1:0.0} FPS)", msec, fps);
            foreach(var p in trackInfo)
            {
                text += $"\n{p.Key}={p.Value}";
            }
            GUI.Label(rect, text, style);
        }

        

        public void SaveTrack(string name, object value)
        {
            trackInfo[name]= value;
        }
    }
}