using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class PathingSystem : VSystem {
        private static VDijkstraPathfinding _pathfinder = new VDijkstraPathfinding();

        public override bool ShouldOperate(VEntity entity) {
            // TODO: Should I just return false?
            return false;
        }

        public Dictionary<Coord, List<Coord>> GetAvailableDestinations(Coord enemy, int movementPoint)
        {
            TerrainCellFinderSystem cellFinderSystem = ecsManager.GetSystem<TerrainCellFinderSystem>();
            UnitFinderSystem unitFinderSystem = ecsManager.GetSystem<UnitFinderSystem>();

            var cachedPaths = new Dictionary<Coord, List<Coord>>();
            var edges = GetGraphEdges();
            var paths = _pathfinder.findAllPaths(edges, enemy);
            foreach (var key in paths.Keys)
            {
                if (!cellFinderSystem.IsValidCell(key) || (unitFinderSystem.GetUnitAtCoord(key) != null)) continue;
                var path = paths[key];

                var pathCost = path.Count;
                if (pathCost <= movementPoint)
                {
                    cachedPaths.Add(key, path);
                }
            } 
            return cachedPaths;
        }

        protected Dictionary<Coord, Dictionary<Coord, int>> GetGraphEdges() {
            CellsListComponent cellsList = ecsManager.GetVSingletonComponent<CellsListComponent>();
            List<Coord> cells = ecsManager.GetVComponentsFromList<PositionComponent>(cellsList.cellIds).Select(i => i.position).ToList();
            Dictionary<Coord, Dictionary<Coord, int>> ret = new Dictionary<Coord, Dictionary<Coord, int>>();
            TerrainCellFinderSystem cellFinderSystem = ecsManager.GetSystem<TerrainCellFinderSystem>();

            foreach (var cell in cells) {
                if (cellFinderSystem.IsValidCell(cell)) {
                    ret[cell] = new Dictionary<Coord, int>();
                    foreach (var neighbour in cell.GetSurroundingCoords().FindAll(c => cellFinderSystem.IsValidCell(c))) {
                        ret[cell][neighbour] = 1; // cost of moving to that cell
                    }
                }
            }
            return ret;
        }
    }
}
