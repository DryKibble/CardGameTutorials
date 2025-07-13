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

  [Export]
  private byte ExtraNodes;
  public byte CurSize
  {
    //Anything can get a Deck's curSize
    get { return curSize; }
    //Only a Deck can set its own curSize
    private set { curSize = Math.Clamp(value, (byte)0, MAXSIZE); }
  }
  public Card TopCard
  {
    get { return GetChildOrNull<Card>(curSize+ExtraNodes - 1); }
  }
  public static byte MAXSIZE = 52;
  private static RandomNumberGenerator rng = new();

  //Returns a string representation of the passed in deck
  //Prints out the Cards in order starting from the Cards at the bottom
  //  working its way to the Cards at the top
  //Ie, in the output, the bottom rows of text are the topmost Cards
  //  and the top rows of text are the bottommost Cards
  public static string ToString(Deck deck)
  {
    string str = "";
    for (byte i = deck.ExtraNodes; i < deck.CurSize + deck.ExtraNodes; i++)
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
    for (int i = deck.ExtraNodes; i < deck.CurSize+deck.ExtraNodes; i++)
    {
      rng.Randomize();
      h = rng.RandiRange(deck.ExtraNodes, deck.CurSize+deck.ExtraNodes);
      deck.MoveChild(deck.GetChild(i), h);
    }
  }

  //Resets each child node's Position relative to their index
  public static void ResetPosition(Deck deck)
  {
    Vector3 reset = Vector3.Zero;
    for (byte i = deck.ExtraNodes; i < deck.CurSize+deck.ExtraNodes; i++)
    {
      reset.Y = Card.HEIGHT * (i - deck.ExtraNodes);
      Card temp = deck.GetChildOrNull<Card>(i);
      temp.Position = reset;
    }
  }

  //Resets each child node's Rotation relative to be face down and aligned
  public static void ResetRotation(Deck deck)
  {
    Vector3 reset = Vector3.Zero;
    reset.Z = Mathf.Pi;
    for (byte i = deck.ExtraNodes; i < deck.CurSize+deck.ExtraNodes; i++)
    {
      deck.GetChild<Card>(i).Rotation = reset;
    }
  }

  //Deals every Card from the from Deck as evenly as possible
  // to every Deck in the Deck[] to
  public static void Deal(Deck from, Deck[] to)
  {
    for (sbyte i = (sbyte)(from.CurSize+from.ExtraNodes - 1); i >= from.ExtraNodes; i--)
    {
      AddCard(to[i % to.Length], RemoveCard(from, from.TopCard));
    }

    for (byte i = 0; i < to.Length; i++)
    {
      ResetPosition(to[i]);
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