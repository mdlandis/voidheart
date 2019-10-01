using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Voidheart {

    [Serializable]
    public abstract class VAnimationSystem {
        protected VEntityComponentSystemManager ecsManager = null;

        public virtual void Init(VEntityComponentSystemManager newEcsManager) {
            this.ecsManager = newEcsManager;
        }

        public abstract bool ShouldOperate(VEntity entity);

        public abstract bool IsImmediate();

        public virtual IEnumerator StartAnimation(VEntity entity, Action yieldAnimation) {
            yield return null;
        }

        public virtual void DoImmediateAnimation(VEntity entity) { }

        public virtual void DestroyAnimation() { }
    }

}