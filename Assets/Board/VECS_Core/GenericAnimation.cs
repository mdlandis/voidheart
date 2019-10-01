using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public class GenericImmediateAnimationEvent : VComponent {
        public Action<VEntityComponentSystemManager> a;
    }

    public class GenericImmediateAnimationSystem : VAnimationSystem {
        public override bool IsImmediate() {
            return true;
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<GenericImmediateAnimationEvent>(entity);
        }

        public override void DoImmediateAnimation(VEntity entity) {
            GenericImmediateAnimationEvent genericAnim = entity.GetVComponent<GenericImmediateAnimationEvent>();
            genericAnim.a(ecsManager);
        }
    }

    public class GenericBlockingAnimationEvent : VComponent {
        public Action<VEntityComponentSystemManager> a;
        public float duration;
    }

    public class GenericAnimationSystem : VAnimationSystem {
        public override bool IsImmediate() {
            return false;
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<GenericBlockingAnimationEvent>(entity);
        }

        public override IEnumerator StartAnimation(VEntity entity, Action yieldAnimation) {
            GenericBlockingAnimationEvent genericAnim = entity.GetVComponent<GenericBlockingAnimationEvent>();
            genericAnim.a(ecsManager);
            yield return new WaitForSeconds(genericAnim.duration);
            yieldAnimation();
        }
    }
}