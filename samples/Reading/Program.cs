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

using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, args) =>
{
    cts.Cancel();
    args.Cancel = true;
};

await using var client = PulsarClient
    .Builder()
    .ExceptionHandler(ExceptionHandler)
    .Build(); // Connecting to pulsar://localhost:6650

await using var reader = client.NewReader(Schema.String)
    .StartMessageId(MessageId.Earliest)
    .StateChangedHandler(StateChangedHandler)
    .Topic("persistent://public/default/mytopic")
    .Create();

Console.WriteLine("Press Ctrl+C to exit");

await ReadMessages(reader, cts.Token);

async Task ReadMessages(IReader<string> reader, CancellationToken cancellationToken)
{
    try
    {
        await foreach (var message in reader.Messages(cancellationToken))
        {
            Console.WriteLine($"Received: {message.Value()}");
        }
    }
    catch (OperationCanceledException) { }
}

void ExceptionHandler(ExceptionContext context) =>
    Console.WriteLine($"The PulsarClient got an exception: {context.Exception}");

void StateChangedHandler(ReaderStateChanged stateChanged) =>
    Console.WriteLine($"The reader for topic '{stateChanged.Reader.Topic}' changed state to '{stateChanged.ReaderState}'");
