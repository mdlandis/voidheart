using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace Voidheart {
    public class EventRaiser : SerializedMonoBehaviour 
    {
        [OdinSerialize, NonSerialized]
        public VEntity eventity;

        [Button]
        public void RaiseEvent()
        {
            var ecsManager = VEntityComponentSystemManager.Instance;
            ecsManager.StackEvent(ecsManager.InsantiateEntityFromBlueprint(eventity));
        }
    }
}
