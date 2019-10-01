using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [Serializable]
    public class TerrainCellComponent : VComponent {
        public int movementCost;
        public VCellType cellType;
    }

    [Serializable]
    /// <summary>
    ///  using legacy cell components for now
    /// </summary>
    public class TerrainCellDisplayComponent : VComponent {

        public VCellDisplay cellDisplay;
        public VCellType cellType;
    }

    [Serializable]
    public class CreateTerrainCellEvent : VComponent {
        public Coord coord;
        public int movementCost;
        public VCellType cellType;
    }

    [Serializable]
    public class CellsListComponent : VComponentSingleton {
        public List<string> cellIds;
    }

    /// <summary>Creates a terrain cell.</summary>
    [Serializable]
    public class TerrainCellCreationSystem : VSystem {
        [SerializeField]
        private VCellDisplay cellDisplayPrefab = null;
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CreateTerrainCellEvent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            var createEvent = VEntityComponentSystemManager.GetVComponent<CreateTerrainCellEvent>(entity);
            VEntity newCellEntity = ecsManager.CreateEntity("Cell", components : new List<VComponent> {
                new PositionComponent {
                    position = createEvent.coord
                },
                new TerrainCellComponent {
                    movementCost = createEvent.movementCost, cellType = createEvent.cellType
                },
                new TerrainCellDisplayComponent {
                    cellDisplay = VCellDisplay.Instantiate(cellDisplayPrefab),
                        cellType = createEvent.cellType
                }
            });
            VEntityComponentSystemManager.GetVComponent<TerrainCellDisplayComponent>(newCellEntity).cellDisplay.gameObject.SetActive(false);
            ecsManager.GetVSingletonComponent<CellsListComponent>().cellIds.Add(newCellEntity.id);
            ecsManager.GetSystem<PositionTrackSystem>().Track(newCellEntity);

            VEntity cellCreationAnimationEntity = ecsManager.CreateEntity(prefix: "CellCreationAnimation", component : new TerrainCellDisplayAppearEvent {
                coord = createEvent.coord,
                    terrainCellEntityId = newCellEntity.id
            });
            ecsManager.QueueAnimationEvent(cellCreationAnimationEntity);
        }
    }
}