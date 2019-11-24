namespace PskOnline.X20.Protocol.WinUSB.Test
{
  using Serilog;
  using Serilog.Events;

  public static class SerilogHelper
  {
    public static Microsoft.Extensions.Logging.ILogger CreateLogger(string loggerName)
    {
      var loggers = new LoggerConfiguration()
             .MinimumLevel.Verbose()
             .Enrich.FromLogContext();

      loggers.WriteTo.Logger(logger => logger
        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose));

      Log.Logger = loggers.CreateLogger();
      Log.Logger.Information("Logger is initialized");

      var loggerFactory = new Serilog.Extensions.Logging.SerilogLoggerProvider();

      return loggerFactory.CreateLogger(loggerName);
    }

    public static Microsoft.Extensions.Logging.ILoggerFactory GetLoggerFactory()
    {
      var fac = new Microsoft.Extensions.Logging.LoggerFactory();
      fac.AddSerilog();
      return fac;
    }

  }
}
