using System.Text.Json;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages;
using AeroTech.Ordering.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AeroTech.Ordering.Consumers.Outbox
{
    public sealed class OutboxPublisher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBus _bus;
        private readonly OutboxPublisherOptions _options;
        private readonly ILogger<OutboxPublisher> _logger;

        public OutboxPublisher(
            IServiceScopeFactory scopeFactory,
            IBus bus,
            IOptions<OutboxPublisherOptions> options,
            ILogger<OutboxPublisher> logger)
        {
            _scopeFactory = scopeFactory;
            _bus = bus;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
        }

         
    }
}
