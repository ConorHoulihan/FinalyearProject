using Photon.Pun;
using UnityEngine;

public class AvatarSetup : MonoBehaviour
{
    public GameObject myChar;
    public int charValue;

    [PunRPC]
    void RPC_AddChar(int whichChar)
    {
        charValue = whichChar;
        myChar = Instantiate(PlayerInfo.PI.allChars[whichChar], transform.position, transform.rotation, transform);
    }
}
