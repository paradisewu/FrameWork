using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GoodsProxy : BaseProxy<GoodsModel>
{

    public GoodsProxy()
        : base()
    {
        this.AddModelToList(new GoodsModel(this.GetMaxId() + 1, "Goods0"));
        this.AddModelToList(new GoodsModel(this.GetMaxId() + 1, "goods1"));
        this.AddModelToList(new GoodsModel(this.GetMaxId() + 1, "goods2"));
    }


    private static GoodsProxy GoodProxy;

    internal static GoodsProxy GetIntance()
    {
        if (GoodProxy == null)
        {
            GoodProxy = new GoodsProxy();
        }
        return GoodProxy;
    }
}
