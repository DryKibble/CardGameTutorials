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
  [Signal]
  public delegate void SkipEventHandler(SJPlayer player);
  [Export]
  public Deck oDeck;
  [Export]
  public Deck iDeck;
  [Export]
  private PlayerCursor cursor;
  public PlayerCursor Cursor
  {
    get { return cursor; }
  }
  [Export]
  private AnimationPlayer anim;
  public AnimationPlayer Anim
  {
    get { return anim; }
  }
  [Export]
  public MeshInstance3D Shockwave
  {
    get;
    private set;
  }

  private bool myTurn = false;
  public bool IsMyTurn
  {
    get { return myTurn; }
    set { myTurn = value; }
  }

  private bool skip = false;
  public bool ShouldSkip
  {
    get { return skip; }
    set { skip = value; }
  }
  private bool isOut = false;
  public bool IsOut
  {
    get { return isOut; }
    set { isOut = value; }
  }

  public bool CanDeal
  {
    get;
    set;
  } = true;

  public enum PlayerState
  {
    Out, Idle
  }

  public PlayerState CurState = PlayerState.Out;

  //Checks if the player wants to deal or slap
  //If the player has no cards, they will be skipped automatically
  public override void _PhysicsProcess(double delta)
  {
    switch (CurState)
    {
      case PlayerState.Idle:
        if (IsMyTurn)
        {
          if (iDeck.CurSize == 0 || IsOut)
          {
            GD.Print("Player skipped");
            EmitSignal(SignalName.Skip, this);
          }
          else
          {
            if (Input.IsActionJustPressed("Deal") && !IsOut)
            {
              if (CanDeal)
              {
                EmitSignal(SignalName.Deal, this);
              }
            }
          }
        }
        if (Input.IsActionJustPressed("Slap") && !IsOut)
        {
          EmitSignal(SignalName.Slap, this);
        }
        break;
    }
  }
}
