// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Benchmarks.Configuration;

namespace Benchmarks.Data;

internal class BatchUpdateString
{
    private const int MaxBatch = 500;

    private static readonly string[] _queries = new string[MaxBatch + 1];

    public static DatabaseServer DatabaseServer;

    public static string Query(int batchSize)
    {
        if (_queries[batchSize] != null)
        {
            return _queries[batchSize];
        }

        var lastIndex = batchSize - 1;

        var sb = StringBuilderCache.Acquire();

        if (DatabaseServer == DatabaseServer.PostgreSql)
        {
            sb.Append("UPDATE world SET randomNumber = temp.randomNumber FROM (VALUES ");
            Enumerable.Range(0, lastIndex).ToList().ForEach(i => sb.Append($"(@Id_{i}, @Random_{i}), "));
            sb.Append($"(@Id_{lastIndex}, @Random_{lastIndex}) ORDER BY 1) AS temp(id, randomNumber) WHERE temp.id = world.id");
        }
        else
        {
            Enumerable.Range(0, batchSize).ToList().ForEach(i => sb.Append($"UPDATE world SET randomnumber = @Random_{i} WHERE id = @Id_{i};"));
        }

        return _queries[batchSize] = StringBuilderCache.GetStringAndRelease(sb);
    }
}
