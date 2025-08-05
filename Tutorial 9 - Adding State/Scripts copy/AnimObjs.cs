using System;
using System.Linq;
using Godot;

public class Shuffle(byte lower, byte upper, byte maxShuffles, byte speed, double delta, Deck deck)
{
  public byte lower = lower;
  public byte upper = upper;
  public byte numShuffles = 0;
  public byte maxShuffles = maxShuffles;
  public byte speed = speed;
  public double delta = delta;
  public Deck deck = deck;
  public Vector3 temp = Vector3.Zero;
}

public class DealAll(byte frameRate, byte endSize, byte speed, Deck deck, SJPlayer[] players)
{
  public byte dealIdx = 0;
  public byte cardIdx = deck.CurSize;
  public byte frameRate = frameRate;
  public byte endSize = endSize;
  public byte speed = speed;
  public int curFrame = 0;
  public double delta = 0;
  public Deck deck = deck;
  public Deck[] players = players.Select(player => player.iDeck).ToArray();
  public System.Collections.Generic.Queue<(Card, Vector3, Vector3, Vector3, float, byte)> cards = new();
}

public class Deal(byte speed, Deck dealer, Deck receiver, Vector3 endRot)
{
  public byte speed = speed;
  public double delta = 0;
  public Deck dealer = dealer;
  public Deck receiver = receiver;
  public Vector3 rot = Vector3.Zero;
  public Vector3 endRot = endRot;
}
