using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XHFrameWork;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
		UIManager.Instance.OpenUI(EnumUIType.TestOne);

        #region MyRegion
        //		GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/TestUIOne"));
        //		TestOne to = go.GetComponent<TestOne>();
        //		if (null == to)
        //			to = go.AddComponent<TestOne>();

        //GameController.Instance.StartCoroutine(AsyncLoadData());

        //DDOLTest.Instance.StartCoroutine_Auto(AsyncLoadData());
        #endregion
       
	}

	private IEnumerator<int> AsyncLoadData()
	{
		int i = 0;
		while(true)
		{
			Debug.Log("------> " + i);
			yield return i;
			i++;
		}
	}

}
