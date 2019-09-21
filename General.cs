using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NETFRAMEWORK
using System.Security;
#endif

// COM Compliance
[assembly: ComVisible(false)]

#if NETFRAMEWORK
// Security
[assembly: SecurityRules(SecurityRuleSet.Level2)]
#endif

// Required for tests to see internals.
[assembly: InternalsVisibleTo("Sharp.Diagnostics.Logging.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] 
