namespace unvs.data
{
 
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.U2D;
    using unvs.game2d.actors;

    [CreateAssetMenu(fileName = "Player", menuName = "Unvs/Data/Player")]
    public partial class UnvsPlayerData : UnvsScriptObject
    {

        public UnvsActor Owner;
       


    }
#if UNITY_EDITOR
    public partial class UnvsPlayerData : UnvsScriptObject
    {
        
    } 
#endif
}