using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class TerrainCellFinderSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return false;
        }

        public IList<VEntity> GetCells(IEnumerable<Coord> coords) {
            List<VEntity> cellEntities = new List<VEntity>();
            foreach (Coord c in coords) {
                VEntity e = GetCellAtCoord(c);
                if (e != null) {
                    cellEntities.Add(e);
                }
            }

            return cellEntities;
        }

        public VEntity GetCellAtCoord(Coord c) {
            PositionTrackSystem positionTracker = ecsManager.GetSystem<PositionTrackSystem>();
            IEnumerable<VEntity> entitiesAtCoord = positionTracker.GetAtCoord(c);

            foreach (VEntity v in entitiesAtCoord) {
                if (v.HasVComponent<TerrainCellComponent>()) {
                    return v;
                }
            }

            return null;
        }

        public bool IsValidCell(Coord c) {
            VEntity cell = GetCellAtCoord(c);
            return (cell != null && cell.GetVComponent<TerrainCellComponent>().cellType != VCellType.VOID);
        }
    }
}