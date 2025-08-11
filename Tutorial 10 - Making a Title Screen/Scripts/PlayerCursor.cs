//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using System;
using Godot;

public partial class PlayerCursor : MeshInstance3D
{
  public static void ShowMaterial(SJPlayer player, byte mat)
  {
    player.Cursor.SetSurfaceOverrideMaterial(0, GD.Load<Material>($"res://Art Assets/Textures/PlayerCursor/Cursor{mat}.tres"));
  }

  public static void Glow(SJPlayer player)
  {
    player.Cursor.SetSurfaceOverrideMaterial(0, GD.Load<Material>($"res://Art Assets/Textures/PlayerCursor/CursorGlow.tres"));
  }
}