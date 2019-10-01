using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public class CoordWorldTranslationSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return false;
        }
    }
}