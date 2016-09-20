using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PackController :MonoBehaviour
{
    public PackProxy packProxy;
    public GoodsProxy goodProxy;

    public PackView view;


    void Awake()
    {
        packProxy = PackProxy.GetIntance();
        goodProxy = GoodsProxy.GetIntance();
        RenderToView();
    }


    public void RenderToView()
    {
        List<PackModel> modelList = packProxy.GetModelList();

        for (int i = 0; i < modelList.Count; i++)
        {
            if (modelList[i].GoodId != 0)
            {
                modelList[i].good = goodProxy.GetModelById(modelList[i].GoodId);
            }
        }

        //view.ShowPack(modelList);
    }


    public void AddGoodModel(int id)
    {
        PackModel model=null;

        //1.判断物体是不是存在
        if (packProxy.TryGetGoodModel(id,out  model))
        {
            model.Count++;
            packProxy.Update(model);
            
        }
        else if(packProxy.IsFull())   //2.判段背包是不是已经满了
        {
            return;
        }
        else //3.添加
        {
            model = packProxy.GetEmptyModel();
            model.GoodId = id;
        }
        this.RenderToView();
    }



  
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

            this.AddGoodModel(1);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {

            this.AddGoodModel(2);
        }

    }
}
