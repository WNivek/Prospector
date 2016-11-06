using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Prospector : MonoBehaviour {

	static public Prospector 	S;
	public Deck					deck;
	public TextAsset			deckXML;

    public Layout layout;
    public TextAsset layoutXML;

    void Awake(){
		S = this;
	}

	void Start() {
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
		Deck.Shuffle (ref deck.cards);

        layout = GetComponent<Layout>();   // Get the Layout
        layout.ReadLayout(layoutXML.text); // Pass LayoutXML to it
    }

}
