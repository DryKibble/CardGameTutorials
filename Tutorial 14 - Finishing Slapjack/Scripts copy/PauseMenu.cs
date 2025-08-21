//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.


using Godot;
using System;

public partial class PauseMenu : Node
{
  [Export]
  CanvasLayer Menu;
  [Export]
  VBoxContainer WinScreen;
  [Export]
  VBoxContainer Rules;
  SceneTree Tree;
  Label Label;
  bool Ended = false;

  public override void _EnterTree()
  {
    Tree = GetTree();
    Label = WinScreen.GetChild<Label>(0);
    Tree.Paused = false;
  }

  public override void _Input(InputEvent @event)
  {
    if (@event.IsActionPressed("Pause") && !Ended)
    {
      Menu.Visible = !Menu.Visible;
      Tree.Paused = !Tree.Paused;
    }
  }

  private void _OnMainMenuPressed()
  {
    Tree.CallDeferred("change_scene_to_packed", Globals.MainMenu);
  }

  private void _OnRestartPressed()
  {
    Tree.CallDeferred("reload_current_scene");
    Tree.Paused = false;
    Ended = false;
  }

  private void _OnResumePressed()
  {
    Menu.Visible = !Menu.Visible;
    Tree.Paused = !Tree.Paused;
  }

  private void _OnGameDraw()
  {
    Menu.Visible = true;
    WinScreen.Visible = true;
    Rules.Visible = false;
    Label.Text = "Draw!";
    Ended = true;
  }

  private void _OnGameWin(SJPlayer player)
  {
    Menu.Visible = true;
    WinScreen.Visible = true;
    Rules.Visible = false;
    Label.Text = player.Name + " Wins!";
    Ended = true;
  }
}
