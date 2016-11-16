using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bartok : MonoBehaviour {
	static public Bartok S;

	public TextAsset deckXML;
	public TextAsset layoutXML;
	public Vector3 layoutCenter = Vector3.zero;

	public float handFanDegrees = 10f;
	public int numStartingCards = 7;
	public float drawTimeStagger = 0.1f;

	public bool ___________________;

	public Deck deck;
	public List<CardBartok> drawPile;
	public List<CardBartok> discardPile;

	public BartokLayout layout;
	public Transform layoutAnchor;

	public List<Player> players;
	public CardBartok targetCard;

	void Awake() {
		S = this;

	}

	void Start () {

		deck = GetComponent<Deck>();
		deck.InitDeck(deckXML.text);
		Deck.Shuffle(ref deck.cards);

		layout = GetComponent<BartokLayout>();
		layout.ReadLayout(layoutXML.text);

		drawPile = UpgradeCardsList(deck.cards);
		LayoutGame();
	}

	List<CardBartok> UpgradeCardsList(List<Card> lCD) {
		List<CardBartok> lCB = new List<CardBartok>();
		foreach (Card tCD in lCD) {
			lCB.Add(tCD as CardBartok);
		}
		return lCB;
	}

	public void ArrangeDrawPile() {
		CardBartok tCB;

		for (int i = 0; i < drawPile.Count; ++i) {
			tCB = drawPile[i];
			tCB.transform.parent = layoutAnchor;
			tCB.transform.localPosition = layout.drawPile.pos;

			tCB.faceUp = false;
			tCB.SetSortingLayerName(layout.drawPile.layerName);
			tCB.SetSortOrder(-1 * 4);
			tCB.state = CBState.drawpile;
		}
	}

	void LayoutGame() {
		if (layoutAnchor == null) {
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
		}

		ArrangeDrawPile();

		Player pl;
		players = new List<Player>();

		foreach (SlotDef tSD in layout.slotDefs) {
			pl = new Player();
			pl.handSlotDef = tSD;
			players.Add(pl);
			pl.playerNum = players.Count;
		}
		players[0].type = PlayerType.human;

		CardBartok tCB;
		// Deal starting hands to each player
		for (int i = 0; i < numStartingCards; i++) {
			for (int j = 0; j < players.Count; j++) {
				tCB = Draw(); // Draw a card
							  // Stagger the draw time a bit. Remember order of operations.
				tCB.timeStart = Time.time + drawTimeStagger * (i * 4 + j);
				// ^ By setting the timeStart before calling AddCard, we
				//  override the automatic setting of timeStart in
				//  CardBartok.MoveTo().

				// Add the card to the player's hand. The modulus (%4)
				//  results in a number from 0 to 3
				players[(j + 1) % players.Count].AddCard(tCB);
			}
		}

		// Call Bartok.DrawFirstTarget() when the hand cards have been drawn.
		Invoke("DrawFirstTarget", drawTimeStagger * (numStartingCards * 4 + 4));
	}

	public void DrawFirstTarget() {
		// Flip up the first target card from the drawPile
		CardBartok tCB = MoveToTarget(Draw());
	}

	// This makes a new card the target
	public CardBartok MoveToTarget(CardBartok tCB) {
		tCB.timeStart = 0;
		tCB.MoveTo(layout.discardPile.pos + Vector3.back);
		tCB.state = CBState.toTarget;
		tCB.faceUp = true;

		targetCard = tCB;

		return (tCB);
	}

	public CardBartok Draw() {
		CardBartok cd = drawPile[0];
		drawPile.RemoveAt(0);
		return cd;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			players[0].AddCard(Draw());
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			players[1].AddCard(Draw());
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			players[2].AddCard(Draw());
		}
		if (Input.GetKeyDown(KeyCode.Alpha4)) {
			players[3].AddCard(Draw());
		}
	}

}
