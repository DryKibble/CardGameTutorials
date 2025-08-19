//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using System;
using Godot;

public partial class NumCPULabel : Label
{
  private void _OnNumCPUChange(float value)
  {
    Globals.NumCPUs = Math.Clamp((byte)value, (byte)1, (byte)7);
    Text = $"Num CPUs: {Globals.NumCPUs}";
  }
}