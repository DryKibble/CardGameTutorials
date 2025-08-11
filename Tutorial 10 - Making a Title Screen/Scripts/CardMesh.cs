//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using Godot;
using System;

public partial class CardMesh : MeshInstance3D
{
  //Sets the CardMesh's parent's (the Card) material to match
  //  its Value and Type
  public static void ShowMaterial(Card card)
  {
    card.Mesh.SetSurfaceOverrideMaterial(0, GD.Load<Material>($"res://Art Assets/Textures/Cards/{card.CardInfo}.tres"));
  }

  //Resets the CardMesh's parent's (the Card) material to its theme's blank
  public static void BlankMaterial(Card card)
  {
    card.Mesh.SetSurfaceOverrideMaterial(0, GD.Load<Material>("res://Art Assets/Textures/Cards/0.tres"));
  }
}
