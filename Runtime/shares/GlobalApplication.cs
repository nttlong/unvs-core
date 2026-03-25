using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using unvs.interfaces;
using unvs.interfaces.sys;
using unvs.manager;
namespace unvs.shares
{
    public class GlobalApplication
    {

        public static Canvas UICanvas { get; set; }

        public static IUISpeakerController SpeakerController { get; set; }
        public static IFadeScreen FadeScreenController { get; set; }
        public static IHubController HubController { get; set; }
        public static IInventoryController InventoryController { get; set; }
        //public static IWorldMonitor WorldMonitorManager { get; set; }
        public static IWorldTracker WorldTrackerObject { get; set; }
        public static ICamCenterCamTracking CamTracking { get; set; }
        public static ISingleScene SingleScene { get; internal set; }
        public static IDiscoveryDialog DiscoveryDialogInstance { get; set; }
        public static LightManagerObject LightManagerObjectInstance { get; internal set; }
        public static IRealtimeStats RealtimeStatsInstance { get; internal set; }
        public static ISceneLoader SceneLoaderManagerInstance { get; internal set; }
        public static IUIHub UIHub { get; internal set; }
        public static IPauseMenu UIPauseMenu { get; internal set; }
        public static IDiscoveryDialog UIDiscoveryDialog { get; internal set; }
    }
}