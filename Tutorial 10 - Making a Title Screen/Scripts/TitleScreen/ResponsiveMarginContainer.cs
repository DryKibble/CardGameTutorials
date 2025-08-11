//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using System;
using Godot;

public partial class ResponsiveMarginContainer : MarginContainer
{
  private readonly float marginRatio = 0.05f;
  private readonly byte minMargin = 24;
  private readonly byte maxMargin = 128;

  public override void _Ready()
  {
    _UpdateMargins();
    GetViewport().SizeChanged += _UpdateMargins;
  }

  private void _UpdateMargins()
  {
    Vector2 size = GetViewportRect().Size;

    float marginX = Mathf.Clamp(size.X * marginRatio, minMargin, maxMargin);
    float marginY = Mathf.Clamp(size.Y * marginRatio, minMargin, maxMargin);

    AddThemeConstantOverride("margin_left", (int)marginX);
    AddThemeConstantOverride("margin_right", (int)marginX);
    AddThemeConstantOverride("margin_top", (int)marginY);
    AddThemeConstantOverride("margin_bottom", (int)marginY/2);
  }
}