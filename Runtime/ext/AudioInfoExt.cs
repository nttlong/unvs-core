using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using unvs.shares;

namespace unvs.ext {
    public static class AudioInfoExt 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static async UniTask PlayBetterAudioClipAsync(this AudioInfo source, params AudioInfo[] others)
        {
            if (!source.IsEmpty())
            {
                GlobalApplication.CommonAudioSource.PlayOneShot(source.Clip, source.volume);
                await UniTask.Delay(TimeSpan.FromSeconds(source.Clip.length), delayTiming: PlayerLoopTiming.Update);
               
            } else
            {
                var first=others.FirstOrDefault(p=>!p.IsEmpty());
                if (!first.IsEmpty())
                {
                    GlobalApplication.CommonAudioSource.PlayOneShot(first.Clip, first.volume);
                    await UniTask.Delay(TimeSpan.FromSeconds(first.Clip.length), delayTiming: PlayerLoopTiming.Update);
                }
            }

        }
        public static void PlayBetterAudioClip(this AudioInfo source, params AudioInfo[] others)
        {
            if (!source.IsEmpty())
            {
                GlobalApplication.CommonAudioSource.PlayOneShot(source.Clip, source.volume);

            }
            else
            {
                var first = others.FirstOrDefault(p => !p.IsEmpty());
                if (!first.IsEmpty())
                {
                    GlobalApplication.CommonAudioSource.PlayOneShot(first.Clip, first.volume);
                }
            }
        }
    }
}