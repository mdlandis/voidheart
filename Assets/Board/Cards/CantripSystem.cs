using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voidheart
{

    [Serializable]
    public class CantripUseEvent : VComponent
    {
        public string cantripEffectCard;
    }

    public class CantripSystem : VSystem
    {

        public bool cantripUsed = false;

        public override bool ShouldOperate(VEntity entity)
        {
            return VEntityComponentSystemManager.HasVComponent<CantripUseEvent>(entity);
        }

        protected override void OnExecuteEvent(VEntity eventEntity)
        {
            cantripUsed = true;
        }

        protected override void OnPlayerTurnStart(VEntity entity)
        {
            cantripUsed = false; ;
        }
    }
}