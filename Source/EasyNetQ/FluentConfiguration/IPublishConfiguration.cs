using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyNetQ.FluentConfiguration
{
    public class PublishConfiguration : IPublishConfiguration
    {
        public int? Expires { get; private set; }

        public int Priority { get; set; }

        public List<string> Topics { get; set; }

        public PublishConfiguration()
        {
            Topics = new List<string>();
            Priority = 0;
        }

        IPublishConfiguration IPublishConfiguration.WithTopic(string topic)
        {
            Topics.Add(topic);
            return this;
        }

        IPublishConfiguration IPublishConfiguration.WithPriority(int priority)
        {
            Priority = priority;
            return this;
        }

        IPublishConfiguration IPublishConfiguration.WithExpires(int expires)
        {
            Expires = expires;
            return this;
        }
    }

    public interface IPublishConfiguration
    {
        IPublishConfiguration WithTopic(string topic);

        IPublishConfiguration WithPriority(int priority);
        IPublishConfiguration WithExpires(int expires);


    }

}
