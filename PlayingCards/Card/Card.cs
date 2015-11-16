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

        public string ShortName
        {
            get
            {
                string res = "";
                switch (Rank)
                {
                    case Rank.Ace:
                        res += "A";
                        break;
                    case Rank.Ten:
                        res += "T";
                        break;
                    case Rank.Jack:
                        res += "J";
                        break;
                    case Rank.Queen:
                        res += "Q";
                        break;
                    case Rank.King:
                        res += "K";
                        break;
                    default:
                        res += ((int)Rank).ToString();
                        break;
                }
                switch (Suit)
                {
                    case Suit.Clubs:
                        res += "♣";
                        break;
                    case Suit.Diamonds:
                        res += "♦";
                        break;
                    case Suit.Hearts:
                        res += "♥";
                        break;
                    case Suit.Spades:
                        res += "♠";
                        break;
                    default:
                        res += "X";
                        break;
                }
                return res;
            }
        }

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
            Suit = (Suit)((num / 13) + 1);
            Rank = (Rank)((num % 13) + 1);
        }
    }
}
