using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Voidheart {
    public class StringUtils {
        public static string ClassEnumToString(CardType cardType) {
            if (cardType == CardType.ORION) {
                return "Orion";
            } else if (cardType == CardType.OPHELIA) {
                return "Ophelia";
            } else if (cardType == CardType.NEUTRAL) {
                return "Neutral";
            } else {
                return "Other";
            }

        }
    }
}