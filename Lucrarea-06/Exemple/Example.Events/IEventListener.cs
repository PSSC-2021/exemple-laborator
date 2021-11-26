using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Events
{
    public interface IEventListener
    { 

        Task StartAsync(string topicName, string subscriptionName, CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}
