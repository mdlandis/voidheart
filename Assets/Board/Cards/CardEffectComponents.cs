using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public interface CardEffectComponent {
        
        string sourceId {
            get;
            set;
        }

        Coord targetCoord {
            get;
            set;
        }

        List<Coord> groupTargetCoords
        {
            get;
            set;
        }
    }
}