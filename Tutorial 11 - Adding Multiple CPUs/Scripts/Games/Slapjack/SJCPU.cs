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
  Timer DealTimer;
  [Export]
  Timer SlapTimer;
  public byte Difficulty;

  protected (float, float)[] slapTimes = [(0.5f, 1f)];
  protected static RandomNumberGenerator rng = new();

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
              rng.Randomize();
              (float lower, float upper) = slapTimes[0];
              SlapTimer.Start(rng.RandfRange(lower, upper));
            }
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
                DealTimer.Start(0.5f);
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
