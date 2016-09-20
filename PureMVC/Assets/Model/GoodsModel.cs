using UnityEngine;
using System.Collections;

public class GoodsModel : ModelBase
{


    public string Src { get; set; }

    public GoodsModel(int id, string src)
    {
        this.ID = id;
        this.Src = src;
    }

    public GoodsModel()
    {

    }

}
