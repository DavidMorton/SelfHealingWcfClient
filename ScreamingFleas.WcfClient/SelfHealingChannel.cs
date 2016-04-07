using System;
using System.ServiceModel;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;

namespace ScreamingFleas.WcfClient
{
    public class SelfHealingChannel<T> : RealProxy where T : class
    {
        public static T Create()
        {
            return (T)new SelfHealingChannel<T>().GetTransparentProxy();
        }
        
        private ChannelFactory<T> _channelFactory;
        private T _currentchannel;

        private SelfHealingChannel() : base(typeof(T)) { }
        
        
        private ChannelFactory<T> CurrentFactory
        {
            get
            {
                if (_channelFactory == null || _channelFactory.State != CommunicationState.Opened)
                {
                    _channelFactory = new ChannelFactory<T>("*");
                    _channelFactory.Open();
                }
                return _channelFactory;
            }
        }

        private bool CurrentChannelIsNotOpen
        {
            get
            {
                var clientChannel = (IClientChannel)_currentchannel;
                return clientChannel == null || clientChannel.State != CommunicationState.Opened;
            }
        }

        private void CreateNewChannel()
        {
            if (_currentchannel != null)
            {
                var clientChannel = (IClientChannel)_currentchannel;
                clientChannel.Dispose();
            }

            _currentchannel = CurrentFactory.CreateChannel();
        }

        private T CurrentChannel
        {
            get
            {
                if (CurrentChannelIsNotOpen)
                {
                    Debug.Write("Creating new channel.");
                    CreateNewChannel();
                }
                
                return _currentchannel;
            }
        }

        [DebuggerStepThrough]
        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage message = msg as IMethodCallMessage;

            try
            {
                var methodName = (string)msg.Properties["__MethodName"];
                var parameterTypes = (Type[])msg.Properties["__MethodSignature"];
                var method = typeof(T).GetMethod(methodName, parameterTypes);

                var parameters = (object[])msg.Properties["__Args"];

                object response = method.Invoke(CurrentChannel, parameters);

                return new ReturnMessage(response, null, 0, null, message);
            }
            catch (Exception ex)
            {
                return new ReturnMessage(ex, message);
            }
        }
    }
}
