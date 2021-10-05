CMD call format:
**TitleRename.exe { [replace_pattern] [replacement] { [directory] } }**

Notes: 
 Solution:
 - XmlReplacer.IsFileToProcess is made in a way it is able to process windows-style extensins ('.xml') and exercise-style extensions ('xml').
 - Places where to replace entries are configured by XmlReplacer parameters.
 - User can configurable if backup file is needed along with a backup file extension (backup will have name "{fileName}{backupExtension}"). Both ".bak" and "bak" extension formats are supported.
 
 Tests
 - Solution algorithm is easy. File management logic is obvious. For me it seems enough to cover the whole solution with acceptance tests (integration level).
 - SpecFlow seems right technology to use for an acceptance tests.
 - Dependency to the file system should be eliminated to ease test automation (that's why I've created some utility classes and interfaces in TitleRename.IO).
 
 Extra:
 - Don't know much about style guide you're using so I was using a usual style guide for my company (mostly based on Microsoft style guide).
 - Because of a complex replacement condition Regular Expression pattern is used to match which text should be replaced (instead of ordinary string).
 - Files may change the formatting but XML content remaining exactly the same (because XML is not formatting-dependent). If formatting should remain exactly the same as well different approach for solution should be used.
 - Only exception messages are printed. Still, information about successfully changed/not changed files also returned (just to east the trace).
 - All unnecessary stuff (mainly settings/options) is left where it is just to make a solution which is complete from my point of view (even though I understand it's not required by exercise conditions). It may easily be deleted/refactored if needed.
 - I've spend around 5 hours on this exercise. I think I can do better, to create everything from scratch just not a usual case for me.
