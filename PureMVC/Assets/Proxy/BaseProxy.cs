using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BaseProxy<T> where T : ModelBase, new()
{

    protected List<T> modelList;

    public BaseProxy()
    {
        modelList = new List<T>();
    }


    public bool TryGetModel(int id, out T model)
    {
        model = this.modelList.FirstOrDefault(a => a.ID == id);

        if (model == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public void Update(T model)
    {
        T tmpModel = this.GetModelById(model.ID);
        tmpModel = model;
    }



    public List<T> GetModelList()
    {
        return this.modelList;
    }

    public void AddModelToList(T model)
    {
        this.modelList.Add(model);
    }

    public void UpdateModel(T model)
    {
        T tmpModel = this.GetModelById(model.ID);
        tmpModel = model;
    }


    public int GetMaxId()
    {
        if (this.modelList.Count == 0)
        {
            return 0;
        }
        return this.modelList.Max(a => a.ID);
    }

    public T GetModelById(int id)
    {
        return modelList.FirstOrDefault(a => a.ID == id);
    }
}
