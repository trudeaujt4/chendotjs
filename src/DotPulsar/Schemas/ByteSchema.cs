﻿/*
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

namespace DotPulsar.Schemas;

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using System;
using System.Buffers;
using System.Collections.Immutable;

/// <summary>
/// Schema definition for Byte (int8) messages.
/// </summary>
public sealed class ByteSchema : ISchema<byte>
{
    public ByteSchema()
        => SchemaInfo = new SchemaInfo("INT8", Array.Empty<byte>(), SchemaType.Int8, ImmutableDictionary<string, string>.Empty);

    public SchemaInfo SchemaInfo { get; }

    public byte Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        if (bytes.Length != 1)
            throw new SchemaSerializationException($"{nameof(ByteSchema)} expected to decode 1 byte, but received {bytes} bytes");

        return bytes.First.Span[0];
    }

    public ReadOnlySequence<byte> Encode(byte message)
        => new(new[] { message });
}
