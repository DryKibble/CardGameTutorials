//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using System;
using Godot;

public static class Globals
{
  public static string CurGame;
  public static byte NumCPUs = 1;
  public static byte CPUDifficulty = 0;
  public static readonly PackedScene MainMenu = GD.Load<PackedScene>($"res://Scenes/TitleScreen.tscn");
}