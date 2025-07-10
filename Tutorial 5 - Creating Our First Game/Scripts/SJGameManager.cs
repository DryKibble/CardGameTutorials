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
  int numTurns = 0;

  //Sets up the game parameters
  public override void _Ready()
  {
    Deck.Shuffle(deck);
    Deck.Deal(deck, [player.iDeck, cpu.iDeck]);
    player.IsMyTurn = true;
    players = [player, cpu];
  }

  //Deals a card from player to the center Deck and lets the next person in
  //  the turn ordering go
  private void _OnDeal(SJPlayer player)
  {
    Deck.AddCard(deck, Deck.RemoveCard(player.iDeck, player.iDeck.TopCard));
    Deck.Reset(deck);
    player.IsMyTurn = false;
    numTurns++;
    players[numTurns % players.Length].IsMyTurn = true;
  }

  //Deals all cards from the center deck to the player
  private void _OnSlap(SJPlayer player)
  {
    Deck.Deal(deck, [player.iDeck]);
    Deck.Shuffle(player.iDeck);
    Deck.Reset(player.iDeck);
  }
}
