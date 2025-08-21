//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using System;
using Godot;

public partial class ResponsiveVBoxContainer : VBoxContainer
{
  [Export]
  public ResponsiveResource Responsive { get; private set; }
  public override void _Ready()
  {
    _UpdateSeparation();
    GetViewport().SizeChanged += _UpdateSeparation;
  }

  private void _UpdateSeparation()
  {
    Vector2 size = GetViewportRect().Size;

    float separation = Mathf.Clamp(size.X * Responsive.Ratio, Responsive.MinWidth, Responsive.MaxWidth);

    AddThemeConstantOverride("separation", (int)separation);
  }
}