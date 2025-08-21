//Copyright Krystal 2025. All rights reserved.
//For educational purposes only.
//Modification and derivatives of this code is permitted
//Commercial use and distribution of this code and any derivatives
//  is strictly prohibited.

using System;
using Godot;

public static class Animation
{

  //Overhand Shuffles a Deck of Cards on the SHORT end of the Cards
  //Inputs:
  // deck - the Deck we are to overhand shuffle
  // temp - a temporary Vector3 to track Positions
  // lower - the index of which Cards at the bottom of the Deck should be ignored
  //           a higher value means more cards will be ignored from the bottom
  // upper - the index of which Cards at the top of the Deck should be ignored
  //           a lower value means more cards will be ignored from the top
  // delta - controls the animation speed
  //Outputs: true when the animation has finished, false otherwise
  public static bool OverhandShuffleShort(Deck deck, Vector3 temp, byte lower, byte upper, float delta)
  {
    if (deck.CurSize - upper <= lower || delta > 1)
    {
      return true;
    }

    //Moves the Cards between lower and upper up and around the Deck
    for (byte i = lower; i < deck.CurSize - upper; i++)
    {
      Card card = Deck.GetCard(deck, i);
      if (card is null)
      {
        return true;
      }

      Vector3 P1 = new(0, Card.HEIGHT * i, 0);
      Vector3 P2 = new(0, Card.HEIGHT * i, Card.LENGTH * Card.SCALE * 3);
      Vector3 P3 = new(0, Card.HEIGHT * (i + 100), Card.LENGTH);
      Vector3 P4 = new(0, Card.HEIGHT * (upper + i), 0);

      card.Position = CubicInterpolate(P1, P2, P3, P4, delta);
    }

    //Moves the Cards above upper down towards the bottom of the deck
    if (Deck.GetCard(deck, (byte)(deck.CurSize - upper - 1)).Position.Z > Card.LENGTH * Card.SCALE)
    {
      for (byte j = (byte)(deck.CurSize - upper); j <= deck.CurSize; j++)
      {
        Card card = Deck.GetCard(deck, j);
        if (card is null)
        {
          return true;
        }

        temp = card.Position;
        temp.Y = Mathf.Lerp(temp.Y, Card.HEIGHT * (lower + upper + j - deck.CurSize - 1), 0.2f);
        card.Position = temp;
      }
    }
    return false;
  }

  //Calls the OverhandShuffleShort() method, but uses the Shuffle's fields as arguments
  public static bool OverhandShuffleShort(Shuffle shuffle)
  {
    return OverhandShuffleShort(shuffle.deck, shuffle.temp, shuffle.lower,
      shuffle.upper, (float)shuffle.delta);
  }

  //Plays a Dealing animation for each Card in the Deck.
  //Each Card goes towards a Player
  //This is to be used STRICTLY by Decks only as the logic handles the capability
  //  to "deal" multiple Cards at once
  //To deal a singular Card, see Deal() below
  public static void DeckDeal(
        Deck deck,
        Deck[] players,
        System.Collections.Generic.Queue<(Card, Vector3, Vector3, Vector3, float, byte)> cards,
        int curFrame,
        byte dealIdx,
        byte cardIdx,
        byte frameRate,
        float Delta,
        byte maxSize = 52
    )
  {
    if (curFrame % frameRate == 0 && cards.Count != maxSize)
    {
      byte playerIdx = (byte)(dealIdx % players.Length);
      Vector3 P1 = new(
          deck.GlobalPosition.X,
          (deck.CurSize - (curFrame / frameRate)) * Card.HEIGHT,
          deck.GlobalPosition.Z
      );
      Vector3 P2 = new(
          (players[playerIdx].GlobalPosition.X - Math.Abs(deck.GlobalPosition.X)) / 2,
          Deck.MAXSIZE * Card.HEIGHT * 2,
          (players[playerIdx].GlobalPosition.Z - Math.Abs(deck.GlobalPosition.Z)) / 2
      );

      Vector3 P3 = new(
          players[playerIdx].GlobalPosition.X,
          curFrame * Card.HEIGHT / (frameRate * players.Length),
          players[playerIdx].GlobalPosition.Z
      );

      cards.Enqueue((Deck.GetCard(deck, cardIdx), P1, P2, P3, 0, playerIdx));
    }

    Vector3 temp;
    for (byte i = 0; i < cards.Count; i++)
    {
      (Card card, Vector3 P1, Vector3 P2, Vector3 P3, float delta, byte player) = cards.Dequeue();
      if (card is not null)
      {
        card.GlobalPosition = QuadraticInterpolate(P1, P2, P3, delta);

        if (delta * 1.5 <= 1)
        {
          temp = card.GlobalRotation;
          temp = temp.Lerp(players[player].GlobalRotation, delta * 1.5f);
          temp.Z = -Mathf.Pi;
          card.GlobalRotation = temp;
        }

        if (delta > 1)
        {
          Deck.AddCard(players[player], Deck.RemoveCard(deck, card));
          // CardMesh.BlankMaterial(card);
          Deck.ResetPosition(players[player]);
          Deck.ResetRotation(players[player]);
          cards.Enqueue((null, P1, P2, P3, delta, 0));
        }
        else
        {
          delta += Delta;
          cards.Enqueue((card, P1, P2, P3, delta, player));
        }
      }
      else
      {
        cards.Enqueue((null, P1, P2, P3, delta, 0));
      }
    }
  }

  public static void DeckDeal(DealAll DealAll, double delta)
  {
    DeckDeal(
      DealAll.deck, DealAll.players,
      DealAll.cards, DealAll.curFrame++,
      DealAll.dealIdx, DealAll.cardIdx,
      DealAll.frameRate, (float)delta*DealAll.speed);
  }

  //Plays a dealing animation between a Player and a Deck
  //Moves the Player's TopCard to the Deck's position with a random rotation
  //  to make the card playing feel natural
  //Returns true when the animation finishes
  public static bool Deal(Deck dealer, Deck deck, Vector3 rot, Vector3 endRot, float delta)
  {
    Card card = dealer.TopCard;
    if (card is null)
    {
      return true;
    }

    Vector3 P1 = new(dealer.GlobalPosition.X, 4 * dealer.CurSize * Card.HEIGHT, dealer.GlobalPosition.Z);
    Vector3 P2 = new(dealer.GlobalPosition.X / 2, deck.CurSize * Card.HEIGHT * 2, dealer.GlobalPosition.Z / 2);
    Vector3 P3 = new(deck.GlobalPosition.X, deck.CurSize * Card.HEIGHT, deck.GlobalPosition.Z);

    card.GlobalPosition = QuadraticInterpolate(P1, P2, P3, delta);

    rot = card.Rotation;
    rot.X = Mathf.LerpAngle(rot.X, endRot.X, delta);
    rot.Y = Mathf.LerpAngle(rot.Y, endRot.Y, delta);
    rot.Z = Mathf.LerpAngle(rot.Z, endRot.Z, delta);
    card.Rotation = rot;

    if (delta > 1)
    {
      rot = card.GlobalRotation;
      rot.X = Mathf.Tau;
      Deck.AddCard(deck, Deck.RemoveCard(dealer, card));
      deck.TopCard.GlobalRotation = rot;
      deck.TopCard.Position = P3;
      CardMesh.ShowMaterial(card);
      return true;
    }

    return false;
  }

  //Calls the Deal method with all the Deal object's components as the arguments
  public static bool Deal(Deal deal)
  {
    return Deal(deal.dealer, deal.receiver, deal.rot, deal.endRot, (float)deal.delta);
  }

  //Returns a Vector3 cubicly interpolated along the curve defined by the
  //  four Vector3 passed in and the float delta
  private static Vector3 CubicInterpolate(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4, float delta)
  {
    delta = Math.Clamp(delta, 0, 1);
    Vector3 Q1 = P1.Lerp(P2, delta);
    Vector3 Q2 = P2.Lerp(P3, delta);
    Vector3 R1 = Q1.Lerp(Q2, delta);
    Q1 = P3.Lerp(P4, delta);
    Q2 = Q2.Lerp(Q1, delta);
    return R1.Lerp(Q2, delta);
  }

  //Returns a Vector3 quadratically interpolated along the curve defined by the
  //  four Vector3 passed in and the float delta
  public static Vector3 QuadraticInterpolate(Vector3 P1, Vector3 P2, Vector3 P3, float delta)
  {
    Vector3 Q1 = P1.Lerp(P2, delta);
    Vector3 Q2 = P2.Lerp(P3, delta);
    return Q1.Lerp(Q2, delta);
  }
}