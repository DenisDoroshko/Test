using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using ThirdPartyEventEditor.Models;
using ThirdPartyEventEditor.Data;
using ThirdPartyEventEditor.Services;
using Moq;
using FluentAssertions;

namespace Tests.UnitTests
{
    public class EventServiceTests
    {
        [Test]
        public void CreateEvent_WhenCreateEvent_NewEventCreated()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();
            var events = new List<ThirdPartyEvent>
            {
                new ThirdPartyEvent { Name = "First event", Description = "Movie", VenueName = "First venue", LayoutName = "First layout" },
                new ThirdPartyEvent { Name = "Second event", Description = "Concert", VenueName = "Second venue", LayoutName = "Second layout" },
            };

            var eventModel = new ThirdPartyEvent
            {
                Name = "New event",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(1),
                EndDate = DateTime.UtcNow.AddSeconds(2),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(events);
            var service = new EventService(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(t => t.Create(It.IsAny<ThirdPartyEvent>()))
                .Callback<ThirdPartyEvent>(e => events.Add(e));

            // Act
            var result = service.CreateEvent(eventModel);

            // Assert
            result.IsValid.Should().BeTrue();
            events.Should().BeEquivalentTo(new List<ThirdPartyEvent>
            {
                new ThirdPartyEvent { Name = "First event", Description = "Movie",VenueName = "First venue", LayoutName = "First layout"  },
                new ThirdPartyEvent { Name = "Second event", Description = "Concert", VenueName = "Second venue", LayoutName = "Second layout" },
                new ThirdPartyEvent { Name = "New event", Description = "Match",VenueName = "Third venue", LayoutName = "Third layout" },
            }, options => options.Excluding(x => x.StartDate).Excluding(x => x.EndDate));
        }

        [Test]
        public void CreateEvent_WhenEventDateExistInVenue_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();
            var events = new List<ThirdPartyEvent>
            {
                new ThirdPartyEvent { Id = 1, StartDate =  DateTime.UtcNow, EndDate = DateTime.UtcNow.AddSeconds(5), Name = "First event", Description = "Movie", VenueName = "First venue", LayoutName = "First layout" },
            };

            var eventModel = new ThirdPartyEvent
            {
                Name = "New event",
                Description = "Match",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                VenueName = "First venue",
                LayoutName = "First layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(events);
            var service = new EventService(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(t => t.Create(It.IsAny<ThirdPartyEvent>()))
                .Callback<ThirdPartyEvent>(e => events.Add(e));

            // Act
            var result = service.CreateEvent(eventModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.SingleOrDefault(error =>
                error.ErrorMessage == "Can not create event for the same venue in the same time.").Should().NotBeNull();
        }

        [Test]
        public void CreateEvent_WhenEventEndBeforeStart_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();

            var eventModel = new ThirdPartyEvent
            {
                Name = "New event",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(1),
                EndDate = DateTime.UtcNow,
                VenueName = "First venue",
                LayoutName = "First layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(new List<ThirdPartyEvent>());
            var service = new EventService(eventRepositoryMock.Object);

            // Act
            var result = service.CreateEvent(eventModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.SingleOrDefault(error =>
                error.ErrorMessage == "Event end can not be before event start.").Should().NotBeNull();
        }

        [Test]
        public void CreateEvent_WhenCreateInPast_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();

            var eventModel = new ThirdPartyEvent
            {
                Name = "New event",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(-10),
                EndDate = DateTime.UtcNow.AddSeconds(-10),
                VenueName = "First venue",
                LayoutName = "First layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(new List<ThirdPartyEvent>());
            var service = new EventService(eventRepositoryMock.Object);

            // Act
            var result = service.CreateEvent(eventModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.SingleOrDefault(error =>
                error.ErrorMessage == "Can not create event in the past.").Should().NotBeNull();
        }

        [Test]
        public void CreateEvent_WhenEventIsNull_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();
            var service = new EventService(eventRepositoryMock.Object);

            // Act
            var result = service.CreateEvent(null);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void UpdateEvent_WhenUpdateEvent_ListWithUpdatedEvent()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();
            var events = new List<ThirdPartyEvent>
            {
                new ThirdPartyEvent { Id = 1, Name = "First event", Description = "Movie", VenueName = "First venue", LayoutName = "First layout" },
                new ThirdPartyEvent { Id = 2, Name = "Second event", Description = "Concert", VenueName = "Second venue", LayoutName = "Second layout" },
                new ThirdPartyEvent { Id = 3, Name = "Third event", Description = "Match", VenueName = "Third venue", LayoutName = "Third layout" },
            };

            var eventModel = new ThirdPartyEvent
            {
                Id = 3,
                Name = "Updated event name",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(1),
                EndDate = DateTime.UtcNow.AddSeconds(2),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(events);
            var service = new EventService(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(t => t.Update(It.IsAny<ThirdPartyEvent>())).Callback<ThirdPartyEvent>(eventToUpdate =>
            {
                events.Remove(events.FirstOrDefault(a => a.Id == eventToUpdate.Id));
                events.Add(eventToUpdate);
            });
            eventRepositoryMock.Setup(t => t.GetById(3)).Returns(new ThirdPartyEvent
                {
                    Name = "Third event",
                    Description = "Match",
                    StartDate = DateTime.UtcNow.AddSeconds(1),
                    EndDate = DateTime.UtcNow.AddSeconds(2),
                    VenueName = "Third venue",
                    LayoutName = "Third layout"
                });

            // Act
            var result = service.UpdateEvent(eventModel);

            // Assert
            result.IsValid.Should().BeTrue();
            events.Should().BeEquivalentTo(new List<ThirdPartyEvent>
            {
                new ThirdPartyEvent { Id = 1, Name = "First event", Description = "Movie",VenueName = "First venue", LayoutName = "First layout"  },
                new ThirdPartyEvent { Id = 2, Name = "Second event", Description = "Concert", VenueName = "Second venue", LayoutName = "Second layout" },
                new ThirdPartyEvent { Id = 3, Name = "Updated event name", Description = "Match",VenueName = "Third venue", LayoutName = "Third layout" },
            }, options => options.Excluding(x => x.StartDate).Excluding(x => x.EndDate));
        }

        [Test]
        public void UpdateEvent_WhenEventNotExist_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();

            var eventModel = new ThirdPartyEvent
            {
                Id = 3,
                Name = "Updated event name",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(1),
                EndDate = DateTime.UtcNow.AddSeconds(2),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(new List<ThirdPartyEvent>());
            var service = new EventService(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(t => t.GetById(3)).Returns(default(ThirdPartyEvent));

            // Act
            var result = service.UpdateEvent(eventModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.SingleOrDefault(error =>
                error.ErrorMessage == "There is no event with this id.").Should().NotBeNull();
        }

        [Test]
        public void UpdateEvent_WhenEndBeforeStart_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();

            var eventModel = new ThirdPartyEvent
            {
                Id = 3,
                Name = "Updated event name",
                Description = "Match",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddSeconds(-10),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(new List<ThirdPartyEvent>());
            var service = new EventService(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(t => t.GetById(3)).Returns(default(ThirdPartyEvent));

            // Act
            var result = service.UpdateEvent(eventModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.SingleOrDefault(error =>
                error.ErrorMessage == "Event end can not be before event start.").Should().NotBeNull();
        }

        [Test]
        public void UpdateEvent_WhenUpdateInPast_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();

            var eventModel = new ThirdPartyEvent
            {
                Id = 3,
                Name = "Updated event name",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(-10),
                EndDate = DateTime.UtcNow.AddSeconds(-10),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(new List<ThirdPartyEvent>());
            var service = new EventService(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(t => t.GetById(3)).Returns(default(ThirdPartyEvent));

            // Act
            var result = service.UpdateEvent(eventModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.SingleOrDefault(error =>
                error.ErrorMessage == "Can not update event in the past.").Should().NotBeNull();
        }

        [Test]
        public void UpdateEvent_WhenEventIsNull_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(new List<ThirdPartyEvent>());
            var service = new EventService(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(t => t.GetById(3)).Returns(default(ThirdPartyEvent));

            // Act
            var result = service.UpdateEvent(null);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void DeleteEvent_WhenDeleteEvent_ListWithoutDeletedEvent()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();
            var events = new List<ThirdPartyEvent>
            {
                new ThirdPartyEvent { Id = 1, Name = "First event", Description = "Movie", VenueName = "First venue", LayoutName = "First layout" },
                new ThirdPartyEvent { Id = 2, Name = "Second event", Description = "Concert", VenueName = "Second venue", LayoutName = "Second layout" },
                new ThirdPartyEvent { Id = 3, Name = "Third event", Description = "Match", VenueName = "Third venue", LayoutName = "Third layout" },
            };

            var eventModel = new ThirdPartyEvent
            {
                Id = 3,
                Name = "Third event",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(1),
                EndDate = DateTime.UtcNow.AddSeconds(2),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(events);
            var service = new EventService(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(t => t.Delete(It.IsAny<ThirdPartyEvent>())).Callback<ThirdPartyEvent>(eventToDelete =>
            {
                events.Remove(events.FirstOrDefault(a => a.Id == eventToDelete.Id));
            });
            eventRepositoryMock.Setup(t => t.GetById(3)).Returns(new ThirdPartyEvent
            {
                Name = "Third event",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(1),
                EndDate = DateTime.UtcNow.AddSeconds(2),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            });

            // Act
            var result = service.DeleteEvent(eventModel);

            // Assert
            result.IsValid.Should().BeTrue();
            events.Should().BeEquivalentTo(new List<ThirdPartyEvent>
            {
                new ThirdPartyEvent { Id = 1, Name = "First event", Description = "Movie",VenueName = "First venue", LayoutName = "First layout"  },
                new ThirdPartyEvent { Id = 2, Name = "Second event", Description = "Concert", VenueName = "Second venue", LayoutName = "Second layout" },
            }, options => options.Excluding(x => x.StartDate).Excluding(x => x.EndDate));
        }

        [Test]
        public void DeleteEvent_WhenEventNotExist_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();

            var eventModel = new ThirdPartyEvent
            {
                Id = 3,
                Name = "Third event",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(1),
                EndDate = DateTime.UtcNow.AddSeconds(2),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            };
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(new List<ThirdPartyEvent>());
            var service = new EventService(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(t => t.GetById(3)).Returns(default(ThirdPartyEvent));

            // Act
            var result = service.DeleteEvent(eventModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors
                .SingleOrDefault(error => error.ErrorMessage == "There is no event with this id.")
                .Should().NotBeNull();
        }

        [Test]
        public void DeleteEvent_WhenEventIsNull_ShouldFailValidation()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();
            eventRepositoryMock.Setup(t => t.GetAll()).Returns(new List<ThirdPartyEvent>());
            var service = new EventService(eventRepositoryMock.Object);

            // Act
            var result = service.DeleteEvent(null);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void GetEvents_WhenGetEvents_EventsList()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();
            var events = new List<ThirdPartyEvent>
            {
                new ThirdPartyEvent { Id = 1, Name = "First event", Description = "Movie", VenueName = "First venue", LayoutName = "First layout" },
                new ThirdPartyEvent { Id = 2, Name = "Second event", Description = "Concert", VenueName = "Second venue", LayoutName = "Second layout" },
                new ThirdPartyEvent { Id = 3, Name = "Third event", Description = "Match", VenueName = "Third venue", LayoutName = "Third layout" },
            };

            eventRepositoryMock.Setup(t => t.GetAll()).Returns(events);
            var service = new EventService(eventRepositoryMock.Object);

            // Act
            var gotEvents = service.GetEvents();

            // Assert
            gotEvents.Should().BeEquivalentTo(new List<ThirdPartyEvent>
            {
                new ThirdPartyEvent { Id = 1, Name = "First event", Description = "Movie",VenueName = "First venue", LayoutName = "First layout"  },
                new ThirdPartyEvent { Id = 2, Name = "Second event", Description = "Concert", VenueName = "Second venue", LayoutName = "Second layout" },
                new ThirdPartyEvent { Id = 3, Name = "Third event", Description = "Match",VenueName = "Third venue", LayoutName = "Third layout" },
            }, options => options.Excluding(x => x.StartDate).Excluding(x => x.EndDate));
        }

        [Test]
        public void GetEvents_WhenEventsNotExist_EmptyList()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();

            eventRepositoryMock.Setup(t => t.GetAll()).Returns(new List<ThirdPartyEvent>());
            var service = new EventService(eventRepositoryMock.Object);

            // Act
            var gotEvents = service.GetEvents();

            // Assert
            gotEvents.Should().BeEquivalentTo(new List<ThirdPartyEvent>());
        }

        [Test]
        public void GetEventById_WhenGetEvent_Event()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();
            var eventModel = new ThirdPartyEvent
            {
                Id = 3,
                Name = "Third event",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(1),
                EndDate = DateTime.UtcNow.AddSeconds(2),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            };

            eventRepositoryMock.Setup(t => t.GetById(3)).Returns(eventModel);
            var service = new EventService(eventRepositoryMock.Object);

            // Act
            var gottenEvent = service.GetEvent(3);

            // Assert
            gottenEvent.Should().BeEquivalentTo(new ThirdPartyEvent
            {
                Id = 3,
                Name = "Third event",
                Description = "Match",
                StartDate = DateTime.UtcNow.AddSeconds(1),
                EndDate = DateTime.UtcNow.AddSeconds(2),
                VenueName = "Third venue",
                LayoutName = "Third layout"
            }, options => options.Excluding(x => x.StartDate).Excluding(x => x.EndDate));
        }

        [Test]
        public void GetEventById_WhenEventNotExist_Null()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IRepository<ThirdPartyEvent>>();

            eventRepositoryMock.Setup(t => t.GetById(3)).Returns(default(ThirdPartyEvent));
            var service = new EventService(eventRepositoryMock.Object);

            // Act
            var gottenEvent = service.GetEvent(3);

            // Assert
            gottenEvent.Should().BeNull();
        }
    }
}
