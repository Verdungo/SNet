using PlayingCards.Cards;
using PlayingCards.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CardsTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Deck deck1 = new Deck();
            Deck deck2 = new Deck();

            int step = 0;
            bool flag = false;
            while (!flag)
            {
                deck2.Shuffle();
                for (int j = 0; j < 52; j++)
                {
                    if (deck1[j].ShortName == deck2[j].ShortName) flag = true;
                }
                step++;
            }

            for (int j = 0; j < 52; j++)
            {
                Console.WriteLine(String.Format("{0} \t{2}\t {1}", deck1[j].ShortName, deck2[j].ShortName, (deck1[j].ShortName == deck2[j].ShortName)?"-----":"     "));
            }
            Console.WriteLine(step);

            Console.ReadLine();

        }
    }
}
