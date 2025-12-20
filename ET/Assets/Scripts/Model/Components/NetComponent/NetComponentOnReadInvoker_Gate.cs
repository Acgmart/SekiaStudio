using System;

namespace ET.Server
{
    [Invoke(SceneType.Gate)]
    public class NetComponentOnReadInvoker_Gate: AInvokeHandler<NetComponentOnRead>
    {
        public override void Handle(NetComponentOnRead args)
        {
            HandleAsync(args).NoContext();
        }

        private async ETTask HandleAsync(NetComponentOnRead args)
        {
            Session session = args.Session;
            object message = args.Message;
            Scene root = args.Session.Root();
            // 根据消息接口判断是不是Actor消息，不同的接口做不同的处理,比如需要转发给Chat Scene，可以做一个IChatMessage接口
            switch (message)
            {
                case ISessionMessage:
                {
                    MessageSessionDispatcher.Instance.Handle(session, message);
                    break;
                }
                case IRequest iRequest: //使用ProcessInnerSender发送Actor请求
                {
                    long unitId = session.GetComponent<SessionPlayerComponent>().Player.Id;
                    int rpcId = iRequest.RpcId; // 这里要保存客户端的rpcId
                    long instanceId = session.InstanceId;
                    var unit = root.GetComponent<UnitComponent>().GetChild<Unit>(unitId);
                    ActorId actorId = new ActorId(root.Fiber.FiberId, unit.InstanceId);
                    IResponse iResponse = await root.GetComponent<ProcessInnerSender>().Call(actorId, iRequest);
                    iResponse.RpcId = rpcId;
                    
                    if (iResponse.Error == ErrorCode.ERR_MessageTimeout)
                    {
                        throw new RpcException(iResponse.Error, $"Rpc error: request, 注意Actor消息超时，请注意查看是否死锁或者没有reply: actorId: {actorId} {iRequest}, response: {iResponse}");
                    }
                    if (ErrorCode.IsRpcNeedThrowException(iResponse.Error))
                    {
                        throw new RpcException(iResponse.Error, $"Rpc error: actorId: {actorId} {iRequest}, response: {iResponse}");
                    }
                    
                    // session可能已经断开了，所以这里需要判断
                    if (session.InstanceId == instanceId)
                    {
                        session.Send(iResponse);
                    }
                    break;
                }
                case IMessage: //使用MailBoxComponent发送Actor消息
                {
                    long unitId = session.GetComponent<SessionPlayerComponent>().Player.Id;
                    var unit = root.GetComponent<UnitComponent>().GetChild<Unit>(unitId);
                    MessageQueue.Instance.Send(root.Fiber.FiberId, new ActorId(root.Fiber.FiberId, unit.InstanceId), (MessageObject)message);
                    break;
                }
				
                default:
                {
                    throw new Exception($"not found handler: {message}");
                }
            }
        }
    }
}