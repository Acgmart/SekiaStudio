using System;
using System.Collections.Generic;

namespace ET.Server
{
    [ConsoleHandler(ConsoleMode.CreateRobot)]
    public class CreateRobotConsoleHandler: IConsoleHandler
    {
        public async ETTask Run(Fiber fiber, ModeContex contex, string content)
        {
            switch (content)
            {
                case ConsoleMode.CreateRobot:
                {
                    Log.Console("CreateRobot args error!");
                    break;
                }
                default:
                {
                    //通过命令行指令 指定机器人数量
                    CreateRobotArgs options = new CreateRobotArgs();

                    RobotManagerComponent robotManagerComponent =
                            fiber.Root.GetComponent<RobotManagerComponent>() ?? fiber.Root.AddComponent<RobotManagerComponent>();

                    // 创建机器人
                    TimerComponent timerComponent = fiber.Root.GetComponent<TimerComponent>();
                    for (int i = 0; i < options.Num; ++i)
                    {
                        await robotManagerComponent.NewRobot($"Robot_{i}");
                        Log.Console($"create robot {i}");
                        await timerComponent.WaitAsync(2000);
                    }
                    break;
                }
            }
            contex.Parent.RemoveComponent<ModeContex>();
            await ETTask.CompletedTask;
        }
    }
}