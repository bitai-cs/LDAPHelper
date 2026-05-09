using System;
using System.Collections.Generic;
using Serilog;

namespace Bitai.LDAPHelper.Demo
{
    public class DemoSummary
    {
        private readonly Dictionary<string, bool> _demoResults = new Dictionary<string, bool>();
        private readonly List<string> _errors = new List<string>();

        public void RecordDemoResult(string demoName, bool success, Exception error = null)
        {
            _demoResults[demoName] = success;
            if (!success && error != null)
            {
                _errors.Add($"{demoName}: {error.Message}");
            }
        }

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
}