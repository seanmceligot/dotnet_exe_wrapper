using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dotnet {
    public class Worker : IHostedService, IDisposable {
			private readonly ILogger<Worker> _logger;
			private Process ps = new Process();
			private StreamWriter _log = null;

			public Worker(ILogger<Worker> logger) {
					_logger = logger;
			}

			public Task StartAsync(CancellationToken stoppingToken) {
					_logger.LogInformation("start.");

					string cmd = "./stdouterr";
        	_log = File.AppendText(cmd+".log");

          ps.StartInfo.UseShellExecute = false;
          ps.StartInfo.RedirectStandardError = true;  
          ps.StartInfo.RedirectStandardOutput = true;
					ps.StartInfo.FileName = cmd;

					try {
						if (!ps.Start()) {
							_logger.LogInformation("not started: " + ps);
						}
						_logger.LogInformation("started: " + ps);
	 				} catch (Exception e) {
                _logger.LogError(e.Message);
          }
					ps.OutputDataReceived += (sender, e) => _log.WriteLine(e.Data);
					ps.ErrorDataReceived += (sender, e) => _log.WriteLine(e.Data);
				
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
					ps.Dispose();
					_log.Close();
					_log.Dispose();
			}

	
	}
}
