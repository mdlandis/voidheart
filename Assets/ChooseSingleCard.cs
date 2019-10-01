using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public class ChooseSingleCard : MonoBehaviour
    {
        public VEntity selectedCard = null;

        public void UnselectAll() {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<MetaCardGameObject>().Unselect();
            }
        }
    }
}