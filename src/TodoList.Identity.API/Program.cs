using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading.Tasks;

namespace TodoList.Identity.API
{
  public class Program
  {
    static async Task Main(string[] args)
    {
      await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host
          .CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder
              .UseStartup<Startup>()
              .UseSerilog();
          });
  }
}
