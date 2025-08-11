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

  //Bug: CPU will deal->slap
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
              SlapTimer.Start(0.25f);
            }
          }
        }

        if (IsMyTurn)
        {
          if (iDeck.CurSize != 0 && !IsOut)
          {
            if (DealTimer.IsStopped() && SlapTimer.IsStopped())
            {
              DealTimer.Start(0.5f);
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
