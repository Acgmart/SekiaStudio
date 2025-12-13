using System;

namespace ET
{
    public class PublishAttribute : BaseAttribute
    {
        public int SceneType { get; }

        public PublishAttribute(int sceneType)
        {
            this.SceneType = sceneType;
        }
    }
}