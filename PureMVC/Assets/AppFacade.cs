using UnityEngine;
using System.Collections;

public class AppFacade : MonoBehaviour
{

    public static AppFacade Intance;
    Controller controller;
    View view;

    public void Awake()
    {
        Intance = this;
        controller = new Controller();
        view = new View();
    }

    public PackItem one;
    public PackItem two;
    void Start()
    {
        this.RestierCommand("RenderToViewCommand", new RenderToViewCommand());
        this.RestierCommand("AddGoodCommand", new AddGoodCommand());
        this.RestierCommand("ChangeItemCommand", new ChangeItemCommand());
        this.ResierView(new PackView());
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //this.Excute("RenderToViewCommand");
            this.ExcuteToController(new INotifier("RenderToViewCommand"));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            //this.Excute("AddGoodCommand");
            this.ExcuteToController(new INotifier("AddGoodCommand", 1));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.ExcuteToController(new INotifier("AddGoodCommand", 2));
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            this.ExcuteToController(new INotifier("ChangeItemCommand", one, two));
        }
    }

    public void ResierView(Mediator mediator)
    {
        if (mediator != null)
        {
            view.ResiterView(mediator);
        }
    }

    public void ExcuteToView(INotifier notifier)
    {
        if (notifier != null)
        {
            view.Excute(notifier);
        }
    }

    public void RestierCommand(string msg, ICommand command)
    {
        this.controller.ResiterCommand(msg, command);
    }

    public void ExcuteToController(INotifier inotifier)
    {
        this.controller.ExcuteCommand(inotifier);
    }

}
