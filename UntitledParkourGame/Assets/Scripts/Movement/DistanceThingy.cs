using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

public class DistanceThingy : NetworkBehaviour
{
    // Start is called before the first frame update
    TextMeshProUGUI distanceLabel;
    TextMeshProUGUI distanceValue;
    PlayerMovement pm;
    PlayerMovement.MovementState prevState;
    PlayerMovement.MovementState currentState;
    Vector3 startPos;
    float startTime;
    void Start()
    {
        if(!IsOwner) return;
        distanceLabel = GameObject.Find("Distance Label").GetComponent<TextMeshProUGUI>();
        distanceValue = GameObject.Find("Distance Value").GetComponent<TextMeshProUGUI>();
        distanceLabel.gameObject.SetActive(false);
        distanceValue.gameObject.SetActive(false);
        pm = GetComponent<PlayerMovement>();
        prevState = pm.state;
        currentState = pm.state;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        currentState= pm.state;
        if( prevState != currentState )
        {
            startPos= transform.position;
            if (currentState == PlayerMovement.MovementState.wallrunning || currentState == PlayerMovement.MovementState.sliding || currentState == PlayerMovement.MovementState.climbing || currentState == PlayerMovement.MovementState.air)
            {
                
                
                Color c = distanceLabel.color;
                c.a = 1;
                distanceLabel.color = c;
                Color c1 = distanceValue.color;
                c1.a = 1;
                distanceValue.color = c1;
                StopAllCoroutines();
                if (currentState == PlayerMovement.MovementState.air) {
                    startTime = Time.time;
                }
                else
                {
                    distanceLabel.SetText(StateHandler(currentState));
                    distanceLabel.gameObject.SetActive(true);
                    distanceValue.gameObject.SetActive(true);
                    startPos = transform.position;
                }
            } else
            {
                //StopAllCoroutines();
                StartCoroutine(FadeOutDistanceLabel(0.25f));
            }   
        }

        if (currentState == PlayerMovement.MovementState.wallrunning || currentState == PlayerMovement.MovementState.sliding || currentState == PlayerMovement.MovementState.climbing)
        { 
            int dist = (int) Mathf.Abs((transform.position - startPos).magnitude);
            distanceValue.SetText(dist.ToString() + "m");
        } else if(currentState == PlayerMovement.MovementState.air)
        {
            float t = (Time.time - startTime);
            if(t > 0.82)
            {
                distanceLabel.SetText(StateHandler(currentState));
                distanceLabel.gameObject.SetActive(true);
                distanceValue.gameObject.SetActive(true);
                distanceValue.SetText(t.ToString("F2") + "s");
            } else
            {
                distanceValue.SetText("");
            }
        }
        prevState = currentState;
    }

    IEnumerator FadeOutDistanceLabel(float time)
    {
        float t = 0.0f;
        TextMeshProUGUI distLabel = distanceLabel;
        TextMeshProUGUI distValue = distanceValue;
        Color lColor = distLabel.color;
        Color vColor = distValue.color;
        while (t < time)
        {
            t += Time.deltaTime;
            float currIAlpha = Mathf.Lerp(1, 0, t / time);
            float currTAlpha = Mathf.Lerp(1, 0, t / time);
            lColor.a = currIAlpha;
            vColor.a = currTAlpha;
            distLabel.color = vColor;
            distValue.color = lColor;
            yield return null;
        }
        lColor.a = 1;
        vColor.a = 1;
        distLabel.color = vColor;
        distValue.color = lColor;
        distLabel.gameObject.SetActive(false);
        distValue.gameObject.SetActive(false);

    }

    string StateHandler(PlayerMovement.MovementState state)
    {
        if (state == PlayerMovement.MovementState.freeze)
        {
            return "Freeze";
        }
        else if (state == PlayerMovement.MovementState.unlimited) { return "Unlimited"; }
        else if (state == PlayerMovement.MovementState.running) { return "Running"; }
        else if (state == PlayerMovement.MovementState.sprinting) { return "Sprinting"; }
        else if (state == PlayerMovement.MovementState.sliding) { return "Sliding"; }
        else if (state == PlayerMovement.MovementState.wallrunning) { return "Wall Running"; }
        else if (state == PlayerMovement.MovementState.climbing) { return "Climbing"; }
        else if (state == PlayerMovement.MovementState.boosting) { return "Boosting"; }
        else if (state == PlayerMovement.MovementState.wallGrabbing) { return "Wall Grabbing"; }
        else if (state == PlayerMovement.MovementState.air) { return "Air"; }
        else{ return null; }

    }

   
}
