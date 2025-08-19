//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using Godot;

[GlobalClass]
public partial class SJBehavior : Resource
{
  [Export] public Vector2 DealTimes { get; private set; }
  [Export] public Vector2[] SlapTimes { get; private set; } = new Vector2[2];
  [Export] public byte MistakeThreshold { get; private set; }
}
