using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class PackCompent : MonoBehaviour
{
    public static PackCompent Intance;

    public void Awake()
    {
        Intance = this;
    }



    public void ShowPack(List<PackModel> modelList)
    {
        while (this.transform.childCount != 0)
        {
            GameObject.DestroyImmediate(this.transform.GetChild(0).gameObject);
        }

        foreach (var item in modelList)
        {
            GameObject obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("PackItem"));
            obj.transform.parent = this.transform;
            obj.transform.localScale = Vector3.one;
            PackItem packItem = obj.GetComponent<PackItem>();
            packItem.Model = item;
        }
    }
}
