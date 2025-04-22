using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogic
{
    private GameManager gameManager;

    public GameLogic(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public bool IsValidMove(Player player, Tile tile)
    {
        if (!player.IsMyTurn)
            return false;

        return player.Hand.Contains(tile);
    }

    public bool IsWinningHand(List<Tile> hand)
    {
        if (hand.Count != 14)
            return false;

        var sortedHand = hand.OrderBy(t => t.Color).ThenBy(t => t.Number).ToList();
        return CheckForSeries(sortedHand) || CheckForGroups(sortedHand);
    }

    private bool CheckForSeries(List<Tile> hand)
    {
        for (int i = 0; i < hand.Count - 2; i++)
        {
            if (hand[i].Color == hand[i + 1].Color && 
                hand[i].Color == hand[i + 2].Color &&
                (int)hand[i].Number + 1 == (int)hand[i + 1].Number &&
                (int)hand[i + 1].Number + 1 == (int)hand[i + 2].Number)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckForGroups(List<Tile> hand)
    {
        for (int i = 0; i < hand.Count - 2; i++)
        {
            if (hand[i].Number == hand[i + 1].Number && 
                hand[i].Number == hand[i + 2].Number &&
                hand[i].Color != hand[i + 1].Color &&
                hand[i].Color != hand[i + 2].Color &&
                hand[i + 1].Color != hand[i + 2].Color)
            {
                return true;
            }
        }
        return false;
    }

    public int CalculateScore(List<Tile> hand)
    {
        int score = 0;
        var sortedHand = hand.OrderBy(t => t.Color).ThenBy(t => t.Number).ToList();

        // Seri puanı
        for (int i = 0; i < sortedHand.Count - 2; i++)
        {
            if (sortedHand[i].Color == sortedHand[i + 1].Color && 
                sortedHand[i].Color == sortedHand[i + 2].Color &&
                (int)sortedHand[i].Number + 1 == (int)sortedHand[i + 1].Number &&
                (int)sortedHand[i + 1].Number + 1 == (int)sortedHand[i + 2].Number)
            {
                score += 10;
            }
        }

        // Grup puanı
        for (int i = 0; i < sortedHand.Count - 2; i++)
        {
            if (sortedHand[i].Number == sortedHand[i + 1].Number && 
                sortedHand[i].Number == sortedHand[i + 2].Number &&
                sortedHand[i].Color != sortedHand[i + 1].Color &&
                sortedHand[i].Color != sortedHand[i + 2].Color &&
                sortedHand[i + 1].Color != sortedHand[i + 2].Color)
            {
                score += 15;
            }
        }

        return score;
    }

    public bool CanTakeDiscardedTile(Player player, Tile discardedTile)
    {
        if (!player.IsMyTurn)
            return false;

        var hand = player.Hand;
        var possibleCombinations = GetPossibleCombinations(hand, discardedTile);
        return possibleCombinations.Any();
    }

    private List<List<Tile>> GetPossibleCombinations(List<Tile> hand, Tile newTile)
    {
        var combinations = new List<List<Tile>>();
        var tempHand = new List<Tile>(hand) { newTile };
        
        // Seri kombinasyonları
        for (int i = 0; i < tempHand.Count - 2; i++)
        {
            for (int j = i + 1; j < tempHand.Count - 1; j++)
            {
                for (int k = j + 1; k < tempHand.Count; k++)
                {
                    if (IsValidSeries(tempHand[i], tempHand[j], tempHand[k]))
                    {
                        combinations.Add(new List<Tile> { tempHand[i], tempHand[j], tempHand[k] });
                    }
                }
            }
        }

        // Grup kombinasyonları
        for (int i = 0; i < tempHand.Count - 2; i++)
        {
            for (int j = i + 1; j < tempHand.Count - 1; j++)
            {
                for (int k = j + 1; k < tempHand.Count; k++)
                {
                    if (IsValidGroup(tempHand[i], tempHand[j], tempHand[k]))
                    {
                        combinations.Add(new List<Tile> { tempHand[i], tempHand[j], tempHand[k] });
                    }
                }
            }
        }

        return combinations;
    }

    private bool IsValidSeries(Tile t1, Tile t2, Tile t3)
    {
        return t1.Color == t2.Color && 
               t2.Color == t3.Color &&
               (int)t1.Number + 1 == (int)t2.Number &&
               (int)t2.Number + 1 == (int)t3.Number;
    }

    private bool IsValidGroup(Tile t1, Tile t2, Tile t3)
    {
        return t1.Number == t2.Number && 
               t2.Number == t3.Number &&
               t1.Color != t2.Color &&
               t2.Color != t3.Color &&
               t1.Color != t3.Color;
    }
} 