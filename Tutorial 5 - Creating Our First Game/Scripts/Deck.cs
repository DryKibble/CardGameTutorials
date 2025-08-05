//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using Godot;
using System;

public partial class Deck : Node3D
{
  [Export]
  private byte curSize;
  public byte CurSize
  {
    //Anything can get a Deck's curSize
    get { return curSize; }
    //Only a Deck can set its own curSize
    private set { curSize = Math.Clamp(value, (byte)0, MAXSIZE); }
  }
  public Card TopCard
  {
    get { return GetChildOrNull<Card>(curSize - 1); }
  }
  public static byte MAXSIZE = 52;
  private static RandomNumberGenerator rng = new();

  //Returns a string representation of the passed in deck
  public static string ToString(Deck deck)
  {
    string str = "";
    for (byte i = 0; i < deck.CurSize; i++)
    {
      str += Card.ToString(deck.GetChild<Card>(i));
      str += "\n";
    }
    return str;
  }

  //Randomly shuffles around the children nodes of the passed in deck
  public static void Shuffle(Deck deck)
  {
    int h;
    for (int i = 0; i < deck.CurSize; i++)
    {
      rng.Randomize();
      h = rng.RandiRange(0, deck.CurSize);
      deck.MoveChild(deck.GetChild(i), h);
    }
    GD.Print(ToString(deck));
  }

  //Reset's each child node's Position relative to their index
  public static void Reset(Deck deck)
  {
    Vector3 reset = Vector3.Zero;
    for (byte i = 0; i < deck.CurSize; i++)
    {
      reset.Y = Card.HEIGHT * i;
      deck.GetChild<Card>(i).Position = reset;
    }
  }

  //Deals every Card from the from Deck as evenly as possible
  // to every Deck in the Deck[] to
  public static void Deal(Deck from, Deck[] to)
  {
    for (sbyte i = (sbyte)(from.CurSize - 1); i >= 0; i--)
    {
      AddCard(to[i % to.Length], RemoveCard(from, from.TopCard));
    }

    for (byte i = 0; i < to.Length; i++)
    {
      Reset(to[i]);
    }
  }

  //Adds a given Card to a given deck
  public static void AddCard(Deck deck, Card card)
  {
    deck.AddChild(card);
    deck.CurSize++;
  }

  //Removes a given Card from a Deck, but returns it in case
  //  the Card is needed for further use
  public static Card RemoveCard(Deck deck, Card card)
  {
    if (card is not null)
    {
      deck.RemoveChild(card);
      deck.CurSize--;
      return card;
    }
    return null;
  }
}