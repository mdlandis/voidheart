using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cantrip : MonoBehaviour
{

    public GameObject inactive;
    public GameObject active;

    public void Hover(bool hovered)
    {
        inactive.SetActive(!hovered);
        active.SetActive(hovered);
    }
}
