# AuthListTest  (Authorization List Checker Test)

AuthListTest is a performance comparison test utility of SessionData (authorization list manager) library.
In the SessionData implemented two modes:

  - RAW: Direct access to sessions database
  - Cached: Access to sessions data via specialized cache manager
  
### Version
1.0.1

### Command Line options

```cmd
AuthListTest [-RAW] [-SILENT] [-ITERATIONS <#>] [-THREADS <#>]
```
* [-RAW] - Raw mode test (Cached mode test by default)
* [-SILENT] - suppress messages, output measurement result only 
* [-ITERATIONS <#>] - number of authentication operations in each thread
* [-THREADS <#>] - number of test threads (users number)

### Dependencies
* System.Data.SQLite (x86/x64) 1.0.98.1 NuGet Package
* SQLite netFx46 setup bundle 2015 1.0.98.0

## License

*[MIT License]* 

Copyright (c) 2015 Igor Alexeev, axaprj2000@yahoo.com