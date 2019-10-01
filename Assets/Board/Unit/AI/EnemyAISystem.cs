using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class AIComponent : VComponent {

    }

    [System.Serializable]
    public class CellScoresComponent : VComponentSingleton {
        public Dictionary<Coord, int> cellScores;
    }

    [System.Serializable]
    public class EnemyTargetComponent : VComponent {

    }

    [System.Serializable]
    public class InitializeCellScores : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CellScoresComponent>(entity);
        }

        protected override void OnEnemySetupStart(VEntity entity) {
            CellScoresComponent CellScoresComponent = ecsManager.GetVSingletonComponent<CellScoresComponent>();
            CellScoresComponent.cellScores = new Dictionary<Coord, int>();
            CellsListComponent cellsList = ecsManager.GetVSingletonComponent<CellsListComponent>();
            List<PositionComponent> positions = ecsManager.GetVComponentsFromList<PositionComponent>(cellsList.cellIds);
            TerrainCellFinderSystem cellFinderSystem = ecsManager.GetSystem<TerrainCellFinderSystem>();
            foreach (PositionComponent p in positions) {
                CellScoresComponent.cellScores.Add(p.position, 0);
            }

            // generically gets all entities that are targetable by the AI, has a position component, and ripples the scores the their positions.
            List<VEntity> enemyTargets = ecsManager.FilterEntities(test: (ventity) => {
                return VEntityComponentSystemManager.HasVComponent<EnemyTargetComponent>(ventity) && VEntityComponentSystemManager.HasVComponent<PositionComponent>(ventity);
            });

            foreach (VEntity targetEntity in enemyTargets) {
                PositionComponent targetComponent = targetEntity.GetVComponent<PositionComponent>();
                RippleScores(targetComponent.position, 5, CellScoresComponent.cellScores);
            }

            List<VEntity> enemies = ecsManager.GetSystem<UnitFinderSystem>().GetAllEnemyUnits().ToList();
            foreach (VEntity enemy in enemies) {
                var availableDestinations = ecsManager.GetSystem<PathingSystem>().GetAvailableDestinations(VEntityComponentSystemManager.GetVComponent<PositionComponent>(enemy).position, 5);

                var moveScores = new Dictionary<Coord, int>();
                foreach (Coord cell in availableDestinations.Keys) {
                    int s = 0;
                    foreach (Coord neighbor in cell.GetSurroundingCoords(true, false).FindAll(c => cellFinderSystem.IsValidCell(c))) {
                        s += CellScoresComponent.cellScores[neighbor];
                    }
                    moveScores.Add(cell, s);
                }

                var sortedMoveScore = moveScores.OrderByDescending(x => x.Value);

                // destination cell is where the unit is going to move to, defaulting to its original position
                Coord destinationCell = VEntityComponentSystemManager.GetVComponent<PositionComponent>(enemy).position;
                if (sortedMoveScore.Count() == 1) {
                    destinationCell = sortedMoveScore.First().Key;
                } else if (sortedMoveScore.Count() > 1) {
                    // randomly choose between the top two choices if there are more than 1
                    destinationCell = sortedMoveScore.ElementAt(Random.Range(0, 2)).Key;
                }
                ecsManager.ExecuteImmediateEvent("EnemyMove", component : new MovementEvent {
                    sourceId = enemy.id,
                        targetCoord = destinationCell
                });

                ecsManager.ExecuteImmediateEvent("QueueAction", component : new QueueActionEvent {
                    entityId = enemy.id
                });
            }
        }

        /// <summary>
        /// Recursive function that ripples a decreasing scores across the board, only updating if the current score is higher
        /// </summary>
        private void RippleScores(Coord c, int score, Dictionary<Coord, int> cellScores) {
            // base case
            if (cellScores[c] >= score) {
                return;
            }

            cellScores[c] = score;

            if (score <= 0) {
                return;
            }

            // recursive case
            foreach (Coord neighbor in c.GetSurroundingCoords()) {
                if (ecsManager.GetSystem<TerrainCellFinderSystem>().IsValidCell(neighbor)) {
                    RippleScores(neighbor, score - 1, cellScores);
                }
            }
        }
    }

    [System.Serializable]
    public class EnemyAISystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<AIComponent>(entity);
        }

        protected override void OnEnemySetupStart(VEntity entity) {

        }
    }
}