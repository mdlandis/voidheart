using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class UnitFinderSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return false;
        }

        public VEntity GetUnitAtCoord(Coord c) {
            IEnumerable<VEntity> entities = ecsManager.GetSystem<PositionTrackSystem>().GetAtCoord(c);
            foreach (VEntity entity in entities) {
                if (entity.GetVComponent<IsUnitComponent>() != null) {
                    return entity;
                }
            }

            return null;
        }

        public VEntity GetPlayerUnitByClass(CardType type) {
            UnitsList unitsList = ecsManager.GetVSingletonComponent<UnitsList>();

            string unitId = unitsList.unitIds.Find((id) => {
                CharacterClassComponent characterClassComponent = ecsManager.GetVComponent<CharacterClassComponent>(id);
                return characterClassComponent != null && characterClassComponent.cardType == type;
            });

            return ecsManager.GetVEntityById(unitId);
        }

        public IEnumerable<VEntity> GetAllPlayerUnits() {
            UnitsList unitsList = ecsManager.GetVSingletonComponent<UnitsList>();
            List<VEntity> list = new List<VEntity>();

            foreach (string unitId in unitsList.unitIds) {
                if (ecsManager.GetVComponent<CharacterClassComponent>(unitId) != null) {
                    list.Add(ecsManager.GetVEntityById(unitId));
                }
            }

            return list;
        }

        public IEnumerable<VEntity> GetAllEnemyUnits() {
            UnitsList unitsList = ecsManager.GetVSingletonComponent<UnitsList>();
            List<VEntity> list = new List<VEntity>();

            foreach (string unitId in unitsList.unitIds) {
                if (ecsManager.GetVComponent<TeamComponent>(unitId).team == Team.ENEMY) {
                    list.Add(ecsManager.GetVEntityById(unitId));
                }
            }

            return list;
        }
    }
}