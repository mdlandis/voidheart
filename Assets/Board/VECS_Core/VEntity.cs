using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Voidheart {
    [Serializable]
    public class VEntity {
        public string id;
        [ShowInInspector]
        public IList<VComponent> Components = new List<VComponent>();

        public T GetVComponent<T>() where T : VComponent {
            return VEntityComponentSystemManager.GetVComponent<T>(this);
        }

        public bool HasVComponent<T>() where T : VComponent {
            return VEntityComponentSystemManager.HasVComponent<T>(this);
        }
    }

    [Serializable]
    public abstract class VComponent { }

    [Serializable]
    public abstract class VComponentSingleton : VComponent { }

    [Serializable]
    public class StartGameComponent : VComponent { }
}