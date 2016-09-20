using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ChangeItemCommand : ICommand
{
    PackProxy packProxy = PackProxy.GetIntance();

    GoodsProxy goodProxy = GoodsProxy.GetIntance();


    public void Excute(INotifier inotifier)
    {
        List<PackModel> modelList = packProxy.GetModelList();

        PackItem one = (PackItem)inotifier.body;
        PackItem two = (PackItem)inotifier.body2;
        int tempx = 0, tempy = 0;
        PackModel temp;
        for (int i = 0; i < modelList.Count; i++)
        {
            if (modelList[i] == one.Model)
            {
                tempx = i;
            }
            else if (modelList[i] == two.Model)
            {
                tempy = i;
            }
        }
        if (tempx != tempy)
        {
            temp = modelList[tempx];
            modelList[tempx] = modelList[tempy];
            modelList[tempy] = temp;
        }


        for (int i = 0; i < modelList.Count; i++)
        {
            //if (modelList[i].GoodId != 0)
            //{
                modelList[i].good = goodProxy.GetModelById(modelList[i].GoodId);
            //}
        }

        AppFacade.Intance.ExcuteToView(new INotifier("show", modelList));
    }

}
