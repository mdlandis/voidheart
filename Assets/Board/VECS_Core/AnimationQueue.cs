using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Voidheart {
    public class AnimationQueue : SerializedSingleton<AnimationQueue> {
        private static Mutex mut = new Mutex();
        [ShowInInspector]
        public Deque<VEntity> animationEventsQueue = new Deque<VEntity>();
        public VEntityComponentSystemManager game;

        private VEntity currentAnimationEntity = null;

        int awaited = 0;

        public bool updateManually = false;

        void Update() {
            if (!updateManually) {
                while (Step()) { }
            }
        }

        [Button]
        public bool Step() {
            if (!animationEventsQueue.IsEmpty && awaited == 0) {
                return Dequeue();
            }

            return false;
        }

        public bool Dequeue() {
            bool isImmediate = true;

            currentAnimationEntity = animationEventsQueue.RemoveFront();
            foreach (VAnimationSystem aniSys in game.GetAllAnimationSystems()) {
                if (aniSys.ShouldOperate(currentAnimationEntity) && !aniSys.IsImmediate()) {
                    awaited++;
                    isImmediate = false;
                }
            }

            foreach (VAnimationSystem aniSys in game.GetAllAnimationSystems()) {
                if (aniSys.ShouldOperate(currentAnimationEntity)) {
                    if (aniSys.IsImmediate()) {
                        aniSys.DoImmediateAnimation(currentAnimationEntity);
                    } else {
                        StartCoroutine(aniSys.StartAnimation(currentAnimationEntity, YieldAnimation));
                    }
                }
            }

            return isImmediate;
        }

        public void YieldAnimation() {
            awaited--;

            if (awaited == 0 && currentAnimationEntity != null) {
                game.MarkRemovalEntity(currentAnimationEntity);
                currentAnimationEntity = null;
            }
        }

        public void Add(VEntity e) {
            animationEventsQueue.AddBack(e);
        }
    }
}