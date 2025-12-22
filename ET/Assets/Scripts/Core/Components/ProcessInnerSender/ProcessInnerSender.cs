using System;
using System.Collections.Generic;

namespace ET
{
    [EntitySystemOf(typeof(ProcessInnerSender))]
    public static partial class ProcessInnerSenderSystem
    {
        [EntitySystem]
        private static void Destroy(this ProcessInnerSender self)
        {
            Fiber fiber = self.Fiber();
            MessageQueue.Instance.RemoveQueue(fiber.FiberId);
        }

        [EntitySystem]
        private static void Awake(this ProcessInnerSender self)
        {
            Fiber fiber = self.Fiber();
            MessageQueue.Instance.AddQueue(fiber.FiberId);
        }

        [EntitySystem]
        private static void Update(this ProcessInnerSender self)
        {
            self.list.Clear();
            Fiber fiber = self.Fiber();
            MessageQueue.Instance.Fetch(fiber.FiberId, 1000, self.list);

            foreach (MessageInfo actorMessageInfo in self.list)
            {
                self.HandleMessage(fiber, actorMessageInfo);
            }
        }

        private static void HandleMessage(this ProcessInnerSender self, Fiber fiber, in MessageInfo messageInfo)
        {
            if (messageInfo.MessageObject is IResponse response)
            {
                self.HandleIActorResponse(response);
                return;
            }

            ActorId actorId = messageInfo.ActorId;
            MessageObject message = messageInfo.MessageObject;

            Entity entity = self.Fiber().Mailboxes.Get(actorId.InstanceId);
            MailBoxComponent mailBoxComponent = entity as MailBoxComponent;
            if (mailBoxComponent == null)
            {
                Log.Warning($"actor not found mailbox, from: {actorId} current: {fiber.FiberId} {message}");
                if (message is IRequest request)
                {
                    IResponse resp = MessageHelper.CreateResponse(request.GetType(), request.RpcId, ErrorCode.ERR_NotFoundActor);
                    MessageQueue.Instance.Send(self.Fiber().FiberId, new ActorId(actorId.FiberId, 0), (MessageObject)resp);
                }
                return;
            }
            mailBoxComponent.Add(actorId.FiberId, message);
        }

        private static void HandleIActorResponse(this ProcessInnerSender self, IResponse response)
        {
            if (!self.requestCallback.Remove(response.RpcId, out MessageSenderStruct actorMessageSender))
            {
                return;
            }
            Run(actorMessageSender, response);
        }
        
        private static void Run(MessageSenderStruct self, IResponse response)
        {
            if (response.Error == ErrorCode.ERR_MessageTimeout)
            {
                self.SetException(new RpcException(response.Error, $"Rpc error: request, 注意Actor消息超时，请注意查看是否死锁或者没有reply: actorId: {self.ActorId} {self.RequestType.FullName}, response: {response}"));
                return;
            }

            if (self.NeedException && ErrorCode.IsRpcNeedThrowException(response.Error))
            {
                self.SetException(new RpcException(response.Error, $"Rpc error: actorId: {self.ActorId} request: {self.RequestType.FullName}, response: {response}"));
                return;
            }

            self.SetResult(response);
        }
        
        private static int GetRpcId(this ProcessInnerSender self)
        {
            return ++self.RpcId;
        }

        public static async ETTask<IResponse> Call(
                this ProcessInnerSender self,
                ActorId actorId,
                IRequest request,
                bool needException = true
        )
        {
            int rpcId = self.GetRpcId();
            request.RpcId = rpcId;
            
            if (actorId == default)
            {
                throw new Exception($"actor id is 0: {request}");
            }
            
            Fiber fiber = self.Fiber();
            Type requestType = request.GetType();
            
            IResponse response;
            if (!MessageQueue.Instance.Send(fiber.FiberId, actorId, (MessageObject)request))  // 纤程不存在
            {
                response = MessageHelper.CreateResponse(requestType, rpcId, ErrorCode.ERR_NotFoundActor);
                return response;
            }
            
            MessageSenderStruct messageSenderStruct = new(actorId, requestType, needException);
            self.requestCallback.Add(rpcId, messageSenderStruct);
            
            async ETTask Timeout()
            {
                await fiber.Root.GetComponent<TimerComponent>().WaitAsync(ProcessInnerSender.TIMEOUT_TIME);

                if (!self.requestCallback.Remove(rpcId, out MessageSenderStruct action))
                {
                    return;
                }
                
                if (needException)
                {
                    action.SetException(new Exception($"actor sender timeout: {requestType.FullName}"));
                }
                else
                {
                    IResponse response = MessageHelper.CreateResponse(requestType, rpcId, ErrorCode.ERR_Timeout);
                    action.SetResult(response);
                }
            }
            
            Timeout().NoContext();
            
            long beginTime = TimeInfo.Instance.ServerFrameTime();

            response = await messageSenderStruct.Wait();
            
            long endTime = TimeInfo.Instance.ServerFrameTime();

            long costTime = endTime - beginTime;
            if (costTime > 200)
            {
                Log.Warning($"actor rpc time > 200: {costTime} {requestType.FullName}");
            }
            
            return response;
        }
    }
    
    [ComponentOf(typeof(Scene))]
    public class ProcessInnerSender: Entity, IAwake, IDestroy, IUpdate
    {
        public const long TIMEOUT_TIME = 40 * 1000;
        
        public int RpcId;

        public readonly Dictionary<int, MessageSenderStruct> requestCallback = new();
        
        public readonly List<MessageInfo> list = new();
    }
}