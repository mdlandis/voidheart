using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Voidheart {

    [Serializable]
    public abstract class VSystem {
        protected VEntityComponentSystemManager ecsManager = null;

        public void Init(VEntityComponentSystemManager newEcsManager) {
            this.ecsManager = newEcsManager;
        }

        public abstract bool ShouldOperate(VEntity entity);

        public void OnLifecycle(VEntity entity, VLifecycle lifecycle) {
            switch (lifecycle) {
                case VLifecycle.EnemySetupStart:
                    OnEnemySetupStart(entity);
                    break;
                case VLifecycle.EnemySetupExecute:
                    OnEnemySetupExecute(entity);
                    break;
                case VLifecycle.PlayerTurnStart:
                    OnPlayerTurnStart(entity);
                    break;
                case VLifecycle.PlayerTurnExecute:
                    OnPlayerTurnExecute(entity);
                    break;
                case VLifecycle.PlayerTurnEnd:
                    OnPlayerTurnEnd(entity);
                    break;
                case VLifecycle.EnemyActionsStart:
                    OnEnemyActionsStart(entity);
                    break;
                case VLifecycle.EnemyActionsResolve:
                    OnEnemyActionsResolve(entity);
                    break;
                case VLifecycle.EnemyActionsEnd:
                    OnEnemyActionsEnd(entity);
                    break;
                case VLifecycle.OnBeforeEvent:
                    OnBeforeEvent(entity);
                    break;
                case VLifecycle.OnExecuteEvent:
                    OnExecuteEvent(entity);
                    break;
                case VLifecycle.OnAfterEvent:
                    OnAfterEvent(entity);
                    break;
                default:
                    throw new Exception("Unhandled lifecycle");
            }
        }

        protected virtual void OnEnemySetupStart(VEntity entity) { }
        protected virtual void OnEnemySetupExecute(VEntity entity) { }
        protected virtual void OnPlayerTurnStart(VEntity entity) { }
        protected virtual void OnPlayerTurnExecute(VEntity entity) { }
        protected virtual void OnPlayerTurnEnd(VEntity entity) { }
        protected virtual void OnEnemyActionsStart(VEntity entity) { }
        protected virtual void OnEnemyActionsResolve(VEntity entity) { }
        protected virtual void OnEnemyActionsEnd(VEntity entity) { }
        protected virtual void OnBeforeEvent(VEntity eventEntity) { }
        protected virtual void OnExecuteEvent(VEntity eventEntity) { }
        protected virtual void OnAfterEvent(VEntity eventEntity) { }
    }

}