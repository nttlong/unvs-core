using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using unvs.ext;
using unvs.ext.physical2d;
using unvs.shares;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

namespace unvs.game2d.scenes.actors
{
    [Serializable]
    public struct MotionAudio
    {
        public string name;
        [SerializeField]
        public AudioInfo audio;
        public int LayerIndex;
        public string LayerName;
        public string blendName;
        public float value;
    }
    [RequireComponent(typeof(AudioSource))]
    public class UnsvPalyerAnimatorEvent : MonoBehaviour
    {
        UnvsAnimStates _sates;
        private Animator _anim;
        private AudioSource _audoiSource;
        private List<MotionAudio> _clips;
        private Collider2D _coll;
        private int _layerMask;

        private void Awake()
        {
            if (Application.isPlaying)
            {
                _polyColl = GetComponent<PolygonCollider2D>();
                _spriteRenderer = GetComponent<SpriteRenderer>();
                _sates = this.GetComponentInParent<UnvsAnimStates>();
                _anim = GetComponent<Animator>();
                _audoiSource = GetComponent<AudioSource>();
                _clips = _sates.motionAudio.Where(p => p.audio.Clip != null).ToList();
                _coll = this.GetComponentInParent<Collider2D>();
                _layerMask = LayerMask.GetMask(Constants.Layers.TERRANT, Constants.Layers.GROUND_FLOOR);
            }
        }
        public void OnKeyFrame()
        {
            if (_anim == null) return;
            RaycastHit2D[] hits = new RaycastHit2D[5];   // mảng kết quả (dùng 1-10 là đủ)


           var audibleObject = _coll.RayCastDownHit< UnvsAudible>( _layerMask);
            if(audibleObject != null)
            {
                AudioInfo audioInfo = audibleObject.audioInfo;
                if (audioInfo.Clip != null)
                {
                    this._audoiSource.PlayOneShot(audioInfo.Clip, audioInfo.volume);
                    return;
                }
            }
            
            playActorSound();
        }
        private PolygonCollider2D _polyColl;
        private SpriteRenderer _spriteRenderer;

       
        private void playActorSound()
        {
            foreach (var item in _clips)
            {
                if (_anim.GetLayerWeight(item.LayerIndex) > 0)
                {
                    var stateInfo = _anim.GetCurrentAnimatorStateInfo(item.LayerIndex);

                    if (stateInfo.IsName(item.blendName))
                    {
                        if (stateInfo.speed == item.value)
                        {
                            this._audoiSource.PlayOneShot(item.audio.Clip, item.audio.volume);

                        }

                    }
                    else
                    {
                        if (stateInfo.IsName(item.name))
                        {
                            this._audoiSource.PlayOneShot(item.audio.Clip, item.audio.volume);
                        }
                    }
                }


            }
        }
       
    }
}
