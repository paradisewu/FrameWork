using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XHFrameWork;

public class TestOne : BaseUI 
{
	private Button btn;

	#region implemented abstract members of BaseUI
	public override EnumUIType GetUIType ()
	{
		return EnumUIType.TestOne;
	}
	#endregion

	// Use this for initialization
	void Start ()
	{
		btn = transform.Find("Panel/Button").GetComponent<Button>();
		btn.onClick.AddListener(OnClickBtn);
	}
	
	private void OnClickBtn()
	{
		UIManager.Instance.OpenUICloseOthers(EnumUIType.TestTwo);
//		GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/TestUITwo"));
//		TestTwo to = go.GetComponent<TestTwo>();
//		if (null == to)
//			to = go.AddComponent<TestTwo>();
//		Close();
	}

	private void Close()
	{
		Destroy(gameObject);
	}
}

