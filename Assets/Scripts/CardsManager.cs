
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GwentEngine;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public static class CardsManager
{
    public static Dictionary<int, CardMetadata> CardMetadata = null;
    
    public static ConcurrentDictionary<int, (GameObject gameObject, Card card, PlayerKind player, Location location, int
        position)> CardGameObjects =
        new ConcurrentDictionary<int, (GameObject gameObject, Card card, PlayerKind player, Location location, int
            position)>();
    
    public static void UdateCardGameObjects(int card)
    {
        CardGameObjects.TryRemove(card, out var removedValue);
    }
    
    public static (GameObject gameObject, Card card, PlayerKind player, Location location, int index)[] GenerateCardGameObjects(
        IEnumerable<Card> cards, PlayerKind player, Location location, IManager manager)
    {
        var cardGameObjects = cards.Select((card, index) =>
            CardGameObjects.AddOrUpdate(card.Number,
                key => (CreateNewCard(card, manager), card, player, location, index),
                (key, existing) => (existing.gameObject, existing.card, player, location, index)
            )
        ).ToArray();

        return cardGameObjects;
    }
    
    public static GameObject CreateNewCard(Card card, IManager manager)
    {
        var gameObject = manager.InstantiateCard();
        var cardImage = GameObject.Find(card.Metadata.Name);
        Image image = gameObject.GetComponent<Image>();
        if (card.IsHero || card.Number > 180)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        image.sprite = cardImage.GetComponent<SpriteRenderer>().sprite;
        return gameObject;
    }
}
