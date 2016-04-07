using System.ServiceModel;

namespace ScreamingFleas.SelfHealingWcfClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISampleWcfService
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        void ThrowException();
    }
}
