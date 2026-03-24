using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public static class Utility {

	public static int MonsterCount(Player player) {
		int count = 0;
		foreach (CardInstance card in player.field.MonsterZones) {
			if (card != null) count++;
		}
		return count;
	}

	public static void Shuffle<T>(List<T> list) {
		for(int i=list.Count-1; i>0; i--) {
			int randomIndex = UnityEngine.Random.Range(0, i + 1);
			T temp = list[i];
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
		}
	}

	public static int BackrowCount(Player player) {
		int count = 0;
		foreach (CardInstance card in player.field.SpellTrapZones) {
			if (card != null) count++;
		}
		return count;
	}

public static void AttachEffects(CardInstance instance, DuelController duelController)
{
    foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
        .Where(t =>
            typeof(CardEffectComponent).IsAssignableFrom(t) &&
            !t.IsAbstract))
    {
        var nameField = type.GetField("Name", BindingFlags.Public | BindingFlags.Static);

        if (nameField == null)
            continue;

        string effectName = nameField.GetValue(null) as string;
        string cardName = instance.CardData.Name;

        // Optional: normalize to avoid hidden character issues
        string Normalize(string s) => s?.Replace("’", "'").Trim().ToLower();

        if (Normalize(effectName) == Normalize(cardName))
        {
            var effect = (CardEffectComponent)Activator.CreateInstance(type);

            instance.AddComponent(effect);
            effect.Register(duelController, instance);
            Debug.Log($"[ATTACH] InstanceID: {instance.GetHashCode()} | Card: {cardName}");
            Debug.Log($"[AttachEffects] Attached {type.Name} to {cardName}");
            return;
        }
    }

    Debug.LogWarning($"[AttachEffects] No effect found for {instance.CardData.Name}");
}	

	
}