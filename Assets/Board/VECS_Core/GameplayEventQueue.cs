using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Voidheart {
    public class GameplayEventQueue {
        [ShowInInspector]
        Deque<VEntity> gameplayEventQueue;
        VEntityComponentSystemManager game;

        public GameplayEventQueue(VEntityComponentSystemManager gameReference) {
            game = gameReference;
            gameplayEventQueue = new Deque<VEntity>();
        }

        public void Step(bool removeFromFront = false) {
            if (!gameplayEventQueue.IsEmpty) {
                VEntity e = removeFromFront ? gameplayEventQueue.RemoveFront() : gameplayEventQueue.RemoveBack();
                game.RunSystems(VLifecycle.OnBeforeEvent, false, new VEntity[] {
                    e
                });
                game.RunSystems(VLifecycle.OnExecuteEvent, false, new VEntity[] {
                    e
                });
                game.RunSystems(VLifecycle.OnAfterEvent, false, new VEntity[] {
                    e
                });
                game.MarkRemovalEntity(e);
                game.DoRemove();
            }
        }

        public void Flush() {
            while (!gameplayEventQueue.IsEmpty) {
                Step();

                if (gameplayEventQueue.Count == 1) {
                    return;
                }
            }
        }

        public void Push(VEntity e) {
            gameplayEventQueue.AddBack(e);
        }

        public void ImmediateExecute(VEntity e) {
            gameplayEventQueue.AddFront(e);
            Step(true);
        }
    }
}