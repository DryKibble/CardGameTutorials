//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using Godot;
using System;

public partial class SJGameManager : Node
{
  [Export]
  Deck deck;
  [Export]
  SJPlayer player;
  [Export]
  SJCPU cpu;
  SJPlayer[] players;
  SJPlayer CurPlayer;
  SJPlayer PrevPlayer;
  int numTurns = 0;
  private static Vector3 vec = new(0, Mathf.Pi, 0);
  private static RandomNumberGenerator rng = new();

  //Sets up the game parameters
  public override void _Ready()
  {
    Deck.Shuffle(deck);
    Deck.Deal(deck, [player.iDeck, cpu.iDeck]);
    player.IsMyTurn = true;
    players = [player, cpu];
    CurPlayer = player;
  }

  //Deals a card from player to the center Deck and lets the next person in
  //  the turn ordering go
  private void _OnDeal(SJPlayer player)
  {
    Deck.AddCard(deck, Deck.RemoveCard(player.iDeck, player.iDeck.TopCard));
    Deck.ResetPosition(deck);
    player.IsMyTurn = false;
    numTurns++;
    PrevPlayer = CurPlayer;
    CurPlayer = players[numTurns % players.Length];
    CurPlayer.IsMyTurn = true;
    rng.Randomize();
    vec.Y = rng.RandfRange(-Mathf.Pi / 4, Mathf.Pi / 4);
    deck.TopCard.GlobalRotation = vec;
    CardMesh.ShowMaterial(deck.TopCard);
  }

  //Deals all cards from the center deck to the player
  //  if the center deck's TopCard is a Jack
  //Otherwise, the player who slapped the deck will give a penalty card
  //  the the previous player
  private void _OnSlap(SJPlayer player)
  {
    if (deck.CurSize != 0)
    {
      if ((deck.TopCard.CardInfo & Card.RANK) == 11)
      {
        Deck.Deal(deck, [player.iDeck]);
        Deck.Shuffle(player.iDeck);
        Deck.ResetPosition(player.iDeck);
        Deck.ResetRotation(player.iDeck);
      }
      else
      {
        if (player.iDeck.CurSize != 0)
        {
          Card penalty = Deck.RemoveCard(player.iDeck, player.iDeck.TopCard);
          Deck.AddCard(PrevPlayer.iDeck, penalty);
          Deck.Shuffle(PrevPlayer.iDeck);
          Deck.ResetPosition(PrevPlayer.iDeck);
        }
      }
    }
  }
}
