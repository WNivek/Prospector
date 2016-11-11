using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bartok : MonoBehaviour {
	static public Bartok S;

	public TextAsset deckXML;
	public TextAsset layoutXML;
	public Vector3 layoutCenter = Vector3.zero;

	public float handFanDegrees = 10f;

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
