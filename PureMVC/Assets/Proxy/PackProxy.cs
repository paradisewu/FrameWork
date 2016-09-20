using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PackProxy : BaseProxy<PackModel>
{

    public PackProxy()
        : base()
    {
        for (int i = 0; i < 15; i++)
        {
            this.AddModelToList(new PackModel(i));
        }
    }


    public PackModel GetEmptyModel()
    {
        return this.modelList.FirstOrDefault(a => a.GoodId == 0);
    }






    internal bool IsFull()
    {
        int count = this.modelList.Count(a => a.GoodId == 0);
        if (count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal bool TryGetGoodModel(int id, out PackModel model)
    {
        model = null;
        model = this.modelList.FirstOrDefault(a => a.GoodId == id);
        if (model == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private static PackProxy packProxy;

    internal static PackProxy GetIntance()
    {
        if (packProxy == null)
        {
            packProxy = new PackProxy();
        }
        return packProxy;
    }
}
