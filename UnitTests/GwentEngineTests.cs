using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace GwentEngine
{
    public class BaseTests 
    { 
        public static string DeckFilePath = @"..\..\..\..\..\..\Assets\Cards\Deck.json";
    }

    [TestClass]
    public class CardMetadataTests : BaseTests
    {
        [TestMethod]
        public void CanReadDeckMetadata()
        {
            var metadata = CardMetadata.FromFile(DeckFilePath);

            Assert.AreEqual(179, metadata.Count);

            var firstCardMetadata = metadata[14];

            Assert.AreEqual("commanders_horn_card", firstCardMetadata.Name);
            Assert.AreEqual(Ability.commandersHorn, firstCardMetadata.Ability);
            Assert.AreEqual(-1, firstCardMetadata.DefaultPower);
            Assert.AreEqual(false, firstCardMetadata.IsHero);
            Assert.AreEqual(Location.ComandersHornCatapult|Location.ComandersHornSword|Location.ComandersHornArchery, firstCardMetadata.PossibleLocations);
            Assert.AreEqual(14, firstCardMetadata.Number);
            Assert.AreEqual(4, firstCardMetadata.Faction);
        }
    }

    [TestClass]
    public class GameStateTest : BaseTests
    {
        private const int commanders_horn_card = 14;
        private const int geralt_of_rivia_card = 0;
        private const int dandelion_card = 6;
        private const int vesemir_card = 2;

        private Dictionary<int, CardMetadata> _metadata;
        private GameState _gameState;

        [TestInitialize]
        public void Setup()
        {
            _metadata = CardMetadata.FromFile(DeckFilePath);
            _gameState = new GameState();
        }

        private void _gameState_OnCardChanged(object sender, CardInfo e)
        {
        }

        private void _gameState_OnCardAdded(object sender, CardInfo e)
        {
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CannotUndoOnAnEmptyBoard()
        {
            _gameState.Undo();
        }

        [TestMethod]
        public void NewGameShouldAddCardsInPlayerAndOpponantsHands()
        {
            _gameState.NewGame(_metadata, Settings.InitialCardCount);

            var playerCardsInHand = _gameState.GetCards(PlayerKind.Player, Location.Hand);
            var opponentCardsInHand = _gameState.GetCards(PlayerKind.Opponent, Location.Hand);

            Assert.AreEqual(Settings.InitialCardCount, playerCardsInHand.Length);
            Assert.AreEqual(Settings.InitialCardCount, opponentCardsInHand.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CannotPlayACardWhenLocationIsNotAllowed()
        {
            _gameState.NewGame(_metadata);
            _gameState.UseCard(commanders_horn_card, PlayerKind.Player);
            _gameState.Play(commanders_horn_card, Location.Catapult);
        }

        [TestMethod]
        public void CanPlayACardWhenInAllowedLocation()
        {
            _gameState.NewGame(_metadata);
            _gameState.UseCard(commanders_horn_card, PlayerKind.Player);

            _gameState.Play(commanders_horn_card, Location.ComandersHornArchery);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CannotPlayCardNotDrawnYet()
        {
            _gameState.NewGame(_metadata);

            _gameState.Play(commanders_horn_card, Location.ComandersHornArchery);
        }

        [TestMethod]
        public void WhenOnlyDandelionPresent_PowerIsNotDoubledOnHero()
        {
            _gameState.NewGame(_metadata);
            _gameState.UseCard(geralt_of_rivia_card, PlayerKind.Player);
            _gameState.UseCard(dandelion_card, PlayerKind.Player);

            _gameState.Play(geralt_of_rivia_card, Location.Sword);
            _gameState.Play(dandelion_card, Location.Sword);

            var cards = _gameState.GetCards(PlayerKind.Player, Location.Sword);

            Assert.AreEqual(2, cards.Length);

            Assert.AreEqual(geralt_of_rivia_card, cards[0].Number);
            Assert.AreEqual(dandelion_card, cards[1].Number);

            Assert.AreEqual(15, cards[0].Power);
            Assert.AreEqual(2, cards[1].Power);
        }

        [TestMethod]
        public void WhenOnlyDandelionPresent_PowerIsDoubledOnNonHeroes()
        {
            _gameState.NewGame(_metadata);
            _gameState.UseCard(vesemir_card, PlayerKind.Player);
            _gameState.UseCard(dandelion_card, PlayerKind.Player);

            _gameState.Play(vesemir_card, Location.Sword);
            _gameState.Play(dandelion_card, Location.Sword);

            var cards = _gameState.GetCards(PlayerKind.Player, Location.Sword);

            Assert.AreEqual(2, cards.Length);

            Assert.AreEqual(vesemir_card, cards[0].Number);
            Assert.AreEqual(dandelion_card, cards[1].Number);

            Assert.AreEqual(12, cards[0].Power);
            Assert.AreEqual(2, cards[1].Power);
        }

        [TestMethod]
        public void WhenDandelionAndCommandersHornPresent_PowerIsDoubledCorrectly()
        {
            _gameState.NewGame(_metadata);
            _gameState.UseCard(commanders_horn_card, PlayerKind.Player);
            _gameState.UseCard(vesemir_card, PlayerKind.Player);            
            _gameState.UseCard(dandelion_card, PlayerKind.Player);

            _gameState.Play(commanders_horn_card, Location.ComandersHornSword);
            _gameState.Play(vesemir_card, Location.Sword);
            _gameState.Play(dandelion_card, Location.Sword);

            var cards = _gameState.GetCards(PlayerKind.Player, Location.Sword);

            Assert.AreEqual(2, cards.Length);

            Assert.AreEqual(vesemir_card, cards[0].Number);
            Assert.AreEqual(dandelion_card, cards[1].Number);

            Assert.AreEqual(12, cards[0].Power);
            Assert.AreEqual(4, cards[1].Power);

            cards = _gameState.GetCards(PlayerKind.Player, Location.ComandersHornSword);
            Assert.AreEqual(1, cards.Length);

            Assert.AreEqual(commanders_horn_card, cards[0].Number);
            Assert.AreEqual(-1, cards[0].Power);
        }

        [TestMethod]
        public void NewGameShouldNotGiveTheSameCardTwice()
        {
            _gameState.NewGame(_metadata, Settings.InitialCardCount);

            var cardNumbers = _gameState.GetCards(PlayerKind.Player, Location.Hand).Select(c => c.Number)
                .Union(
                _gameState.GetCards(PlayerKind.Opponent, Location.Hand).Select(c => c.Number))
                .Distinct()
                .ToArray();

            Assert.AreEqual(2 * Settings.InitialCardCount, cardNumbers.Length);
        }
    }
}