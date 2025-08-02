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
  [Export]
  Timer SlapTimer;
  SJPlayer[] players;
  SJPlayer CurPlayer;
  SJPlayer PrevPlayer;
  private System.Collections.Generic.Queue<SJPlayer> slapOrder = new();
  int numTurns = 0;
  private static RandomNumberGenerator rng = new();
  private enum GameState
  {
    Shuffle, DealAll, Idle, Deal,
    Slap, Penalty, Out,
  }

  private GameState curState = GameState.Shuffle;
  private GameState CurState
  {
    get { return curState; }
    set { curState = value; }
  }
  private byte numSkipped = 0;
  private bool started = false;
  private Shuffle Shuffle;
  private DealAll DealAll;
  private Deal Deal;

  //Sets up the game parameters
  public override void _Ready()
  {
    player.IsMyTurn = true;
    players = [player, cpu];
    CurPlayer = player;
    for (byte i = 1; i < players.Length; i++)
    {
      PlayerCursor.ShowMaterial(players[i], i);
    }
    PlayerCursor.Glow(CurPlayer);
    Shuffle = new Shuffle((byte)rng.RandiRange(2, 4), (byte)rng.RandiRange(2, 4), 3, 2, 0, deck);
    DealAll = new DealAll(6/3, 0, 3*3, deck, players);
    Deal = new Deal(3, CurPlayer.iDeck, deck, deck.GlobalRotation);
  }

  public override void _PhysicsProcess(double delta)
  {
    switch (CurState)
    {
      //Shuffles the current Deck to be shuffled inside the Shuffle field
      //If the game has not started, we transition to the DealAll State
      //Otherwise, we call the _ExitReward() method since only the Reward logic
      //  could trigger a shuffling
      case GameState.Shuffle:
        Shuffle.delta += delta * Shuffle.speed;
        if (Animation.OverhandShuffleShort(Shuffle))
        {
          Shuffle.numShuffles++;
          Shuffle.lower += (byte)(Shuffle.upper * 2);
          Shuffle.delta = 0;
          Deck.ResetPosition(Shuffle.deck);
          if (Shuffle.numShuffles == Shuffle.maxShuffles)
          {
            Shuffle.numShuffles = 0;
            Shuffle.lower = (byte)rng.RandiRange(2, 4);
            Shuffle.upper = (byte)rng.RandiRange(2, 4);
            Deck.Shuffle(deck);

            if (!started)
            {
              CurState = GameState.DealAll;
            }
            else
            {
              _ExitReward();
            }
          }
        }
        break;

      //Deals all the cards in the DealAll field until the size
      // of the deck dealing is equal to the endSize
      //If the game has not started, we transition each
      // player to their Idle State and the game transitions to the Idle State
      //Otherwise, we transition to the Shuffle State because we just
      //  rewarded a player
      case GameState.DealAll:
        if (DealAll.deck.CurSize > DealAll.endSize)
        {
          Animation.DeckDeal(DealAll, delta);
          if (DealAll.curFrame % DealAll.frameRate == 0)
          {
            DealAll.dealIdx++;
            DealAll.cardIdx--;
          }
        }
        else
        {
          DealAll.curFrame = 0;
          DealAll.dealIdx = 0;
          DealAll.cards.Clear();

          if (!started)
          {
            started = true;
            for (byte i = 0; i < players.Length; i++)
            {
              players[i].CurState = SJPlayer.PlayerState.Idle;
            }
            CurState = GameState.Idle;
          }
          else
          {
            CurState = GameState.Shuffle;
          }
        }
        break;

      //Deals a single card from one Deck to another as specified
      // by the Deal object
      //Transitions to the Idle state on completion
      case GameState.Deal:
        if (Animation.Deal(Deal))
        {
          Deal.delta = 0;
          CurState = GameState.Idle;
        }
        Deal.delta += delta * Deal.speed;
        break;

      //Checks if all the current Shockwave animations have stopped
      //If they have, it calls the _ExitSlap() method
      case GameState.Slap:
        bool stopped = false;
        byte count = (byte)slapOrder.Count; //since we're removing/adding from the Queue each iteration
                                            // we should store the initial size to avoid any potential
                                            // bugs with the loop condition
        SJPlayer temp;
        for (byte i = 0; i < count; i++)
        {
          temp = slapOrder.Dequeue();
          stopped |= temp.Anim.IsPlaying(); // stopped will be true if anyone's animation is playing
          slapOrder.Enqueue(temp);
        }

        if (!stopped)
        {
          _ExitSlap();
        }
        break;

      //Deals a Card from the player being penalized to the PrevPlayer
      //  and transitions to the Idle state
      case GameState.Penalty:
        if (Animation.Deal(Deal))
        {
          Deal.delta = 0;
          Deck.ResetPosition(Deal.receiver);
          Deck.ResetRotation(Deal.receiver);
          _ExitPenalty(Deal.dealer);
          CurState = GameState.Idle;
        }
        Deal.delta += delta * Deal.speed;
        break;
    }
  }

  //Deals a card from player to the center Deck and lets the next person in
  //  the turn ordering go
  private void _OnDeal(SJPlayer player)
  {
    if (CurState == GameState.Deal)
    {
      return;
    }

    numSkipped = 0;
    player.IsMyTurn = false;
    numTurns++;
    PrevPlayer = CurPlayer;
    CurPlayer = players[numTurns % players.Length];
    CurPlayer.IsMyTurn = true;
    rng.Randomize();


    Deal.endRot.X = 0;
    Deal.endRot.Y = rng.RandfRange(-Mathf.Pi / 4, Mathf.Pi / 4);

    if (player.GlobalPosition.Z > 0)
    {
      Deal.endRot.Y += -Mathf.Pi;
    }

    Deal.endRot.Z = 0;

    if (!CurPlayer.IsOut)
    {
      Deal.dealer = PrevPlayer.iDeck;
      PlayerCursor.Glow(CurPlayer);
      CurState = GameState.Deal;
    }
    PlayerCursor.ShowMaterial(PrevPlayer, (byte)Array.IndexOf(players, PrevPlayer));

    EmitSignal(SignalName.StopSlapTimer);
  }

  //Handles the logic for slapping
  //Ensures no duplicate players will be added to the slapOrder Queue
  //Plays the Shockwave animation for each unique
  // player emitting the Slap Signal
  private void _OnSlap(SJPlayer player)
  {
    if (player.Anim.IsPlaying())
    {
      return;
    }

    CurState = GameState.Idle;
    //If someone is dealing, we want their card to go back to their deck
    Deck.ResetPosition(CurPlayer.iDeck);
    Deck.ResetRotation(CurPlayer.iDeck);

    player.Anim.Play("Shockwave");
    slapOrder.Enqueue(player);

    if (SlapTimer.IsStopped())
    {
      SlapTimer.Start(1.5);
    }
  }

  //Checks whether or not we should transition to the Reward or Penalty logic
  private void _ExitSlap()
  {
    if (deck.CurSize != 0 && (deck.TopCard.CardInfo & Card.RANK) == 11)
    {
      CurPlayer = slapOrder.Dequeue();
      numTurns = Array.IndexOf(players, CurPlayer);
      DealAll.deck = deck;
      DealAll.players = [CurPlayer.iDeck];
      Shuffle.deck = CurPlayer.iDeck;
      CurState = GameState.DealAll;
    }
    else
    {
      SJPlayer penalty = slapOrder.Dequeue();
      numTurns = Array.IndexOf(players, penalty);
      numTurns++;
      numTurns %= players.Length;
      CurPlayer = players[numTurns];

      PlayerCursor.ShowMaterial(penalty, (byte)Array.IndexOf(players, penalty));
      PlayerCursor.Glow(CurPlayer);
      if (PrevPlayer is not null)
      {
        if (penalty.iDeck.CurSize != 0)
        {
          Deal.dealer = penalty.iDeck;
          Deal.receiver = PrevPlayer.iDeck;
          Deal.endRot = PrevPlayer.Rotation;
          CurState = GameState.Penalty;
        }
        else
        {
          penalty.IsOut = true;
          penalty.ShouldSkip = true;
          PlayerCursor.ShowMaterial(penalty, 8);
          CurState = GameState.Idle;
        }
      }
      else
      {
        _ExitPenalty(penalty.iDeck);
      }
      slapOrder.Clear();
    }
  }

  //Checks for a winner
  //If one is found, everyone is disabled from making inputs
  //Otherwise, lets the CurPlayer go and transitions back to the Idle state
  //Transitions all players with 0 cards in their Deck to their Out state
  private void _ExitReward()
  {
    byte numOut = 0;
    bool allCards = false;
    SJPlayer winner = null;
    for (byte i = 0; i < players.Length; i++)
    {
      PlayerCursor.ShowMaterial(players[i], i);
      if (players[i].iDeck.CurSize != 0)
      {
        players[i].CurState = SJPlayer.PlayerState.Idle;
        players[i].ShouldSkip = false;
        allCards |= players[i].iDeck.CurSize == Deck.MAXSIZE;
        winner = players[i];
      }
      else if (players[i].ShouldSkip)
      {
        players[i].CurState = SJPlayer.PlayerState.Out;
        PlayerCursor.ShowMaterial(players[i], 8);
        numOut++;
      }
      else
      {
        players[i].ShouldSkip = true;
      }
    }

    if (numOut == players.Length - 1 && allCards)
    {
      for (byte i = 0; i < players.Length; i++)
      {
        players[i].CurState = SJPlayer.PlayerState.Out;
      }

      GD.Print($"{winner.Name} wins!");
      CurState = GameState.Out;
    }
    else
    {
      CurPlayer.IsMyTurn = true;
      PlayerCursor.Glow(CurPlayer);
      CurState = GameState.Idle;
    }
  }

  //Transitions the player who is getting penalized to the Out state
  //  if their iDeck has no Cards
  private void _ExitPenalty(Deck penalty)
  {
    for (byte i = 0; i < players.Length; i++)
    {
      //If the player at index i is out, go to the next iteration of the loop
      if (players[i].IsOut)
      {
        continue;
      }

      //If the player has cards or wasn't penalized, put them back into Idle
      //otherwise
      if (players[i].iDeck.CurSize != 0 || players[i].iDeck != penalty)
      {
        players[i].CurState = SJPlayer.PlayerState.Idle;
        players[i].IsOut = false;
      }
      else
      {
        players[i].ShouldSkip = true;
        players[i].IsOut = true;
        players[i].CurState = SJPlayer.PlayerState.Out;
        PlayerCursor.ShowMaterial(players[i], 8);
      }
    }
    CurState = GameState.Idle;
    CurPlayer.IsMyTurn = true;
  }

  //Skips the passed in player and sets the new CurPlayer
  //Also determines if there's a draw in the game
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

  //Disables everyone from slapping over and over and transitions into the
  //  Slap state
  private void _OnSlapTimeout()
  {
    //essentially disable everyone from doing anything once the timer ends
    for (byte i = 0; i < players.Length; i++)
    {
      players[i].CurState = SJPlayer.PlayerState.Out;
      players[i].IsMyTurn = false;
    }
    EmitSignal(SignalName.StopSlapTimer);
    CurState = GameState.Slap;
  }
}
