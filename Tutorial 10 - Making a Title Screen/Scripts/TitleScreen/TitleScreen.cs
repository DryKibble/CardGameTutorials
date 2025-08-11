//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using Godot;
using System;
public partial class TitleScreen : Control
{
  [Export]
  MarginContainer GameMenu;
  [Export]
  MarginContainer GameInfo;
  [Export]
  RichTextLabel Rules;
  private void _OnGameSelect()
  {
    GameMenu.Visible = !GameMenu.Visible;
    GameInfo.Visible = !GameInfo.Visible;

    System.IO.StreamReader reader = new($"./GameRules/{Globals.CurGame}.txt");

    Rules.Text = reader.ReadToEnd();
  }

  private void _OnBackPress()
  {
    GameMenu.Visible = !GameMenu.Visible;
    GameInfo.Visible = !GameInfo.Visible;
    Globals.CurGame = null;
  }

  private void _OnPlayPress()
  {
    // PackedScene game = GD.Load<PackedScene>($"res://Scenes/Games/{Globals.CurGame}/{Globals.CurGame}.tscn");
    PackedScene game = GD.Load<PackedScene>($"res://Scenes/{Globals.CurGame}.tscn");
    if (game != null)
    {
      GetTree().CallDeferred("change_scene_to_packed", game);
    }
  }
}