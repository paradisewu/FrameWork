using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PackView : Mediator
{
    public int TotalCount = 15;
    
   
    public static PackView Intance;


    public PackView()
    {
        packCompent = PackCompent.Intance;
    }


    PackCompent packCompent;

    public override string Name
    {
        get { return "PackView"; }
    }

    public override List<string> MsgList
    {
        get
        {
            List<string> msgList = new List<string>();
            msgList.Add("show");
            return msgList;
        }
    }


    public override void Execute(INotifier inofifier)
    {
        switch (inofifier.msg)
        {
            case "show":
                List<PackModel> packModelList = (List<PackModel>)inofifier.body;
                packCompent.ShowPack(packModelList);
                break;
            default:
                break;
        }
    }
}
