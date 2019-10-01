
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;


[CreateAssetMenu]
public class CardScriptableObject1 : SerializedScriptableObject
{
    public string cardName;
    [TextArea(3, 10)]
    public string cardDescription;
    public List<CardEffect> cardEffects = new List<CardEffect>();
    public CardTargeter cardTargeter = new CardTargeter();
    public Sprite cardSprite;
    //public CharacterLogic associatedCharacter;
    public int energyCost;
}

public enum CardTargeterType
{
    Single,
    Team,
    Area
}

public class CardTargeter
{
    public CardTargeterType type;
    public Dictionary<string, string> targetData = new Dictionary<string, string>();
}

public class CardEffect
{
    public enum EffectType
    {
        Damage,
        Heal,
        Buff
    }

    public EffectType type;
    public Dictionary<string, string> effectStats = new Dictionary<string, string>();

    public void ApplyEffect()
    {
       
    }

    [Button("Fill Dictionary with effect values")]
    private void FillDictionary()
    {
        switch (type)
        {
            case EffectType.Damage:
                effectStats["i_damage"] = "0";
                break;
            case EffectType.Buff:
                effectStats["s_bufftype"] = "energy";
                effectStats["i_buffvalue"] = "0";
                break;
        }
    }
}


public class CardIntensityEffect : CardEffect{
    public int intensity = 0;
}
