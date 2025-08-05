//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

// using Godot;
// using System;

// public partial class Card : CharacterBody3D
// {
//     [Export]
//     private byte cardInfo = 0b00000000;

//     public byte CardInfo
//     {
//         get { return cardInfo; }
//     }

//     //Gets the Card's Rank
//     public static readonly byte RANK = 0b00001111;
//     //Gets the Card's Suit
//     public static readonly byte SUIT = 0b00110000;

//     public static readonly string[] SUITS = ["Spades", "Diamonds", "Clubs", "Hearts"];
//     public static readonly byte SCALE = 10;
//     public static readonly float HEIGHT = 0.0002f * SCALE;
//     public static readonly float LENGTH = 0.00889f * SCALE;
//     public static readonly float WIDTH = 0.00635f * SCALE;

//     public override void _Ready()
//     {
//         GD.Print(ToString(this));
//     }

//     //Returns a string representation of a card
//     public static string ToString(Card card)
//     {
//         return $"The {GetValue((byte)(card.CardInfo & RANK))} of {SUITS[(card.CardInfo & SUIT) >> 4]}";
//     }

//     //Returns a string representation of a card's rank
//     //Face cards (Ace, Jack, Queen, King) will be returned as their title
//     //  and all other values will be returned as their number
//     private static string GetValue(byte val)
//     {
//         return val switch
//         {
//             0b0001 => "Ace",
//             0b1011 => "Jack",
//             0b1100 => "Queen",
//             0b1101 => "King",
//             _ => $"{val}",
//         };
//     }
// }
