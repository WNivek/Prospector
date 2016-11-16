using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CBState {
	drawpile,
	toHand,
	hand,
	toTarget,
	target,
	discard,
	to,
	idle
}

public class CardBartok : Card {

	static public float MOVE_DURATION = 0.5f;
	static public string MOVE_EASING = Easing.InOut;
	static public float CARD_HEIGHT = 3.5f;
	static public float CARD_WIDTH = 2f;

	public CBState state = CBState.drawpile;

	public List<Vector3> bezierPts;
	public List<Quaternion> bezierRots;
	public float timeStart;
	public float timeDuration;

	public int eventualSortOrder;
	public string eventualSortLayer;

	public GameObject reportFinishTo = null;

	public void MoveTo(Vector3 ePos, Quaternion eRot) {

		// initial and final positions
		bezierPts = new List<Vector3>();
		bezierPts.Add(transform.localPosition);
		bezierPts.Add(ePos);

		// initial and final rotations
		bezierRots = new List<Quaternion>();
		bezierRots.Add(transform.rotation);
		bezierRots.Add(eRot);

		if (timeStart == 0) {
			timeStart = Time.time;
		}

		timeDuration = MOVE_DURATION;

		state = CBState.to;
	}

	// Wrapper for 'default' rotation
	public void MoveTo(Vector3 ePos) {
		MoveTo(ePos, Quaternion.identity);
	}

	void Update() {
		switch (state) {
			case CBState.toHand:
			case CBState.toTarget:
			case CBState.to:
				float u = (Time.time - timeStart) / timeDuration;
				float uC = Easing.Ease(u, MOVE_EASING);

				if (u<0) { // not yet started
					transform.localPosition = bezierPts[0];
					transform.rotation = bezierRots[0];
					return;

				} else if (u >= 1) { // finished
					uC = 1; // clamp

					// update state
					if (state == CBState.toHand) state = CBState.hand;
					if (state == CBState.toTarget) state = CBState.target;
					if (state == CBState.to) state = CBState.idle;

					// move to end
					transform.localPosition = bezierPts[bezierPts.Count - 1];
					transform.rotation = bezierRots[bezierRots.Count - 1];

					timeStart = 0;

					if (reportFinishTo != null) {
						reportFinishTo.SendMessage("CBCallback", this);
						reportFinishTo = null;
					} else {
						// no-op
					}

				} else { // (0 < u < 1) therefore we're interpolating

					Vector3 pos = Utils.Bezier(uC, bezierPts);
					transform.localPosition = pos;

					Quaternion rotQ = Utils.Bezier(uC, bezierRots);
					transform.rotation = rotQ;

					if (u > 0.5f && spriteRenderers[0].sortingOrder != eventualSortOrder) {
						// Jump to the proper sort order
						SetSortOrder(eventualSortOrder);
					}
					if (u > 0.75f && spriteRenderers[0].sortingLayerName != eventualSortLayer) {
						// Jump to the proper sort layer
						SetSortingLayerName(eventualSortLayer);
					}

				}
				break;
		}
	}


}
