global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Stowaway.Infrastructure.Database;
global using Microsoft.EntityFrameworkCore;

// WebApplicationFactory<Program> uses a process-wide HostFactoryResolver hook to intercept
// Program.Main; two test classes booting it at once (xUnit's default parallel collections)
// race on that hook and produce spurious "entry point exited without building an IHost" /
// intermittent 401s. Serializing test collections avoids it.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
