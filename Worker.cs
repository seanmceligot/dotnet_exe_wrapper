using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace dotnet {
    public class Worker : IHostedService, IDisposable {
			private readonly ILogger<Worker> _logger;
			private readonly IConfiguration _config;
			private Process ps = new Process();
			private StreamWriter _log = null;

			public Worker(ILogger<Worker> logger, IConfiguration config) {
					_logger = logger;
          _config = config;
			}

			public Task StartAsync(CancellationToken stoppingToken) {
					_logger.LogInformation("start.");

          string cmd = _config["Cmd"];
          string args = _config["Args"];
          string logfile = _config["Log"];
        	_log = File.AppendText(logfile);
					_logger.LogInformation("log: " + logfile);

          ps.StartInfo.UseShellExecute = false;
          ps.StartInfo.RedirectStandardError = true;  
          ps.StartInfo.RedirectStandardOutput = true;
					ps.StartInfo.FileName = cmd;
					ps.StartInfo.Arguments = args;

					try {
						if (!ps.Start()) {
							_logger.LogInformation("not started: " + ps);
						}
						_logger.LogInformation("started: " + ps);
	 				} catch (Exception e) {
             _logger.LogError(e.Message);
          }
					ps.OutputDataReceived += (sender, e) => {
              if (e.Data != null) {
                _logger.LogInformation(e.Data);
               _log.WriteLine(e.Data);
              }
          };
					ps.ErrorDataReceived += (sender, e) => {
              if (e.Data != null) {
                _logger.LogError(e.Data);
               _log.WriteLine(e.Data);
              }
          };

				
					ps.BeginOutputReadLine();
					ps.BeginErrorReadLine();
					_logger.LogInformation("started.");
					return Task.CompletedTask;
			}

			public Task StopAsync(CancellationToken stoppingToken) {
					_logger.LogInformation("stop.");
					return Task.CompletedTask;
			}
			public void Dispose() {
					_logger.LogInformation("dispose.");
          var killProcess = bool.Parse(_config["KillProcess"]);
          if (killProcess) {
            ps.Kill(true);
          }
					ps.Dispose();
					_log.Close();
					_log.Dispose();
			}
	}
}
