using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class SelfValidator : ValidTargetingMethod {
        public override bool ValidateCoord(Coord loc, VEntityComponentSystemManager boardState) {
            return true;
        }
    }

    [System.Serializable]
    public class AllegianceValidator : ValidTargetingMethod {
        public Team team;

        public override bool ValidateCoord(Coord loc, VEntityComponentSystemManager boardState) {
            VEntity unit = boardState.GetSystem<UnitFinderSystem>().GetUnitAtCoord(loc);
            if (unit != null) {
                return (unit.GetVComponent<TeamComponent>().team == team);
            }

            return false;
        }
    }

    [System.Serializable]
    public class OccupancyValdiator : ValidTargetingMethod {
        public bool shouldBeOccupied;

        public override bool ValidateCoord(Coord loc, VEntityComponentSystemManager boardState) {
            return (shouldBeOccupied == (boardState.GetSystem<UnitFinderSystem>().GetUnitAtCoord(loc) != null));
        }
    }
}