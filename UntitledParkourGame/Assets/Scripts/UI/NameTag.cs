using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NameTag : MonoBehaviour
{
    [SerializeField]
    TextMeshPro frontText;
    [SerializeField]
    TextMeshPro backText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNameTagText(FixedString32Bytes text)
    {
        frontText.SetText(text.ToString());
        backText.SetText(text.ToString());
    }
}
