using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo PI;
    public int mySelectedChar;
    public GameObject[] allChars;

    private void OnEnable()
    {
        if(PlayerInfo.PI == null)
        {
            PlayerInfo.PI = this;
        }
        else
        {
            if(PlayerInfo.PI != this)
            {
                Destroy(PlayerInfo.PI.gameObject);
                PlayerInfo.PI = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("MyChar"))
        {
            mySelectedChar = PlayerPrefs.GetInt("MyChar");
        }
        else
        {
            mySelectedChar = 0;
            PlayerPrefs.SetInt("MyChar", mySelectedChar);
        }
    }
}
