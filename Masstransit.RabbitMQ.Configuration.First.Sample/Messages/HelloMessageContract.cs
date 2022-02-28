using System;

namespace Masstransit.RabbitMQ.Configuration.First.Sample.Messages
{
    public class HelloMessageContract
    {
        public Guid CustomerId { get; set; }

        public string User { get; set; }
    }

    /*
     * send the following json format to rabbitMQ for testing.
{
    "messageType": [
        "urn:message:iHerb.Rewards.Infra.RabbitMQ.Sample.Messages:HelloMessageContract"
    ],
    "message": {
        "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "user": "string"
    }
}
     */
}