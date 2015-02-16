using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            //function calls in Main are for my own verification and understanding
            PokerPlayer player1 = new PokerPlayer();
            Deck newDeck = new Deck();

            newDeck.Shuffle();

            player1.DrawHand(newDeck.Deal(5));

            player1.ShowHand();

            Console.ReadKey();
        }
    }

    /// <summary>
    /// Poker player that has a hand of cards
    /// </summary>
    class PokerPlayer
    {
        // List of cards
        public List<Card> CurrentHand { get; set; }
        
        // Enum of different hand types
        public enum HandType
        {
            HighCard = 1, OnePair , TwoPair, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, StraightFlush, RoyalFlush
        }

        // Rank of hand that player holds
        public HandType HandRank
        {
            get
            {
                if (HasRoyalFlush())
                {
                    return HandType.RoyalFlush;
                } if (HasStraightFlush())
                {
                    return HandType.StraightFlush;
                } if (HasFourOfAKind())
                {
                    return HandType.FourOfAKind;
                } if (HasFullHouse())
                {
                    return HandType.FullHouse;
                } if (HasFlush())
                {
                    return HandType.Flush;
                } if (HasStraight())
                {
                    return HandType.Straight;
                } if (HasThreeOfAKind())
                {
                    return HandType.ThreeOfAKind;
                } if (HasTwoPair())
                {
                    return HandType.TwoPair;
                } if (HasPair())
                {
                    return HandType.OnePair;
                }
                return HandType.HighCard;
            }
        }

        // Constructor that isn't used
        public PokerPlayer() { }

        //get a list of Cards to form a "hand"
        public void DrawHand(List<Card> currentHand)
        {
            this.CurrentHand = currentHand;
        }

        //print to console the suit and rank of each card in hand, and the handrank
        public void ShowHand()
        {
            foreach (Card card in this.CurrentHand)
            {
                Console.WriteLine(card.Rank + " " + card.Suit);
            }
            Console.WriteLine(this.HandRank);
        }

        //functions to check HandRank
        public bool HasPair()
        {
            return (this.CurrentHand.GroupBy(x => x.Rank).Count(x => x.Count() == 2) == 1);
        }
        public bool HasTwoPair()
        {
            return (this.CurrentHand.GroupBy(x => x.Rank).Count(x => x.Count() == 2) == 2);
        }
        public bool HasThreeOfAKind()
        {
            return (this.CurrentHand.GroupBy(x => x.Rank).Count(x => x.Count() == 3) == 1) && !HasPair();
        }
        public bool HasStraight()
        {
            return (this.CurrentHand.GroupBy(x => x.Rank).Count() == this.CurrentHand.Count) && (((this.CurrentHand.Max(x => x.Rank)) - (this.CurrentHand.Min(x => x.Rank))) == 4);
        }
        public bool HasFlush()
        {
            return (this.CurrentHand.Select(x => x.Suit).Distinct().Count() == 1);
        }
        public bool HasFullHouse()
        {
            return (this.CurrentHand.GroupBy(x => x.Rank).Count(x => x.Count() == 3) == 1) && HasPair();
        }
        public bool HasFourOfAKind()
        {
            return ((this.CurrentHand.GroupBy(x => x.Rank).Count(x => x.Count() == 4) == 1) 
                && (this.CurrentHand.GroupBy(x => x.Rank).OrderByDescending(x => x.Count()).First().Select(x => x.Suit).Distinct().Count() == 4));
        }
        public bool HasStraightFlush()
        {
            return (HasFlush() && HasStraight());
        }
        public bool HasRoyalFlush()
        {
            return ((HasStraightFlush()) && (this.CurrentHand.OrderByDescending(x => x.Rank).First().Rank == Rank.Ace));
        }
    }

    /// <summary>
    /// Deck of 52 playing cards
    /// </summary>
    class Deck
    {
        //Declare Properties
        public int CardsRemaining { get; set; }
        public List<Card> DeckOfCards { get; set; }
        public List<Card> DiscardedCards { get; set; }

        /// <summary>
        /// CONSTRUCTOR: Creates a deck of 52 playing cards
        /// </summary>
        public Deck()
        {
            //initialize Properties
            this.CardsRemaining = 0;
            this.DeckOfCards = new List<Card>();
            this.DiscardedCards = new List<Card>();
            //initialize DeckOfCards with 52 unique cards of each Rank and Suit
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 2; j <= 14; j++)
                {
                    this.DeckOfCards.Add(new Card(j, i));
                }
            }
        }

        /// <summary>
        /// CONSTRUCTOR: Creates a deck of (52 * numberOfDecks) cards
        /// </summary>
        /// <param name="numberOfDecks">number of decks</param>
        public Deck(int numberOfDecks)
        {
            //initialize Properties
            this.CardsRemaining = 0;
            this.DeckOfCards = new List<Card>();
            this.DiscardedCards = new List<Card>();
            //initialize DeckOfCards with 52 unique cards of each Rank and Suit for (numberOfDecks) times
            for (int n = 0; n < numberOfDecks; n++)
            {
                for (int i = 1; i <= 4; i++)
                {
                    for (int j = 2; j <= 14; j++)
                    {
                        this.DeckOfCards.Add(new Card(j, i));
                    }
                }
            }
        }

        /// <summary>
        /// Rearrange Cards in the DeckOfCards at random
        /// </summary>
        public void Shuffle()
        {
            //add DiscardedCards pile to the DeckOfCards deck
            this.DeckOfCards.AddRange(DiscardedCards);
            //clear all cards from the DiscardedCards pile
            this.DiscardedCards.Clear();
            //create a random number generator
            Random rng = new Random();
            //grab a random card from the DeckOfCards, remove it, and add it to the front of the deck.  Continue until all cards have been placed in a new random position
            for (int i = 0; i < this.DeckOfCards.Count; i++)
            {
                int randomIndex = rng.Next(i, this.DeckOfCards.Count);
                this.DeckOfCards.Insert(i, this.DeckOfCards[randomIndex]);
                this.DeckOfCards.RemoveAt(randomIndex + 1);
            }
        }

        /// <summary>
        /// Returns a List of Cards based on number requested
        /// </summary>
        /// <param name="numberOfCards">number of cards requested</param>
        /// <returns>List of Cards</returns>
        public List<Card> Deal(int numberOfCards)
        {
            //temporary List to hold number of Cards requested
            List<Card> dealtCards = this.DeckOfCards.Take(numberOfCards).ToList();
            //remove the requested cards from original deck
            this.DeckOfCards.RemoveRange(0, 5);
            return dealtCards;
        }

        /// <summary>
        /// Adds discarded card to the DiscardedCards deck
        /// </summary>
        /// <param name="card">Card to discard</param>
        public void Discard(Card card)
        {
            this.DiscardedCards.Add(card);
        }

        /// <summary>
        /// Adds discarded cards to the DiscardedCards deck
        /// </summary>
        /// <param name="cards">List of Cards to discard</param>
        public void Discard(List<Card> cards)
        {
            this.DiscardedCards.AddRange(cards);
        }
    }

    /// <summary>
    /// Playing card that has a rank and a suit
    /// </summary>
    class Card
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }

        public Card(int rank, int suit)
        {
            this.Rank = (Rank)rank;
            this.Suit = (Suit)suit;
        }
    }

    //enums of SUIT AND RANK
    public enum Suit
    {
        Heart = 1,
        Diamond,
        Club,
        Spade
    }
    public enum Rank
    {
        Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
    }

}

