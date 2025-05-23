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

namespace DotPulsar.Abstractions;

using System.Buffers;

/// <summary>
/// A schema abstraction.
/// </summary>
public interface ISchema<T>
{
    /// <summary>
    /// Decode the raw bytes.
    /// </summary>
    T Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null);

    /// <summary>
    /// Encode the message.
    /// </summary>
    ReadOnlySequence<byte> Encode(T message);

    /// <summary>
    /// The schema info.
    /// </summary>
    SchemaInfo SchemaInfo { get; }
}
