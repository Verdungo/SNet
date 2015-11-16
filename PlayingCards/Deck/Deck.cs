using PlayingCards.Cards;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Collections;

namespace PlayingCards.Deck
{
    public class Deck : Collection<Card>
    {
        private static Random _shuffleRandom = new Random();

        public Deck(bool shuffled = false)
        {
            for (int i = 0; i < 52; i++)
            {
                Add(new Card(i));
            }
            if (shuffled) Shuffle();
        }

        public void Shuffle()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                int newRandom = _shuffleRandom.Next(i);
                Card temp = this[newRandom];
                this[newRandom] = this[i];
                this[i] = temp;
            }
        }
    }
}
