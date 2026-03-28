using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace unvs.shares
{
    public class Constants
    {
        public class CinemachineDefaut
        {
            public const float OrthographicSize = 20;
        }
        public class Layers
        {
            public const string INTERACT_OBJECT = "Interact-Object";

            public const string NPC = "NPC";
            public const string ACTOR = "Actor";

            public const string TERRANT = "Terrant";
            public const string WORLD_BOUND = "World-Bound";

            public const string UI = "UI";

            public const string INVENTORY = "inventory";

            public const string HOLD_ITEM = "hold_item";
            public const string LIMIT_WALL = "limit_wall";

            public const string SURFACE = "surface";

            public const string ENEMIES = "enemies";

           
        }
        public class Scenes
        {

            //public const string CORE = "CoreScene";
            public const string MAIN = "MainMenu";

            //public const string SINGLE = "Single";
            public const string MAIN_SCENE = "WorldScene";
        }
        public class ObjectsConst
        {
            public const string GLOBAL_WORLD_BOUND = "GlobalWorldBound";

            public const string GLOBAL_LIGHT = "Global-Light";

            public const string SCENE_GLOBAL_LIGHT = "GlobalLight";

            public const string SCENE_TRACKER = "SceneTracker";

            public const string BACKUP_INTERIOR_SCENE = "BACKUP-INTERIOR-SCENE";

            public const string GROUND_PHYSICAL = "GroundPhysics";

            public const string WORLD_BOUND = "MyWorldBound";

            public const string WORLD_JOIN_INFO = "WorldJoinInfo";

            public const string SCENE_LOADER = "SCENE-LOADER";

            public const string CHUNK_SCENES = "CHUNK-SCENES";

            public const string CHUNK_SCENES_TEMP_LOADER = "CHUNK-SCENES-TEMP-LOADER";

            public const string CHUNK_SCENES_TEMP_DELETE = "CHUNK-SCENES-TEMP-DELETE";
            public const string INTERIOR_SCENE = "INTERIOR-SCENE";

            public const string ACTOR_PLACE_HOLDER = "ACTOR-PLACE-HOLDER";
            public const string BACKUP_GLOBAL_LIGHT = "BACKUP-GLOBAL-LIGHT";
            public const string LEFT_WALL_NAME = "left-wall";
            public const string RIGHT_WALL_NAME = "right-wall";
            public const string TRGIGER_LEFT_NAME = "trigger-left";
            public const string TRGIGER_RIGHT_NAME = "trigger-right";
            public const string START_POINT = "StartPoint";

            public const string WORLD_INFO = "WorldJoinInfo";

            public const string MY_WORLD_BOUND  = "MyWorldBound";

            public const string CAM_WATCHER = "cam-watcher";
            public const string CAM_TRACKER = "CAM-TRACKER";
            public const string CAM_TRACKER_BODY = "CAM-TRACKER-BODY";
            public const string HUB_CANVAS = "HUB-CANVAS";
            public const string HUB_PANEL = "HUB-PANEL";

            public const string INVENTORY_CANVAS = "INVENTORY-CANVAS";

            public const string INVENTORY_PANEL = "INVENTORY-PANEL";
        }
        public class Settings
        {
            public const float DEFAULT_WORLD_BOUND_SCALE = 1.1f;

            public const float REACH_TOLERRANCE = 0.4f;
        }
    }
}