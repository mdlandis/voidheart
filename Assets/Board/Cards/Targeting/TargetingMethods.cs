using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class SelfTargeter : TargetingMethod {

        public override List<Coord> SelectCoords(Coord loc, VEntityComponentSystemManager boardState) {
            List<Coord> selectedCoords = new List<Coord> {
                loc
            };
            return selectedCoords;
        }
    }

    [System.Serializable]
    public class CardinalTargeter : TargetingMethod {

        public int offset = 0;
        public int length = 0;
        public bool infinite = false;

        public override List<Coord> SelectCoords(Coord loc, VEntityComponentSystemManager boardState) {
            List<Coord> selectedCoords = new List<Coord>();
            TerrainCellFinderSystem terrainCellFinderSystem = boardState.GetSystem<TerrainCellFinderSystem>();

            foreach (Coord direction in Coord.cardinalCoords) {
                for (int distance = offset + 1; infinite || distance <= offset + length; distance++) {
                    Coord newCoord = loc + (direction * distance);
                    if (terrainCellFinderSystem.IsValidCell(newCoord)) {
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
    public class DiagonalTargeter : TargetingMethod {

        public int offset = 0;
        public int length = 0;
        public bool infinite = false;

        public override List<Coord> SelectCoords(Coord loc, VEntityComponentSystemManager boardState) {
            List<Coord> selectedCoords = new List<Coord>();
            TerrainCellFinderSystem terrainCellFinderSystem = boardState.GetSystem<TerrainCellFinderSystem>();
            foreach (Coord direction in Coord.diagonalCoords) {
                for (int distance = offset + 1; infinite || distance <= length; distance++) {
                    Coord newCoord = loc + (direction * distance);
                    if (terrainCellFinderSystem.IsValidCell(newCoord)) {
                        selectedCoords.Add(newCoord);
                    } else {
                        break;
                    }
                }
            }
            return selectedCoords;
        }
    }
}