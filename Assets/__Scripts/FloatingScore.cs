using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// An enum to track the possible states of a FloatingScore
public enum FSState {
    idle,
    pre,
    active,
    post
}

// FloatingScore can move itself on screen following a Bézier curve
public class FloatingScore : MonoBehaviour {
    public FSState state = FSState.idle;
    [SerializeField]
    private int _score = 0; // The score field
    [SerializeField]
    private int _mult = 1; // The current multiplier

    public string scoreString="";
    public string multString="";

    // The score property also sets scoreString when set
    public int score {
        get {
            return (_score);
        }
        set {
            _score = value;
            scoreString = Utils.AddCommasToNumber(_score);
            GetComponent<Text>().text = scoreString+multString;
        }
    }

    public int mult {
        get {
            return (_mult);
        }
        set {
            _mult = value;
            if (_mult != 1) {
                multString = " x "+Utils.AddCommasToNumber(_mult);
            } else {
                multString = "";
            }
            GetComponent<Text>().text = scoreString + multString;
        }
    }

    public List<Vector3> bezierPts; // Bezier points for movement
    public List<float> fontSizes; // Bezier points for font scaling
    public float timeStart = -1f;
    public float timeDuration = 1f;
    public string easingCurve = Easing.InOut; // Uses Easing in Utils.cs

    // The GameObject that will receive the SendMessage when this is done moving
    public GameObject reportFinishTo = null;

    public void setPos(Vector3 pos) {
        Vector2 pos2d = new Vector2 ( pos.x, pos.y );
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = pos2d;
        rt.anchorMax = pos2d;
        rt.anchoredPosition3D = Vector3.zero;
    }

    // Set up the FloatingScore and movement
    // Note the use of parameter defaults for eTimeS & eTimeD
    public void Init(List<Vector3> ePts, float eTimeS = 0, float eTimeD = 1) {
        bezierPts = new List<Vector3>(ePts);

        if (ePts.Count == 1) { // If there's only one point
            // ...then just go there.
            //transform.position = ePts[0];
            setPos(ePts[0]);
            return;
        }

        // If eTimeS is the default, just start at the current time
        if (eTimeS == 0) eTimeS = Time.time;
        timeStart = eTimeS;
        timeDuration = eTimeD;

        state = FSState.pre; // Set it to the pre state, ready to start moving
    }

    public void FSCallback(FloatingScore fs) {
        // When this callback is called by SendMessage,
        //  add the score from the calling FloatingScore
        score = score + fs.score;
        mult = mult * fs.mult;
    }

    // Update is called once per frame
    void Update() {
        // If this is not moving, just return
        if (state == FSState.idle) return;

        // Get u from the current time and duration
        // u ranges from 0 to 1 (usually)
        float u = (Time.time - timeStart) / timeDuration;
        // Use Easing class from Utils to curve the u value
        float uC = Easing.Ease(u, easingCurve);
        if (u < 0) { // If u<0, then we shouldn't move yet.
            state = FSState.pre;
            // Move to the initial point
            //transform.position = bezierPts[0];
            setPos(bezierPts[0]);
        } else {
            if (u >= 1) { // If u>=1, we're done moving
                uC = 1; // Set uC=1 so we don't overshoot
                state = FSState.post;
                if (reportFinishTo != null) { //If there's a callback GameObject
                    // Use SendMessage to call the FSCallback method
                    //  with this as the parameter.
                    reportFinishTo.SendMessage("FSCallback", this);
                    // Now that the message has been sent,
                    //  Destroy this gameObject
                    Destroy(gameObject);
                } else { // If there is nothing to callback
                    // ...then don't destroy this. Just let it stay still.
                    state = FSState.idle;
                }
            } else {
                // 0<=u<1, which means that this is active and moving
                state = FSState.active;
            }
            // Use Bezier curve to move this to the right point
            Vector3 pos = Utils.Bezier(uC, bezierPts);
            //transform.position = pos;
            setPos(pos);
            if (fontSizes != null && fontSizes.Count > 0) {
                // If fontSizes has values in it
                // ...then adjust the fontSize of this GUIText
                int size = Mathf.RoundToInt(Utils.Bezier(uC, fontSizes));
                GetComponent<Text>().fontSize = size;
            }
        }
    }
}