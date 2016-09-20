
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XHFrameWork;

public class TestTwo : BaseUI
{

	private Button btn;

	#region implemented abstract members of BaseUI
	public override EnumUIType GetUIType ()
	{
		return EnumUIType.TestTwo;
	}
	#endregion

	// Use this for initialization
	void Start ()
	{
		btn = transform.Find ("Panel/Button").GetComponent<Button> ();
		btn.onClick.AddListener (OnClickBtn);
	}
	
	private void OnClickBtn ()
	{
		UIManager.Instance.OpenUICloseOthers(EnumUIType.TestOne);
//		GameObject go = Instantiate (Resources.Load<GameObject> ("Prefabs/TestUIOne"));
//		TestOne to = go.GetComponent<TestOne> ();
//		if (null == to)
//			to = go.AddComponent<TestOne> ();
//		Close ();
	}
	
	private void Close ()
	{
		Destroy (gameObject);
	}
}

