Feature: Title Renamer Test

Scenario: General
	Given directory "/root" contains files
| FileName |
| aaa      |
| bbb.xml  |
| ccc.xsl  |
| ddd.cs   |
	And directory "/root/internal" contains files
| FileName           |
| aaa                |
| test.xml           |
| test_namespace.xsl |
| updated.xslt       |
	And file "/root/internal/test.xml" contains
"""
<?xml version="1.0" encoding="utf-16"?>
<test title="This Trisoft will be updated">
  <line1>Trisoft has been renamed to SDL Trisoft</line1>
</test>
"""
	And file "/root/internal/test_namespace.xsl" contains
"""
<?xml version="1.0" encoding="utf-16"?>
<test h:title="This Trisoft will be updated" xmlns:h="http://www.w3.org/TR/html4/">
  <line1>Trisoft has been renamed to SDL Trisoft</line1>
</test>
"""
	And file "/root/internal/updated.xslt" contains
"""
<?xml version="1.0" encoding="utf-16"?>
<test title="This SDL Trisoft will not be updated">
  <line1>SDL Trisoft don't have to be changed SDL Trisoft</line1>
</test>
"""
	When "(?<!SDL )Trisoft" is replaced by "SDL Trisoft" in directory "/root/internal"
	Then directory "/root" should contain
| FileName |
| aaa      |
| bbb.xml  |
| ccc.xsl  |
| ddd.cs   |
	And directory "/root/internal" should contain
| FileName               |
| aaa                    |
| test.xml               |
| test.xml.bak           |
| test_namespace.xsl     |
| test_namespace.xsl.bak |
| updated.xslt           |
	And file "/root/internal/test.xml.bak" should contain
"""
<?xml version="1.0" encoding="utf-16"?>
<test title="This Trisoft will be updated">
  <line1>Trisoft has been renamed to SDL Trisoft</line1>
</test>
"""
	And file "/root/internal/test.xml" should contain
"""
<?xml version="1.0" encoding="utf-16"?>
<test title="This SDL Trisoft will be updated">
  <line1>SDL Trisoft has been renamed to SDL Trisoft</line1>
</test>
"""
	And file "/root/internal/test_namespace.xsl.bak" should contain
"""
<?xml version="1.0" encoding="utf-16"?>
<test h:title="This Trisoft will be updated" xmlns:h="http://www.w3.org/TR/html4/">
  <line1>Trisoft has been renamed to SDL Trisoft</line1>
</test>
"""
	And file "/root/internal/test_namespace.xsl" should contain
"""
<?xml version="1.0" encoding="utf-16"?>
<test h:title="This SDL Trisoft will be updated" xmlns:h="http://www.w3.org/TR/html4/">
  <line1>SDL Trisoft has been renamed to SDL Trisoft</line1>
</test>
"""
	And file "/root/internal/updated.xslt" should contain
"""
<?xml version="1.0" encoding="utf-16"?>
<test title="This SDL Trisoft will not be updated">
  <line1>SDL Trisoft don't have to be changed SDL Trisoft</line1>
</test>
"""
    And rest of the files should be unchanged