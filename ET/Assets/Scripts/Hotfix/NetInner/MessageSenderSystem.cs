using System;
using System.Collections.Generic;

namespace ET.Server
{
    [FriendOf(typeof(MessageSender))]
    public static partial class MessageSenderSystem
    {
        public static void Send(this MessageSender self, ActorId actorId, IMessage message)
        {
            Fiber fiber = self.Fiber();
            MessageQueue.Instance.Send(fiber.FiberId, actorId, (MessageObject)message);
        }

        private static int GetRpcId(this MessageSender self)
        {
            return ++self.RpcId;
        }

        public static async ETTask<IResponse> Call(
                this MessageSender self,
                ActorId actorId,
                IRequest request,
                bool needException = true
        )
        {
            if (actorId == default)
            {
                throw new Exception($"actor id is 0: {request}");
            }
            Fiber fiber = self.Fiber();

            IResponse response = await fiber.Root.GetComponent<ProcessInnerSender>().Call(actorId, request, needException: needException);

            if (response.Error == ErrorCode.ERR_MessageTimeout)
            {
                throw new RpcException(response.Error, $"Rpc error: request, 注意Actor消息超时，请注意查看是否死锁或者没有reply: actorId: {actorId} {request}, response: {response}");
            }
            if (needException && ErrorCode.IsRpcNeedThrowException(response.Error))
            {
                throw new RpcException(response.Error, $"Rpc error: actorId: {actorId} {request}, response: {response}");
            }
            return response;
        }
    }
}