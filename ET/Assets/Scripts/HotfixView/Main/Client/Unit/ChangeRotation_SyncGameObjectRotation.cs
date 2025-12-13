using UnityEngine;

namespace ET.Client
{
    [Publish(SceneType.Current)]
    public class ChangeRotation_SyncGameObjectRotation : APublishHandler<ChangeRotation>
    {
        protected override async ETTask Run(Scene scene, ChangeRotation args)
        {
            Unit unit = args.Unit;
            GameObjectComponent gameObjectComponent = unit.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
            {
                return;
            }
            Transform transform = gameObjectComponent.GameObject.transform;
            transform.rotation = unit.Rotation;
            await ETTask.CompletedTask;
        }
    }
}
