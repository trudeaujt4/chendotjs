﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A consumer abstraction.
    /// </summary>
    public interface IConsumer : IStateChanged<ConsumerState>, IDisposable
    {
        /// <summary>
        /// Acknowledge the consumption of a single message.
        /// </summary>
        Task Acknowledge(Message message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of a single message using the MessageId.
        /// </summary>
        Task Acknowledge(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of all the messages in the topic up to and including the provided message.
        /// </summary>
        Task AcknowledgeCumulative(Message message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of all the messages in the topic up to and including the provided MessageId.
        /// </summary>
        Task AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the MessageId of the last message on the topic.
        /// </summary>
        Task<MessageId> GetLastMessageId(CancellationToken cancellationToken = default);

        /// <summary>
        /// Receives a single message.
        /// </summary>
        Task<Message> Receive(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reset the subscription associated with this consumer to a specific MessageId.
        /// </summary>
        Task Seek(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unsubscribe the consumer.
        /// </summary>
        Task Unsubscribe(CancellationToken cancellationToken = default);
    }
}
