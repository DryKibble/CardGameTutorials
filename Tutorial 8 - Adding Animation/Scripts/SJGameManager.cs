//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using Godot;
using System;

public partial class SJGameManager : Node
{
  [Signal]
  public delegate void StopSlapTimerEventHandler();
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
  private byte numSkipped = 0;

  //Sets up the game parameters
  public override void _Ready()
  {
    Deck.Shuffle(deck);
    Deck.Deal(deck, [player.iDeck, cpu.iDeck]);
    player.IsMyTurn = true;
    players = [player, cpu];
    CurPlayer = player;
    for (byte i = 1; i < players.Length; i++)
    {
      PlayerCursor.ShowMaterial(players[i], i);
    }
    PlayerCursor.Glow(CurPlayer);
  }

  //Deals a card from player to the center Deck and lets the next person in
  //  the turn ordering go
  private void _OnDeal(SJPlayer player)
  {
    numSkipped = 0;
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
    if (!CurPlayer.IsOut)
    {
      PlayerCursor.Glow(CurPlayer);
    }
    PlayerCursor.ShowMaterial(PrevPlayer, (byte)Array.IndexOf(players, PrevPlayer));

    EmitSignal(SignalName.StopSlapTimer);
  }

  //Deals all cards from the center deck to the player
  //  if the center deck's TopCard is a Jack
  //Otherwise, the player who slapped the deck will give a penalty card
  //  the the previous player
  private void _OnSlap(SJPlayer player)
  {
    player.Anim.Play("Shockwave");

    // while (player.Anim.IsPlaying()) {} //creates an infinite loop because this isn't updating every frame

    //Combine the deck.CurSize != 0 condition with the top condition
    //  since we don't want to be able to spam slap the deck in the beginning of the game

    //Add logic to check for a winner in this branch
    if (deck.CurSize != 0 && (deck.TopCard.CardInfo & Card.RANK) == 11)
    {
      Deck.Deal(deck, [player.iDeck]);
      Deck.Shuffle(player.iDeck);
      Deck.ResetPosition(player.iDeck);
      Deck.ResetRotation(player.iDeck);
      //Set the CurPlayer to the player who just slapped
      CurPlayer = player;
      //numTurns tracks the index of who should go, so just set it to
      //  the index of the current player
      numTurns = Array.IndexOf(players, CurPlayer);
      //We don't know where the CurPlayer is in the array,
      //  So just have every player's cursor not glow
      //Also make sure that we set their IsMyTurn to false
      byte numOut = 0;
      bool allCards = false;
      for (byte i = 0; i < players.Length; i++)
      {
        PlayerCursor.ShowMaterial(players[i], i);
        players[i].IsMyTurn = false;

        //If allCards is true, it stays as true, if not, it equals that expression which may or may not be true
        allCards = allCards ? allCards : players[i].iDeck.CurSize == Deck.MAXSIZE;

        if (players[i].IsOut)
        {
          numOut++;
        }

        if (players[i].iDeck.CurSize == 0)
        {
          if (players[i].ShouldSkip)
          {
            players[i].IsOut = true;
            PlayerCursor.ShowMaterial(players[i], 8);
            numOut++;
          }
          else
          {
            players[i].ShouldSkip = true;
          }
        }
      }

      CurPlayer.IsMyTurn = true;

      if (numOut == players.Length - 1 && allCards)
      {
        GD.Print("Found a winner");
        for (byte i = 0; i < players.Length; i++)
        {
          players[i].IsMyTurn = false;
          players[i].ShouldSkip = true;
          players[i].IsOut = true;
        }

        if (player.iDeck.CurSize == Deck.MAXSIZE)
        {
          GD.Print("You win");
        }
        else
        {
          GD.Print("You lose");
        }
      }
      //Now have the current player's cursor glow
      PlayerCursor.Glow(CurPlayer);
    }
    else
    {
      if (player.iDeck.CurSize != 0)
      {
        if (PrevPlayer is not null)
        {
          Card penalty = Deck.RemoveCard(player.iDeck, player.iDeck.TopCard);
          Deck.AddCard(PrevPlayer.iDeck, penalty);
          Deck.Shuffle(PrevPlayer.iDeck);
          Deck.ResetPosition(PrevPlayer.iDeck);
        }

        //Get the index of the player who just wrongly slapped
        numTurns = Array.IndexOf(players, player);
        //Set the index to be the player after that player
        numTurns++;
        //Reset numTurns to be within the array bounds just in case
        numTurns %= players.Length;
        //Set the correct CurPlayer
        CurPlayer = players[numTurns];
        //We don't know where the CurPlayer is in the array,
        //  So just have every player's cursor not glow
        //Also make sure that we set their IsMyTurn to false
        for (byte i = 0; i < players.Length; i++)
        {
          PlayerCursor.ShowMaterial(players[i], i);
          players[i].IsMyTurn = false;
        }
        CurPlayer.IsMyTurn = true;
        //Now have the current player's cursor glow
        PlayerCursor.Glow(CurPlayer);
      }
      else
      {
        player.ShouldSkip = true;
        player.IsOut = true;
      }
    }
    EmitSignal(SignalName.StopSlapTimer);
  }

  private void _OnSkip(SJPlayer player)
  {
    numSkipped++;
    PlayerCursor.ShowMaterial(player, (byte)Array.IndexOf(players, player));
    player.IsMyTurn = false;
    numTurns++;
    CurPlayer = players[numTurns % players.Length];
    CurPlayer.IsMyTurn = true;
    PlayerCursor.Glow(CurPlayer);
    player.ShouldSkip = true;
    if (numSkipped == players.Length && deck.TopCard is not null && (deck.TopCard.CardInfo & Card.RANK) != 11)
    {
      GD.Print("Draw");
      //now no one can go
      for (byte i = 0; i < players.Length; i++)
      {
        players[i].IsMyTurn = false;
        players[i].ShouldSkip = true;
        players[i].IsOut = true;
      }
    }
  }
}
