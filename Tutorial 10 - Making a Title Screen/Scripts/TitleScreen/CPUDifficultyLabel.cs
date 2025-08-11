//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using System;
using Godot;

public partial class CPUDifficultyLabel : Label
{
  private void _OnCPUDifficultyChange(float value)
  {
    Globals.CPUDifficulty = Math.Clamp((byte)value, (byte)0, (byte)4);
    Text = GetText(Globals.CPUDifficulty);
  }

  private string GetText(byte difficulty)
  {
    return difficulty switch
    {
      1 => "CPU Difficulty: Medium",
      2 => "CPU Difficulty: Hard",
      3 => "CPU Difficulty: Master",
      4 => "CPU Difficulty: Random",
      _ => "CPU Difficulty: Easy"
    };
  }
}