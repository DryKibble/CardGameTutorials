//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.
using Godot;

[GlobalClass]
public partial class ResponsiveResource : Resource
{
  [Export]
  public float Ratio { get; private set; }
  [Export]
  public int MinWidth { get; private set; }
  [Export]
  public int MaxWidth { get; private set; }
}
