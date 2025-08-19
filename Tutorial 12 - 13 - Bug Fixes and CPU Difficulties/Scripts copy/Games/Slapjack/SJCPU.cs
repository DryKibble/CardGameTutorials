//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using Godot;
using System;

public partial class SJCPU : SJPlayer
{
  [Export]
  public Timer DealTimer { get; private set; }
  [Export]
  public Timer SlapTimer { get; private set; }
  public static RandomNumberGenerator RNG { get; private set; } = new();
  public SJBehavior Behavior { private get; set; }
  private bool canRNG = true;
  public override void _PhysicsProcess(double delta)
  {
    switch (CurState)
    {
      case PlayerState.Idle:
        if (oDeck.CurSize != 0)
        {
          if ((oDeck.TopCard.CardInfo & Card.RANK) == 11 && !IsOut)
          {
            if (SlapTimer.IsStopped())
            {
              RNG.Randomize();
              SlapTimer.Start(RNG.RandfRange(Behavior.SlapTimes[0].X, Behavior.SlapTimes[0].Y));
            }
          }
          else if ((oDeck.TopCard.CardInfo & Card.RANK) != 11 && !IsOut && canRNG)
          {
            RNG.Randomize();
            int chance = RNG.RandiRange(0, 99);
            if (chance > Behavior.MistakeThreshold)
            {
              if (SlapTimer.IsStopped())
              {
                RNG.Randomize();
                SlapTimer.Start(RNG.RandfRange(Behavior.SlapTimes[0].X, Behavior.SlapTimes[0].Y));
              }
            }
            canRNG = false;
          }
        }

        if (IsMyTurn)
        {
          if (iDeck.CurSize != 0 && !IsOut)
          {
            if (DealTimer.IsStopped() && SlapTimer.IsStopped())
            {
              if (CanDeal)
              {
                RNG.Randomize();
                DealTimer.Start(RNG.RandfRange(Behavior.DealTimes.X, Behavior.DealTimes.Y));
              }
            }
          }
          else
          {
            GD.Print("CPU Skipped");
            EmitSignal(SJPlayer.SignalName.Skip, this);
          }
        }
        break;
    }
  }

  //Emits the Deal signal
  private void _OnDealTimeout()
  {
    EmitSignal(SJPlayer.SignalName.Deal, this);
    canRNG = true;
  }

  //Emits the Slap signal
  private void _OnSlapTimeout()
  {
    EmitSignal(SJPlayer.SignalName.Slap, this);
  }

  private void _StopSlapTimer()
  {
    SlapTimer.Stop();
  }
}
