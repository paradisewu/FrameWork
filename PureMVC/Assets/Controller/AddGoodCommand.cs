using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class AddGoodCommand : ICommand
{
    PackProxy packProxy = PackProxy.GetIntance();

    GoodsProxy goodProxy = GoodsProxy.GetIntance();


    public void Excute(INotifier inotifier)
    {
        PackModel model = null;

        //1.判断物体是不是存在
        int id = 1;

        if (!int.TryParse(inotifier.body.ToString(), out id))
        {
            return;
        }

        if (packProxy.TryGetGoodModel(id, out  model))
        {
            model.Count++;
            packProxy.Update(model);
        }
        else if (packProxy.IsFull())   //2.判段背包是不是已经满了
        {
            return;
        }
        else //3.添加
        {
            model = packProxy.GetEmptyModel();
            model.GoodId = id;
        }


        //renderToView.Excute();

        AppFacade.Intance.ExcuteToController(new INotifier("RenderToViewCommand"));
    }


}

