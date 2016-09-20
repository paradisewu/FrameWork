using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class View
{
    private Dictionary<string, Mediator> nameToMediator;
    private Dictionary<string, Mediator> msgToMediator;

    public View()
    {
        nameToMediator = new Dictionary<string, Mediator>();
        msgToMediator = new Dictionary<string, Mediator>();
    }


    public void ResiterView(Mediator mediator)
    {
        if (nameToMediator.ContainsKey(mediator.Name))
        {
            //Debug.Log("已经存在了");
        }
        else
        {
            foreach (var item in mediator.MsgList)
            {
                this.msgToMediator.Add(item, mediator);
            }
        }
    }

    //TODO：删除View

    public void Excute(INotifier notifier)
    {
        if (msgToMediator.ContainsKey(notifier.msg))
        {
            Mediator mediator = msgToMediator[notifier.msg];
            mediator.Execute(notifier);
        }
    }



}
