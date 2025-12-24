using UnityEngine;

namespace ET.Client
{
    [Publish(SceneType.Current)]
    public class ChangePosition_SyncGameObjectPos : APublishHandler<ChangePosition>
    {
        protected override async ETTask Run(Scene scene, ChangePosition args)
        {
            Unit unit = args.Unit;
            GameObjectComponent gameObjectComponent = unit.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
            {
                return;
            }

            Transform transform = gameObjectComponent.Transform;
            transform.position = unit.Position;
            await ETTask.CompletedTask;
        }
    }
}