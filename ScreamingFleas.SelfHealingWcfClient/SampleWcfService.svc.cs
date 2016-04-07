using System;

namespace ScreamingFleas.SelfHealingWcfClient
{
    public class SampleWcfService : ISampleWcfService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public void ThrowException()
        {
            throw new InvalidOperationException();
        }
    }
}
