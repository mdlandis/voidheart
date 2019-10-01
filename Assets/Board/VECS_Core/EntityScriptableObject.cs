using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace Voidheart {
    [CreateAssetMenu]
    public class EntityScriptableObject : SerializedScriptableObject
    {
        [OdinSerialize, NonSerialized]
        public VEntity entity = new VEntity();


        //public Sprite cardArt;

        //public Dictionary<string, string> parameters = new Dictionary<string, string>();
    }
}
