using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;

namespace ET.Client
{
    public static partial class MoveHelper
    {
        // 可以多次调用，多次调用的话会取消上一次的协程
        public static async ETTask<int> MoveToAsync(this Unit unit, float3 targetPos)
        {
            C2M_PathfindingResult msg = C2M_PathfindingResult.Create();
            msg.Position = targetPos;
            unit.Root().GetComponent<SessionComponent>().Session.Send(msg);

            ObjectWait objectWait = unit.GetComponent<ObjectWait>();
            
            // 要取消上一次的移动协程
            objectWait.Notify(new Wait_UnitStop() { Error = WaitTypeError.Cancel });
            
            // 一直等到unit发送stop
            Wait_UnitStop waitUnitStop = await objectWait.Wait<Wait_UnitStop>();
            ETCancellationToken cancellationToken = await ETTaskHelper.GetContextAsync<ETCancellationToken>();
            if (cancellationToken.IsCancel())
            {
                return WaitTypeError.Cancel;
            }
            return waitUnitStop.Error;
        }
        
        public static async ETTask MoveToAsync(this Unit unit, List<float3> path)
        {
            float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
            MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
            await moveComponent.MoveToAsync(path, speed);
        }
    }
}

namespace ET.Server
{
    public static partial class MoveHelper
    {
        public static void Find(this Unit unit, float3 start, float3 target, List<float3> result)
        {
            //去除寻路组件 直接返回结果
            result.Add(start);
            result.Add(target);
        }
        
        // 可以多次调用，多次调用的话会取消上一次的协程
        public static async ETTask FindPathMoveToAsync(this Unit unit, float3 target)
        {
            float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
            if (speed < 0.01)
            {
                unit.SendStop(2);
                return;
            }

            M2C_PathfindingResult m2CPathfindingResult = M2C_PathfindingResult.Create();
            unit.Find(unit.Position, target, m2CPathfindingResult.Points);

            if (m2CPathfindingResult.Points.Count < 2)
            {
                unit.SendStop(3);
                return;
            }
                
            // 广播寻路路径
            m2CPathfindingResult.Id = unit.Id;
            MapMessageHelper.Broadcast(unit, m2CPathfindingResult);

            MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
            
            bool ret = await moveComponent.MoveToAsync(m2CPathfindingResult.Points, speed);
            if (ret) // 如果返回false，说明被其它移动取消了，这时候不需要通知客户端stop
            {
                unit.SendStop(0);
            }
        }

        public static void Stop(this Unit unit, int error)
        {
            unit.GetComponent<MoveComponent>().Stop(error == 0);
            unit.SendStop(error);
        }

        // error: 0表示协程走完正常停止
        public static void SendStop(this Unit unit, int error)
        {
            M2C_Stop m2CStop = M2C_Stop.Create();
            m2CStop.Error = error;
            m2CStop.Id = unit.Id;
            m2CStop.Position = unit.Position;
            m2CStop.Rotation = unit.Rotation;
            
            MapMessageHelper.Broadcast(unit, m2CStop);
        }
    }
}