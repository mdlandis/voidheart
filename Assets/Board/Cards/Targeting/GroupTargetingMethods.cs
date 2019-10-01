using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class LocalDiagonal : GroupTargetingMethod {
        public int offset = 0;
        public int length = 0;
        public bool infinite = false;
        public override List<Coord> SelectGroupCoords(Coord loc, Coord casterLoc, VEntityComponentSystemManager boardState) {
            List<Coord> selectedCoords = new List<Coord>();
            foreach (Coord direction in Coord.diagonalCoords) {
                for (int distance = offset + 1; infinite || distance <= length; distance++) {
                    Coord newCoord = casterLoc + (direction * distance);
                    if (boardState.GetSystem<TerrainCellFinderSystem>().IsValidCell(newCoord)) {
                        selectedCoords.Add(newCoord);
                    } else {
                        break;
                    }
                }
            }
            return selectedCoords;
        }
    }

    [System.Serializable]
    public class LocalCardinal : GroupTargetingMethod {
        public int offset = 0;
        public int length = 0;
        public bool infinite = false;

        public override List<Coord> SelectGroupCoords(Coord loc, Coord casterLoc, VEntityComponentSystemManager boardState) {
            List<Coord> selectedCoords = new List<Coord>();
            foreach (Coord direction in Coord.cardinalCoords) {
                for (int distance = offset + 1; infinite || distance <= length; distance++) {
                    Coord newCoord = casterLoc + (direction * distance);
                    if (boardState.GetSystem<TerrainCellFinderSystem>().IsValidCell(newCoord)) {
                        selectedCoords.Add(newCoord);
                    } else {
                        break;
                    }
                }
            }
            return selectedCoords;
        }
    }

    [System.Serializable]
    public class LineArea : GroupTargetingMethod {
        public override List<Coord> SelectGroupCoords(Coord loc, Coord casterLoc, VEntityComponentSystemManager boardState) {
            List<Coord> selectedCoords = new List<Coord>();
            Coord dir = Coord.GetDirection(casterLoc, loc);
            Coord current = casterLoc + dir;
            while (boardState.GetSystem<TerrainCellFinderSystem>().GetCellAtCoord(current) != null) {
                selectedCoords.Add(current);
                current += dir;
            }
            return selectedCoords;
        }
    }

    [System.Serializable]
    public class SingleCell : GroupTargetingMethod {

        public override List<Coord> SelectGroupCoords(Coord loc, Coord casterLoc, VEntityComponentSystemManager boardState) {
            List<Coord> selectedCoords = new List<Coord> {
                loc
            };
            return selectedCoords;
        }
    }

    [System.Serializable]
    public class FirstAllegianceInLine : GroupTargetingMethod {
        public Team team;

        public override List<Coord> SelectGroupCoords(Coord loc, Coord casterLoc, VEntityComponentSystemManager boardState) {
            List<Coord> selectedCoords = new List<Coord>();
            Coord dir = Coord.GetDirection(casterLoc, loc);
            Coord current = casterLoc;
            while (boardState.GetSystem<TerrainCellFinderSystem>().IsValidCell(current)) {
                VEntity unit = boardState.GetSystem<UnitFinderSystem>().GetUnitAtCoord(current);
                if (unit != null) { //&&unit.team == team
                    break;
                }
                current += dir;
            }
            if (current != casterLoc) {
                selectedCoords.Add(current);
            }
            return selectedCoords;
        }
    }

    [System.Serializable]
    public class CardinalAreaTargeter : GroupTargetingMethod {
        public int length;

        public override List<Coord> SelectGroupCoords(Coord loc, Coord casterLoc, VEntityComponentSystemManager boardState) {
            List<Coord> selectedCoords = new List<Coord>();
            for (int i = 1; i <= length; i++) {
                foreach (Coord dir in Coord.cardinalCoords) {
                    selectedCoords.Add(loc + dir * length);
                }
            }
            return selectedCoords;
        }
    }
}