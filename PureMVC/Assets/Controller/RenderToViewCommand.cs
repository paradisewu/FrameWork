using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class RenderToViewCommand : ICommand
{
    PackProxy packProxy = PackProxy.GetIntance();

    GoodsProxy goodProxy = GoodsProxy.GetIntance();

    public void Excute(INotifier inotifier)
    {
        List<PackModel> modelList = packProxy.GetModelList();

        for (int i = 0; i < modelList.Count; i++)
        {
            if (modelList[i].GoodId != 0)
            {
                modelList[i].good = goodProxy.GetModelById(modelList[i].GoodId);
            }
        }

        AppFacade.Intance.ExcuteToView(new INotifier("show", modelList));

        //TODO:view 与command 的解耦
        //view.ShowPack(modelList);
    }


}

