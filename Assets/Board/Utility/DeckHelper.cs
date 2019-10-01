using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public static class DeckHelper {
        public static void Shuffle(IList list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = Random.Range(0, n + 1);
                object value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}