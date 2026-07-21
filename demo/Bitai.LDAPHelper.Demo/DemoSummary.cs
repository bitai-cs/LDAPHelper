using System;
using System.Collections.Generic;
using Serilog;

namespace Bitai.LDAPHelper.Demo;

/// <summary>
/// Collects and prints summary information for executed demo scenarios.
/// </summary>
public class DemoSummary
{
    private readonly Dictionary<string, bool> _demoResults = new Dictionary<string, bool>();
    private readonly List<string> _errors = new List<string>();

    /// <summary>
    /// Records the result of one demo scenario.
    /// </summary>
    /// <param name="demoName">Scenario name.</param>
    /// <param name="success">Whether the scenario succeeded.</param>
    /// <param name="error">Optional error associated with failure.</param>
    public void RecordDemoResult(string demoName, bool success, Exception error = null)
    {
        _demoResults[demoName] = success;
        if (!success && error != null)
        {
            _errors.Add($"{demoName}: {error.Message}");
        }
    }

    /// <summary>
    /// Prints an aggregated summary for all recorded demo results.
    /// </summary>
    public void PrintSummary()
    {
        Console.WriteLine();
        Console.WriteLine("================================================");
        Console.WriteLine("           DEMO EXECUTION SUMMARY               ");
        Console.WriteLine("================================================");
        
        var total = _demoResults.Count;
        var successful = 0;
        var failed = 0;
        
        foreach (var result in _demoResults)
        {
            if (result.Value)
            {
                successful++;
                Log.Information($"✅ {result.Key}: SUCCESS");
            }
            else
            {
                failed++;
                Log.Error($"❌ {result.Key}: FAILED");
            }
        }
        
        Console.WriteLine();
        Log.Information($"Total Demos: {total}");
        Log.Information($"Successful: {successful}");
        Log.Information($"Failed: {failed}");
        
        if (_errors.Count > 0)
        {
            Console.WriteLine();
            Log.Warning("Errors encountered:");
            foreach (var error in _errors)
            {
                Log.Error($"  - {error}");
            }
        }
        
        Console.WriteLine("================================================");
    }
}
