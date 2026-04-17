using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace unvs.shares
{
    public class Constants
    {
        public class  Tags
        {
            public const string TRIGGER_LOAD_SCENE_LEFT = "TRIGGER-LOAD-SCENE-LEFT";
            public const string TRIGGER_LOAD_SCENE_RIGHT = "TRIGGER-LOAD-SCENE-RIGHT";
            public const string TRIGGER_SCENE_CHANGE = "TRIGGER-SCENE-CHANGE";

            public const string TRIGGER_LOAD_SCENE = "TRIGGER-LOAD-SCENE";

            public const string PLAYER_CAM_WATCHER = "PLAYER-CAM-WATCHER";

            public const string INTERACT_SCANER = "INTERACT-SCANER";

            public const string SOCKET = "SOCKET";

            public const string PLAYER_FOOTER = "PLAYER-FOOTER";

            public const string ACTOR = "ACTOR";
        }

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

            public const string HOLD_ITEM = "Hold-Item";
            public const string LIMIT_WALL = "limit_wall";

            public const string SURFACE = "surface";

            public const string ENEMIES = "enemies";

            public const string TOP_UI = "TOP-UI";

            public const string TRIGGER_LOAD_SCENE = "TRIGGER-LOAD-SCENE";

            public const string TRIGGER_SCENE_CHANGE = "TRIGGER-SCENE-CHANGE";

            public const string INTERACT_SCANER = "INTERACT-SCANER";

            public const string GROUND_FLOOR = "GROUND-FLOOR";

            public const string WORLD_GROUND = "WORLD-GROUND";

            public const string PLAYER_FOOTER = "PLAYER-FOOTER";
        }
        public class Scenes
        {

            //public const string CORE = "CoreScene";
            public const string MAIN = "MainMenu";

            //public const string SINGLE = "Single";
            public const string MAIN_SCENE = "WorldScene";
            public const string TEST_SCENE = "TestScene";
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
            public const string INVENTORY_PANEL_INTERACT = "INVENTORY-PANEL-INTERACT";
            public const string INVENTORY_PANEL_BAGGER = "INVENTORY-PANEL-BAGGER";

            public const string UI_SYSTEM_DRAG_DROP = "UI-SYSTEM-DRAG_DROP";

            public const string TOP_CANVAS = "TOP-CANVAS";
            public const string VIRTUAL_CURSOR = "VIRTUAL-CURSOR";

            public const string COMMON_AUDIO_SOURCE = "COMMON-AUDIO-SOURCE";

            public const string GLOBAL_WORLD_BOUND_SINGLE_POLYGON = "GLOBAL-WORLD-BOUND-SINGLE-POLYGON";

            public const string DEFAULT_CAM_WATCHER = "DEFAULT-CAM-WATCHER";

            public const string SCENE_SETTINGS = "SCENE-SETTINGS";

            public const string PLAYER_DIALOGUE_CONATAINER = "PLAYER-DIALOGUE-CONATAINER";
        }
        public class Settings
        {
            public const float DEFAULT_WORLD_BOUND_SCALE = 1.0f;

            public const float REACH_TOLERRANCE = 0.4f;
        }
    }
}