using System;
using System.Collections.Generic;

namespace Voidheart {

    /// <summary>This system sets up the entirety of the other systems and components present.</summary>
    [Serializable]
    public class StartGameSystem : VSystem {

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<StartGameComponent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            VEntity singletonEntity = ecsManager.CreateEntity("SingletonContainer");
            ecsManager.AddComponent(singletonEntity, new CurrentLifecycleComponent {
                currentLifecycle = VLifecycle.EnemySetupStart
            });
            ecsManager.AddComponent(singletonEntity, new TurnCounterComponent {
                turnCounter = 0
            });
            ecsManager.AddComponent(singletonEntity, new CardZoneDataComponent {
                zones = new Dictionary<Zone, List<string>> {
                    {
                        Zone.DECK, new List<string>()
                    },
                    {
                        Zone.HAND,
                        new List<string>()
                    },
                    {
                        Zone.DISCARD,
                        new List<string>()
                    },{
                        Zone.INPLAY,
                        new List<string>()
                    }
                }
            });
            ecsManager.AddComponent(singletonEntity, new CellsListComponent {
                cellIds = new List<string>()
            });
            ecsManager.AddComponent(singletonEntity, new UnitsList {
                unitIds = new List<string>()
            });
            ecsManager.AddComponent(singletonEntity, new PlayerManaComponent {
                currMana = 0,
                    maxMana = 4
            });
            ecsManager.AddComponent(singletonEntity, new PlayerHandComponent {
                startTurnDrawCount = 5,
                    maxHandSize = 10,
            });
            ecsManager.AddComponent(singletonEntity, new CellScoresComponent{
                cellScores = new Dictionary<Coord, int>()
            });

            VEntity firstLifecycleEvent = ecsManager.CreateEvent("FirstLifecycleEvent");
            ecsManager.AddComponent(firstLifecycleEvent, new SetLifecycleEventComponent {
                newVLifecycle = VLifecycle.EnemySetupStart
            });
        }
    }
}