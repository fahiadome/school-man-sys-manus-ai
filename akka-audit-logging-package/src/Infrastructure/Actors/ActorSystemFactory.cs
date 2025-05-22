using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolManagement.Domain.Models.Audit;
using SchoolManagement.Infrastructure.Actors.Audit;

namespace SchoolManagement.Infrastructure.Actors
{
    /// <summary>
    /// Factory for creating and managing the Akka.NET actor system
    /// </summary>
    public class ActorSystemFactory : IDisposable
    {
        private readonly ILogger<ActorSystemFactory> _logger;
        private ActorSystem _actorSystem;
        private IActorRef _auditCoordinatorActor;
        private IActorRef _auditQueryActor;
        private bool _disposed = false;

        public ActorSystemFactory(ILogger<ActorSystemFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Initialize();
        }

        /// <summary>
        /// Initialize the actor system and create root actors
        /// </summary>
        private void Initialize()
        {
            try
            {
                _logger.LogInformation("Initializing Akka.NET actor system");

                // Create actor system with configuration
                var config = ConfigurationFactory.ParseString(@"
                    akka {
                        loglevel = INFO
                        loggers = [""Akka.Logger.Extensions.Logging.LoggingLogger, Akka.Logger.Extensions.Logging""]
                        actor {
                            debug {
                                receive = on
                                autoreceive = on
                                lifecycle = on
                                event-stream = on
                                unhandled = on
                            }
                        }
                        persistence {
                            journal {
                                plugin = ""akka.persistence.journal.sql-server""
                                sql-server {
                                    class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
                                    connection-string = ""<connection-string>""
                                    schema-name = dbo
                                    table-name = EventJournal
                                    auto-initialize = on
                                }
                            }
                            snapshot-store {
                                plugin = ""akka.persistence.snapshot-store.sql-server""
                                sql-server {
                                    class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                                    connection-string = ""<connection-string>""
                                    schema-name = dbo
                                    table-name = SnapshotStore
                                    auto-initialize = on
                                }
                            }
                        }
                    }
                ");

                _actorSystem = ActorSystem.Create("SchoolManagementSystem", config);

                // Create root actors
                _auditCoordinatorActor = _actorSystem.ActorOf(
                    Props.Create(() => new AuditCoordinatorActor()),
                    "audit-coordinator");

                _auditQueryActor = _actorSystem.ActorOf(
                    Props.Create(() => new AuditQueryActor()),
                    "audit-query");

                _logger.LogInformation("Akka.NET actor system initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Akka.NET actor system");
                throw;
            }
        }

        /// <summary>
        /// Get the audit coordinator actor reference
        /// </summary>
        public IActorRef GetAuditCoordinatorActor()
        {
            return _auditCoordinatorActor;
        }

        /// <summary>
        /// Get the audit query actor reference
        /// </summary>
        public IActorRef GetAuditQueryActor()
        {
            return _auditQueryActor;
        }

        /// <summary>
        /// Get the actor system
        /// </summary>
        public ActorSystem GetActorSystem()
        {
            return _actorSystem;
        }

        /// <summary>
        /// Dispose the actor system
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the actor system
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logger.LogInformation("Shutting down Akka.NET actor system");
                    _actorSystem?.Terminate().Wait();
                }

                _disposed = true;
            }
        }
    }
}
