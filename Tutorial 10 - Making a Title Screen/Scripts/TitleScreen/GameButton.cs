//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using Godot;

public partial class GameButton : Button
{
  [Signal]
  public delegate void GameSelectedEventHandler();

  private void _OnGamePressed()
  {
    Globals.CurGame = Text;
    EmitSignal(SignalName.GameSelected);
  }
}