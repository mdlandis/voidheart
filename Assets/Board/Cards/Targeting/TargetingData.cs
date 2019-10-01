using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {

    [Serializable]
    public abstract class TargetingMethod {
        public abstract List<Coord> SelectCoords(Coord loc, VEntityComponentSystemManager boardState);
    }

    [Serializable]
    public abstract class ValidTargetingMethod {
        public abstract bool ValidateCoord(Coord loc, VEntityComponentSystemManager boardState);
        public CardEffectType effectType;
    }

    [Serializable]
    public abstract class GroupTargetingMethod {
        public abstract List<Coord> SelectGroupCoords(Coord loc, Coord casterLoc, VEntityComponentSystemManager boardState);
        public CardEffectType effectType;
    }
}