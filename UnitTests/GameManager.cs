using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GwentEngine
{
    public class GameManager
    {
        public CardMetadata[] AllAvailableCards
        {
            get
            {
                return new CardMetadata[0];
            }
        }


        public void UseCard(int number, PlayerKind player)
        {     
        }

        public void Play(int number, Location location)
        {
        }

        public void EndCurrentPhase()
        {
        }

        public Card[] GetCards(PlayerKind player, Location location)
        {
            return new Card[0];
        }
    }
}
