using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Voidheart {
    [CreateAssetMenu]
    public class CardListScriptableObject : SerializedScriptableObject {
        public List<string> cardNames;

        public void Add(string cardName) {
            cardNames.Add(cardName);
        }
    }
}