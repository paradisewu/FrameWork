using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TinyTeam.UI;

public class GameMain : MonoBehaviour
{

    void Start()
    {
        TTUIPage.ShowPage<UITopBar>();
        TTUIPage.ShowPage<UIMainPage>();
    }

    void Update()
    {

    }

}
