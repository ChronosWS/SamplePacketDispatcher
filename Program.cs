using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutofacSerilogIntegration;
using SamplePacketDispatcher.Contracts;
using SamplePacketDispatcher.Dispatcher;
using SamplePacketDispatcher.Handlers;
using SamplePacketDispatcher.Loggers;
using SamplePacketDispatcher.Packets;
using Serilog;

namespace SamplePacketDispatcher
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            Setup();
            Run();
        }

        /// <summary>
        /// Set up our dependency injection container and logger
        /// </summary>
        static void Setup()
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            var builder = new ContainerBuilder();
            builder.RegisterLogger();

            RegisterComponents(builder);

            Container = builder.Build();
        }

        /// <summary>
        /// Register all of the packet-related components
        /// </summary>
        static void RegisterComponents(ContainerBuilder builder)
        {
            // Register packet handlers
            builder.RegisterType<PacketHandlerOne>().As<IPacketHandler>().SingleInstance();
            builder.RegisterType<PacketHandlerTwo>().As<IPacketHandler>().SingleInstance();
            builder.RegisterType<PacketHandlerThree>().As<IPacketHandler>().SingleInstance();

            // Register dispatcher
            builder.RegisterType<PacketDispatcher>().As<IPacketDispatcher>().SingleInstance();

            builder.RegisterType<PacketLogger>().As<IPacketLogger>().SingleInstance();
        }

        /// <summary>
        /// Run the main packet processing loop
        /// </summary>
        static void Run()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var dispatcher = scope.Resolve<IPacketDispatcher>();
                var packerLogger = scope.Resolve<IPacketLogger>();
                ProcessPacketsAsync(dispatcher, packerLogger, CancellationToken.None).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// The main packet processing loop.
        /// </summary>
        /// <remarks>
        /// This method illustrates how the dispatcher can be used. For this illustration
        /// we use an async foreach, to simulate a thread which receives and processes packets
        /// independent of the rest of the process, and which responds to cancellation tokens
        /// by terminating any outstanding dispatches and terminating the waits on any
        /// incoming packets.
        /// </remarks>
        private static async Task ProcessPacketsAsync(
            IPacketDispatcher dispatcher,
            IPacketLogger packetLogger,
            CancellationToken cancellationToken)
        {
            await foreach (var packet in GeneratePacketsAsync(cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    await dispatcher.DispatchAsync(packet, cancellationToken);
                }
                catch (Exception ex)
                {
                    Guid correlationId = Guid.NewGuid();
                    Log.Error(ex, "{correlationId}: Exception invoking packet dispatcher: {id} {type}", correlationId, packet?.Id, packet?.GetType().Name);
                    packetLogger.LogPacket(PacketLogReason.Error, correlationId, packet);
                }
            }
        }

        /// <summary>
        /// This is a sample packet source, assumed to be asynchronous and producing fully-formed packets.
        /// </summary>
        private static async IAsyncEnumerable<IPacket> GeneratePacketsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var testData = new byte[] { 1, 2, 3 };
            var packets = new[] {
                new TestPacket { Id = 1, Data = testData },
                new TestPacket { Id = 2, Data = testData },
                new TestPacket { Id = 3, Data = testData },
                new TestPacket { Id = 4, Data = testData },
                new TestPacket { Id = 2, Data = testData },
                new TestPacket { Id = 5, Data = testData },
                new TestPacket { Id = 2, Data = TestPacket.BAD_DATA },
                new TestPacket { Id = 3, Data = testData },
                new TestPacket { Id = 2, Data = testData },
                new TestPacket { Id = 1, Data = testData },
            };

            foreach (var packet in packets)
            {
                await Task.Delay(500, cancellationToken);
                yield return packet;
            }
        }
    }
}
