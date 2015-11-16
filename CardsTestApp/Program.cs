using PlayingCards.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Card> deck = new List<Card>();
            for (int i = 0; i < 52; i++)
            {
                deck.Add(new Card(i));
            }

            for (int j = 0; j < 52; j++)
            {
                Console.WriteLine(String.Format("{0} \t\t {1}", deck[j].FullName , deck[j].ShortName));
            }

            Console.ReadLine();

        }
    }
}
