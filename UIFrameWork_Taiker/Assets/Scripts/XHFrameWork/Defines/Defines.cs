//
// /**************************************************************************
//
// Defines.cs
//
// Author: xiaohong  <704872627@qq.com>
//
// Unity课程讨论群:  152767675
//
// Date: 15-8-6
//
// Description:Provide  functions  to connect Oracle
//
// Copyright (c) 2015 xiaohong
//
// **************************************************************************/



using UnityEngine;
using System.Collections;

namespace XHFrameWork
{

	#region Global delegate 委托
	/// <summary>
	/// Begins the invoke.
	/// </summary>
	/// <returns>The invoke.</returns>
	/// <param name="sender">Sender.</param>
	/// <param name="newState">New state.</param>
	/// <param name="oldState">Old state.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="object">Object.</param>
	/// <summary>
	/// Invoke the specified sender, newState and oldState.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="newState">New state.</param>
	/// <param name="oldState">Old state.</param>
	/// <summary>
	/// Initializes a new instance of the <see cref="XHFrameWork.StateChangedEvent"/> delegate.
	/// </summary>
	/// <param name="object">Object.</param>
	/// <param name="method">Method.</param>
	/// <summary>
	/// Ends the invoke.
	/// </summary>
	/// <param name="result">Result.</param>
	public delegate void StateChangedEvent (object sender,EnumObjectState newState,EnumObjectState oldState);
	
	#endregion

	#region Global enum 枚举
	/// <summary>
	/// 对象当前状态 
	/// </summary>
	public enum EnumObjectState
	{
		/// <summary>
		/// The none.
		/// </summary>
		None,
		/// <summary>
		/// The initial.
		/// </summary>
		Initial,
		/// <summary>
		/// The loading.
		/// </summary>
		Loading,
		/// <summary>
		/// The ready.
		/// </summary>
		Ready,
		/// <summary>
		/// The disabled.
		/// </summary>
		Disabled,
		/// <summary>
		/// The closing.
		/// </summary>
		Closing
	}

	/// <summary>
	/// Enum user interface type.
	/// UI面板类型
	/// </summary>
	public enum EnumUIType : int
	{
		/// <summary>
		/// The none.
		/// </summary>
		None = -1,
		/// <summary>
		/// The test one.
		/// </summary>
		TestOne,
		/// <summary>
		/// The test two.
		/// </summary>
		TestTwo,
	}

	#endregion

	#region Defines static class & cosnt

	/// <summary>
	/// 路径定义。
	/// </summary>
	public static class UIPathDefines
	{
		/// <summary>
		/// UI预设。
		/// </summary>
		public const string UI_PREFAB = "Prefabs/";
		/// <summary>
		/// UI小控件预设。
		/// </summary>
		public const string UI_CONTROLS_PREFAB = "UIPrefab/Control/";
		/// <summary>
		/// ui子页面预设。
		/// </summary>
		public const string UI_SUBUI_PREFAB = "UIPrefab/SubUI/";
		/// <summary>
		/// icon路径
		/// </summary>
		public const string UI_IOCN_PATH = "UI/Icon/";

		/// <summary>
		/// Gets the type of the prefab path by.
		/// </summary>
		/// <returns>The prefab path by type.</returns>
		/// <param name="_uiType">_ui type.</param>
		public static string GetPrefabPathByType(EnumUIType _uiType)
		{
			string _path = string.Empty;
			switch (_uiType)
			{
			case EnumUIType.TestOne:
				_path = UI_PREFAB + "TestUIOne";
				break;
			case EnumUIType.TestTwo:
				_path = UI_PREFAB + "TestUITwo";
				break;
			default:
				Debug.Log("Not Find EnumUIType! type: " + _uiType.ToString());
				break;
			}
			return _path;
		}

		/// <summary>
		/// Gets the type of the user interface script by.
		/// </summary>
		/// <returns>The user interface script by type.</returns>
		/// <param name="_uiType">_ui type.</param>
		public static System.Type GetUIScriptByType(EnumUIType _uiType)
		{
			System.Type _scriptType = null;
			switch (_uiType)
			{
			case EnumUIType.TestOne:
				_scriptType = typeof(TestOne);
				break;
			case EnumUIType.TestTwo:
				_scriptType = typeof(TestTwo);
				break;
			default:
				Debug.Log("Not Find EnumUIType! type: " + _uiType.ToString());
				break;
			}
			return _scriptType;
		}

	}

	#endregion


	public class Defines : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
