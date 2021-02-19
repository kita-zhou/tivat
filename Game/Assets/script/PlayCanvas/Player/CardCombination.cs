using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombination : IComparable
{
    public CardCombination()
    {
        cardstrings = new List<CardName>();
    }
    public CardCombination(string cardstr)
    {
        string[] strings = cardstr.Split('+');
        cardstrings = new List<CardName>();
        for(int i = 1; i < strings.Length; i++)
        {
            cardstrings.Add((CardName)Enum.Parse(typeof(CardName),strings[i]));
        }
    }
    public List<CardName> cardstrings;
    public override string ToString()
    {
        string strings = "#";
        
        cardstrings.ForEach(str => strings += "+" + str.ToString());
        return strings;
    }

    public override int GetHashCode()
    {
        int hash=0;
        cardstrings.ForEach(str => hash += str.GetHashCode());
        return hash;
    }

    public static implicit operator CardCombination (string cardstr)
    {
        return new CardCombination(cardstr);
    }

    public static implicit operator string (CardCombination cards)
    {
        return cards.ToString();
    }

    public static bool operator == (CardCombination cards1, CardCombination cards2)
    {
        if (cards1.cardstrings.Count != cards2.cardstrings.Count) return false;
        foreach(CardName cardstr in cards1.cardstrings)
        {
            if (cards1.cardstrings.FindAll(t => (t == cardstr)).Count != cards2.cardstrings.FindAll(t => (t == cardstr)).Count) return false;
        }
        return true;
    }
    public static bool operator !=(CardCombination cards1, CardCombination cards2)
    {
        return !(cards1==cards2);
    }

    int CompareTo(CardCombination cards)
    {
        if (this == cards) return 0;
        return ToString().CompareTo(cards.ToString());
    }
    int IComparable.CompareTo(object obj)
    {
        CardCombination cards = obj as CardCombination;
        if (this == cards) return 0;
        return ToString().CompareTo(cards.ToString());
    }

    public static bool operator >(CardCombination cards1,CardCombination cards2)
    {
        return cards1.CompareTo(cards2)>0;
    }
    public static bool operator <(CardCombination cards1, CardCombination cards2)
    {
        return cards1.CompareTo(cards2) <0;
    }
}
