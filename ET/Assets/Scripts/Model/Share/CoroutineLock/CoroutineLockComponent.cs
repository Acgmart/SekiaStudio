using System;
using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class CoroutineLockComponent: Entity, IAwake, IUpdate
    {
        public readonly Queue<(long, long, int)> nextFrameRun = new();
    }
}