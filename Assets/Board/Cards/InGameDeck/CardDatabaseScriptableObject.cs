using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Voidheart {
    [CreateAssetMenu]
    public class CardDatabaseScriptableObject : SerializedScriptableObject {
        public List<EntityScriptableObject> Cards;
    }
}