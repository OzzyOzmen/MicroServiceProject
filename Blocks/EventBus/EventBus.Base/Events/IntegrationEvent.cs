using System;
using Newtonsoft.Json;

namespace EventBus.Base.Events
{
    // this IntegrationEvent will make transactions across RubbitMQ and / or Azure
    public class IntegrationEvent
    {
        // We added 2 properties and set them both as JsonProperty.
        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public DateTime CreateDate { get; private set; }

        // If parameters doesnt exist, then the constructor will create Id and creation date.
        public IntegrationEvent()
        {
            Id = new Guid();
            CreateDate = DateTime.Now;
            
        }

        // If the parameters are exist, then we set properties with parameters.
        [JsonConstructor]
        public IntegrationEvent(Guid id,DateTime createDate)
        {
            Id = id;
            CreateDate = createDate;

        }
    }
}

