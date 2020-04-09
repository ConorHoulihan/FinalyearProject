using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
public void OnSelectChar(int charChoice)
    {
        if(PlayerInfo.PI != null)
        {
            PlayerInfo.PI.mySelectedChar = charChoice;
            PlayerPrefs.SetInt("MyChar", charChoice);
        }
    }
}
