﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Data.SqlClient;
using Xunit;

namespace Microsoft.DotNet.Interactive.SqlServer.Tests;

public sealed class MsSqlFactAttribute : FactAttribute
{
    private const string TEST_MSSQL_CONNECTION_STRING = nameof(TEST_MSSQL_CONNECTION_STRING);
    private static readonly string _skipReason;
        
    static MsSqlFactAttribute()
    {
        _skipReason = TestConnectionAndReturnSkipReason();
    }
        
    public MsSqlFactAttribute()
    {
        if (_skipReason is not null)
        {
            Skip = _skipReason;
        }
    }
        
    internal static string TestConnectionAndReturnSkipReason()
    {
        string connectionString = GetConnectionStringForTests();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return $"Environment variable {TEST_MSSQL_CONNECTION_STRING} is not set. To run tests that require "
                   + "SQL Server, this environment variable must be set to a valid connection string value.";
        }
            
        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
        }
        catch (Exception e)
        {
            return $"A connection could not be established to SQL Server. Verify the connection string value used " +
                   $"for environment variable {TEST_MSSQL_CONNECTION_STRING} targets a running SQL Server instance. " +
                   $"Connection failed failed with error: {e}";                    
        }

        return null;
    }
        
    public static string GetConnectionStringForTests()
    {
        // example:
        // "Persist Security Info=False; Integrated Security=true; Initial Catalog=AdventureWorks; Server=localhost"
        return Environment.GetEnvironmentVariable(TEST_MSSQL_CONNECTION_STRING); 
    }  
}