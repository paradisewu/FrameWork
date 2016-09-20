//
// /**************************************************************************
//
// ResManager.cs
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


using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace XHFrameWork
{
	public class AssetInfo
	{
		private UnityEngine.Object _Object;
		public Type AssetType { get; set; }
		public string Path { get; set; }
		public int RefCount { get; set; }
		public bool IsLoaded 
		{
			get 
			{
				return null != _Object;
			}
		}

		public UnityEngine.Object AssetObject
		{
			get
			{
				if (null == _Object)
				{
					_ResourcesLoad();
				}
				return _Object;
			}
		}

		public IEnumerator GetCoroutineObject(Action<UnityEngine.Object> _loaded)
		{
			while (true)
			{
				if (null == _Object)
				{
					yield return null;
					_ResourcesLoad();
					yield return null;
				}
				else
				{
					if (null != _loaded)
						_loaded(_Object);
				}
				yield break;
			}

		}

		private void _ResourcesLoad()
		{
			try {
				_Object = Resources.Load(Path);
				if (null == _Object)
					Debug.Log("Resources Load Failure! Path:" + Path);
			}
			catch(Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}

		public IEnumerator GetAsyncObject(Action<UnityEngine.Object> _loaded)
		{
			return GetAsyncObject(_loaded, null);
		}

		public IEnumerator GetAsyncObject(Action<UnityEngine.Object> _loaded, Action<float> _progress)
		{
			// have Object
			if (null != _Object)
			{
				_loaded(_Object);
				yield break;
			}

			// Object null. Not Load Resources
		    ResourceRequest _resRequest = Resources.LoadAsync(Path);

			// 
			while (_resRequest.progress < 0.9)
			{
				if (null != _progress)
					_progress(_resRequest.progress);
				yield return null;
			}

			// 
			while (!_resRequest.isDone)
			{
				if (null != _progress)
					_progress(_resRequest.progress);
				yield return null;
			}

			// ???
			_Object = _resRequest.asset;
			if (null != _loaded)
				_loaded(_Object);

			yield return _resRequest;
		}
	}

	public class ResManager : Singleton<ResManager>
	{
		private Dictionary<string, AssetInfo> dicAssetInfo = null;

		public override void Init ()
		{

		}

		public UnityEngine.Object LoadInstance(string _path)
		{
			UnityEngine.Object _retObj = null;
			UnityEngine.Object _obj = Load(_path);
			if (null != _obj)
			{
				_retObj = MonoBehaviour.Instantiate(_obj);
				if (null != _retObj)
				{
					return _retObj;
				}
				else
				{
					Debug.LogError("Error: null Instantiate _retObj.");
				}
			}
			else
			{
				Debug.LogError("Error: null Resources Load return _obj.");
			}
			return null;
		}

		public void LoadInstance(string _path, Action<UnityEngine.Object> _loaded)
		{
			LoadInstance(_path, _loaded, null);
		}

		public void LoadInstance(string _path, Action<UnityEngine.Object> _loaded, Action<float> _progress)
		{
			Load(_path, (_obj) => {
				UnityEngine.Object _retObj = null;
				if (null != _obj)
				{
					_retObj = MonoBehaviour.Instantiate(_obj);
					if (null != _retObj)
					{
						if(null != _loaded)
							_loaded(_retObj);
						else
							Debug.LogError("Error: null _loaded.");
					}
					else
					{
						Debug.LogError("Error: null Instantiate _retObj.");
					}
				}
				else
				{
					Debug.LogError("Error: null Resources Load return _obj.");
				}
			}, _progress);
		}

		public UnityEngine.Object Load(string _path)
		{
			if (string.IsNullOrEmpty(_path))
			{
				Debug.LogError("Error: null _path name.");
				return null;
			}

			// Load Res ....
			UnityEngine.Object _retObj = null;

			return _retObj;
		}

		public void Load(string _path, Action<UnityEngine.Object> _loaded)
		{
			Load(_path, _loaded, null);
		}

		public void Load(string _path, Action<UnityEngine.Object> _loaded, Action<float> _progress)
		{
			if (string.IsNullOrEmpty(_path))
			{
				Debug.LogError("Error: null _path name.");
				if (null != _loaded)
					_loaded(null);
			}

			// Load Res....
			AssetInfo _assetInfo = null;
			if (!dicAssetInfo.TryGetValue(_path, out _assetInfo))
			{
				_assetInfo = new AssetInfo();
				_assetInfo.Path = _path;
				dicAssetInfo.Add(_path, _assetInfo);
			}
			_assetInfo.RefCount++;

			CoroutineController.Instance.StartCoroutine(_assetInfo.GetAsyncObject(_loaded, _progress));
		}

	}
}

