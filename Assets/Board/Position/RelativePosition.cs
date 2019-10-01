using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class RelativePositionComponent : VComponent {
        public string relativeEntityId;
        public Coord delta;
    }

    public class RelativePositionSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return false;
        }

        public Coord Resolve(RelativePositionComponent cRelativePosition) {
            VEntity eOrigin = ecsManager.GetVEntityById(cRelativePosition.relativeEntityId);
            return eOrigin.GetVComponent<PositionComponent>().position + cRelativePosition.delta;
        }

    }
}