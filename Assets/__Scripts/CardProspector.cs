using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is an enum, which defines a type of variable that only has a few
// possible named values. The CardState variable type has one of four values:
// drawpile, tableau, target, & discard
public enum CardState {
    drawpile,
    tableau,
    target,
    discard
}

public class CardProspector : Card { // Make sure CardProspector extends Card
    // This is how you use the enum CardState
    public CardState state = CardState.drawpile;
    // The hiddenBy list stores which other cards will keep this one face down
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    // LayoutID matches this card to a Layout XML id if it's a tableau card
    public int layoutID;
    // The SlotDef class stores information pulled in from the LayoutXML <slot>
    public SlotDef slotDef;

    private bool _gold = false;

    // This allows the card to react to being clicked
    override public void OnMouseUpAsButton() {
        // Call the CardClicked method on the Prospector singleton
        Prospector.S.CardClicked(this);
        // Also call the base class (Card.cs) version of this method
        base.OnMouseUpAsButton();
    }

    public bool gold {
        get {
            return _gold;
        }
        set {
            if (_gold != value) {
                if (value) {
                    GetComponent<SpriteRenderer>().sprite = deck.cardFrontGold;
                    transform.Find("back").gameObject.GetComponent<SpriteRenderer>().sprite = deck.cardBackGold;
                } else {
                    GetComponent<SpriteRenderer>().sprite = deck.cardFront;
                    transform.Find("back").gameObject.GetComponent<SpriteRenderer>().sprite = deck.cardBack;
                }
            }
            _gold = value;
        }
    }
}