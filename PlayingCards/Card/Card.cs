using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayingCards.Card
{
    /// <summary>
    /// Игральная карта
    /// </summary>
    public class Card
    {
        public Suit Suit { get; set; }

        public Rank Rank { get; set; }

        public string FullName => String.Format("{0} of {1}", Rank.ToString(), Suit.ToString());

        /// <summary>
        /// Конструктор по-умолчанию. Создаст туза пик
        /// </summary>
        public Card()
        {
            Rank = Rank.Ace;
            Suit = Suit.Spades;
        }

        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;                
        }

        public Card(int num)
        {
            Suit = (Suit)(num/13) + 1;
            Rank = (Rank)(num % 13) + 1;
        }
    }
}
