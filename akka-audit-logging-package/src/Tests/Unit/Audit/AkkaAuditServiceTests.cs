using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagement.Application.Services;
using SchoolManagement.Domain.Models.Audit;
using SchoolManagement.Infrastructure.Actors;
using Xunit;
using Akka.Actor;
using Akka.TestKit.Xunit2;

namespace SchoolManagement.Tests.Unit.Audit
{
    public class AkkaAuditServiceTests : TestKit
    {
        private readonly Mock<ILogger<AkkaAuditService>> _loggerMock;
        private readonly Mock<ActorSystemFactory> _actorSystemFactoryMock;
        private readonly TestProbe _auditCoordinatorProbe;
        private readonly TestProbe _auditQueryProbe;
        private readonly AkkaAuditService _auditService;

        public AkkaAuditServiceTests() : base()
        {
            _loggerMock = new Mock<ILogger<AkkaAuditService>>();
            _actorSystemFactoryMock = new Mock<ActorSystemFactory>();
            
            _auditCoordinatorProbe = CreateTestProbe("audit-coordinator");
            _auditQueryProbe = CreateTestProbe("audit-query");
            
            _actorSystemFactoryMock.Setup(x => x.GetAuditCoordinatorActor())
                .Returns(_auditCoordinatorProbe.Ref);
                
            _actorSystemFactoryMock.Setup(x => x.GetAuditQueryActor())
                .Returns(_auditQueryProbe.Ref);
                
            _auditService = new AkkaAuditService(_loggerMock.Object, _actorSystemFactoryMock.Object);
        }

        [Fact]
        public async Task LogCreateAsync_ShouldSendCreateAuditCommand()
        {
            // Arrange
            var tenantId = "tenant-1";
            var entityId = "entity-1";
            var userId = "user-1";
            var entity = new TestEntity { Id = entityId, Name = "Test Entity" };
            
            _auditCoordinatorProbe.SetAutoPilot(new RespondWithAutoPilot(true));

            // Act
            await _auditService.LogCreateAsync(tenantId, entityId, entity, userId);

            // Assert
            var receivedMessage = _auditCoordinatorProbe.ExpectMsg<CreateAuditCommand>();
            Assert.Equal(tenantId, receivedMessage.TenantId);
            Assert.Equal(typeof(TestEntity).Name, receivedMessage.EntityName);
            Assert.Equal(entityId, receivedMessage.EntityId);
            Assert.Equal(userId, receivedMessage.UserId);
            Assert.NotNull(receivedMessage.NewValues);
        }

        [Fact]
        public async Task LogUpdateAsync_ShouldSendUpdateAuditCommand()
        {
            // Arrange
            var tenantId = "tenant-1";
            var entityId = "entity-1";
            var userId = "user-1";
            var originalEntity = new TestEntity { Id = entityId, Name = "Original Name" };
            var updatedEntity = new TestEntity { Id = entityId, Name = "Updated Name" };
            
            _auditCoordinatorProbe.SetAutoPilot(new RespondWithAutoPilot(true));

            // Act
            await _auditService.LogUpdateAsync(tenantId, entityId, originalEntity, updatedEntity, userId);

            // Assert
            var receivedMessage = _auditCoordinatorProbe.ExpectMsg<UpdateAuditCommand>();
            Assert.Equal(tenantId, receivedMessage.TenantId);
            Assert.Equal(typeof(TestEntity).Name, receivedMessage.EntityName);
            Assert.Equal(entityId, receivedMessage.EntityId);
            Assert.Equal(userId, receivedMessage.UserId);
            Assert.NotNull(receivedMessage.OriginalValues);
            Assert.NotNull(receivedMessage.NewValues);
        }

        [Fact]
        public async Task LogDeleteAsync_ShouldSendDeleteAuditCommand()
        {
            // Arrange
            var tenantId = "tenant-1";
            var entityId = "entity-1";
            var userId = "user-1";
            var entity = new TestEntity { Id = entityId, Name = "Test Entity" };
            
            _auditCoordinatorProbe.SetAutoPilot(new RespondWithAutoPilot(true));

            // Act
            await _auditService.LogDeleteAsync(tenantId, entityId, entity, userId);

            // Assert
            var receivedMessage = _auditCoordinatorProbe.ExpectMsg<DeleteAuditCommand>();
            Assert.Equal(tenantId, receivedMessage.TenantId);
            Assert.Equal(typeof(TestEntity).Name, receivedMessage.EntityName);
            Assert.Equal(entityId, receivedMessage.EntityId);
            Assert.Equal(userId, receivedMessage.UserId);
            Assert.NotNull(receivedMessage.OriginalValues);
        }

        [Fact]
        public async Task GetAuditTrailAsync_ShouldSendGetAuditTrailQuery()
        {
            // Arrange
            var tenantId = "tenant-1";
            var entityName = "TestEntity";
            var entityId = "entity-1";
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;
            
            var expectedResult = new AuditTrailResult
            {
                Records = new List<AuditRecord>
                {
                    new AuditRecord
                    {
                        Id = 1,
                        TenantId = tenantId,
                        EntityName = entityName,
                        EntityId = entityId,
                        Action = "CREATE",
                        UserId = "user-1",
                        Timestamp = DateTime.UtcNow.AddDays(-5),
                        NewValues = "{\"Id\":\"entity-1\",\"Name\":\"Test Entity\"}"
                    }
                },
                TotalCount = 1
            };
            
            _auditQueryProbe.SetAutoPilot(new RespondWithAutoPilot(expectedResult));

            // Act
            var result = await _auditService.GetAuditTrailAsync(tenantId, entityName, entityId, startDate, endDate);

            // Assert
            var receivedMessage = _auditQueryProbe.ExpectMsg<GetAuditTrailQuery>();
            Assert.Equal(tenantId, receivedMessage.TenantId);
            Assert.Equal(entityName, receivedMessage.EntityName);
            Assert.Equal(entityId, receivedMessage.EntityId);
            Assert.Equal(startDate, receivedMessage.StartDate);
            Assert.Equal(endDate, receivedMessage.EndDate);
            
            Assert.NotNull(result);
            Assert.Single(result);
        }

        private class TestEntity
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        
        private class RespondWithAutoPilot : AutoPilot
        {
            private readonly object _response;
            
            public RespondWithAutoPilot(object response)
            {
                _response = response;
            }
            
            public override AutoPilot Run(ActorSystem system, IActorRef sender, object message)
            {
                sender.Tell(_response);
                return Keep;
            }
        }
    }
}
