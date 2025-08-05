//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using Godot;
using System;

public partial class SJPlayer : Node3D
{
  [Signal]
  public delegate void DealEventHandler(SJPlayer player);
  [Signal]
  public delegate void SlapEventHandler(SJPlayer player);
  [Export]
  public Deck oDeck;
  [Export]
  public Deck iDeck;
  private bool myTurn = false;
  public bool IsMyTurn
  {
    get { return myTurn; }
    set { myTurn = value; }
  }

  //Checks if the player wants to deal or slap
  public override void _PhysicsProcess(double delta)
  {
    if (Input.IsActionJustPressed("Deal"))
    {
      if (IsMyTurn)
      {
        EmitSignal(SignalName.Deal, this);
      }
    }
    else if (Input.IsActionJustPressed("Slap"))
    {
      EmitSignal(SignalName.Slap, this);
    }
  }
}
