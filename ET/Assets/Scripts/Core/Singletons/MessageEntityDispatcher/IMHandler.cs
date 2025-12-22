using System;

namespace ET
{
    public interface IMHandler
    {
        ETTask Handle(Entity entity, int fiberId, MessageObject actorMessage);
        Type GetRequestType();
        Type GetResponseType();
    }
}