/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar.Internal;

using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;

public sealed class ProducerProcess : Process
{
    private readonly IStateManager<ProducerState> _stateManager;
    private readonly IContainsChannel _subProducer;

    public ProducerProcess(
        Guid correlationId,
        IStateManager<ProducerState> stateManager,
        IContainsChannel producer) : base(correlationId)
    {
        _stateManager = stateManager;
        _subProducer = producer;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync().ConfigureAwait(false);
        _stateManager.SetState(ProducerState.Closed);
    }

    protected override void CalculateState()
    {
        if (_stateManager.IsFinalState())
            return;

        if (ExecutorState == ExecutorState.Faulted)
        {
            var newState = Exception is ProducerFencedException ? ProducerState.Fenced : ProducerState.Faulted;
            var formerState = _stateManager.SetState(newState);
            if (formerState != ProducerState.Faulted && formerState != ProducerState.Fenced)
                ActionQueue.Enqueue(async _ => await _subProducer.ChannelFaulted(Exception!).ConfigureAwait(false));
            return;
        }

        switch (ChannelState)
        {
            case ChannelState.ClosedByServer:
            case ChannelState.Disconnected:
                _stateManager.SetState(ProducerState.Disconnected);
                ActionQueue.Enqueue(async x =>
                {
                    await _subProducer.CloseChannel(x).ConfigureAwait(false);
                    await _subProducer.EstablishNewChannel(x).ConfigureAwait(false);
                });
                return;
            case ChannelState.Connected:
                ActionQueue.Enqueue(x =>
                {
                    _stateManager.SetState(ProducerState.Connected);
                    return Task.CompletedTask;
                });
                return;
            case ChannelState.WaitingForExclusive:
                _stateManager.SetState(ProducerState.WaitingForExclusive);
                return;
        }
    }
}
