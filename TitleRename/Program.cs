using System;
using TitleRename.IO;

namespace TitleRename {
    /*
     * Solution:
     * - XmlReplacer.IsFileToProcess is made in a way it is able to process windows-style extensins ('.xml') and exercise-style extensions ('xml').
     * - Places where to replace entries are configured by XmlReplacer parameters.
     * - User can configurable if backup file is needed along with a backup file extension (backup will have name "{fileName}{backupExtension}"). Both ".bak" and "bak" extension formats are supported.
     * 
     * Tests
     * - Solution algorithm is easy. File management logic is obvious. For me it seems enough to cover the whole solution with acceptance tests (integration level).
     * - SpecFlow seems right technology to use for an acceptance tests.
     *  If you have problems with running SpecFlow tests just look into logs in TestResults directory.
     *  Most likely you have to sign-in with a SpecFlow account (link to sign-in page is in the logs).
     * - Dependency to the file system should be eliminated to ease test automation (that's why I've created some utility classes and interfaces in TitleRename.IO).
     * I know there are ready solutions to mock file system dependencies (for example System.IO.Abstractions package) but there's not much file interations needed and I never used them before. Also I figured it out too late to use them in the implementation and don't want to spend even more personal time on a task.
     * 
     * Extra:
     * - I'd like to add more scenarios to the feature (ideally create a full set of acceptance tests) and perform a refactoring in steps (for example make use of IO.Abstractions there)
     * but I've already spend on the excercise much more time that I was expected so I'll just finish at this point.
     * - Don't know much about style guide you're using so I was using a usual style guide for my company (mostly based on Microsoft style guide).
     * - Because of a complex replacement condition Regular Expression pattern is used to match which text should be replaced (instead of ordinary string).
     * - Files may change the formatting but XML content remaining exactly the same (because XML is not formatting-dependent). If formatting should remain exactly the same as well different approach for solution should be used.
     * - Only exception messages are printed. Still, information about successfully changed/not changed files also returned (just to east the trace).
     * - All unnecessary stuff (mainly settings/options) is left where it is just to make a solution which is complete from my point of view (even though I understand it's not required by exercise conditions). It may easily be deleted/refactored if needed.
     * - I've spend around 6 hours on this exercise. I think I can do better, to create everything from scratch just not a usual case for me.
     * 
     * Exercise feedback:
     * - I was told it's a 3-hour task. Without a testing it really is.
     * But to get the solution properly tested (as required) it's full day task and, as for me, it's not fair to give sich big tasks for a potential newcomers.
     * I'd prefer more technical questions at a technical interview instead of such a big task.
     * - Task mey be shortened (more easily testable) if file system dependency is removed.
     */

    static class Program {
        private const string HelpText = "TitleRename.exe { [replace_pattern] [replacement] { [directory] } }";

        private static void Main(string[] args) {
            string replacePattern = "(?<!SDL )Trisoft";
            string replacement = "SDL Trisoft";
            string directory = Environment.CurrentDirectory;
            if (args.Length == 1) { throw new ArgumentException($"Not enough arguments for the call.\n{HelpText}"); }
            if (args.Length >= 2) {
                replacePattern = args[0];
                replacement = args[1];
            }
            if (args.Length == 3) {
                directory = args[2];
            }
            if (args.Length > 3) { throw new ArgumentException($"Too many arguments for the call.\n{HelpText}"); }

            var fileSystem = new FileSystem();
            var xmlReplacer = new XmlReplacer(fileSystem);
            foreach (var logMessage in xmlReplacer.ReplaceAllInDirectory(replacePattern, replacement, directory).ExceptionWhileProcessingLogs) {
                Console.WriteLine(logMessage);
            }
        }
    }
}
