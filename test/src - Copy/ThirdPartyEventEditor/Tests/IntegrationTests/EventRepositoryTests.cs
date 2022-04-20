using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThirdPartyEventEditor.Models;
using ThirdPartyEventEditor.Data;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace Tests.IntegrationTests
{
    public class EventRepositoryTests
    {
        private string _filePath;

        [SetUp]
        public void Setup()
        {
            _filePath = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build()["FilePath"];
            var eventsJson = JsonConvert.SerializeObject(new List<ThirdPartyEvent> { new ThirdPartyEvent { 
                Id = 1,
                Name = "First event",
                VenueName = "First venue",
                LayoutName = "First layout"},new ThirdPartyEvent {
                Id = 2,
                Name = "Second event",
                VenueName = "Second venue",
                LayoutName = "Second layout"} }, Formatting.Indented);
            File.WriteAllText(_filePath, eventsJson);
        }

        [Test]
        public void CreateEvent_WhenCreateEvent_NewEventCreated()
        {
            // Arrange
            var repository = new EventRepository(_filePath);
            var eventModel = new ThirdPartyEvent
            {
                Name = "Second event",
                VenueName = "First venue",
                LayoutName = "First layout",
            };

            // Act
            var id = repository.Create(eventModel);

            // Assert
            eventModel.Id = id;
            var createdEvent = GetEventById(id);
            createdEvent.Should().BeEquivalentTo(eventModel, options => options.Excluding(t => t.StartDate).Excluding(t => t.EndDate));
        }

        [Test]
        public void UpdateEvent_WhenUpdateEvent_UpdatedEvent()
        {
            // Arrange
            var repository = new EventRepository(_filePath);
            var eventModel = new ThirdPartyEvent
            {
                Id = 1,
                Name = "Updated event name",
                VenueName = "First venue",
                LayoutName = "First layout",
            };

            // Act
            repository.Update(eventModel);

            // Assert
            var updatedEvent = GetEventById(1);
            updatedEvent.Should().BeEquivalentTo(eventModel, options => options.Excluding(t => t.StartDate).Excluding(t => t.EndDate));
        }

        [Test]
        public void DeleteEvent_WhenDeleteEvent_EventNotExist()
        {
            // Arrange
            var repository = new EventRepository(_filePath);
            var eventModel = new ThirdPartyEvent
            {
                Id = 1,
                Name = "First event",
                VenueName = "First venue",
                LayoutName = "First layout",
            };
            var eventBeforeDelete = GetEventById(1);
            // Act
            repository.Delete(eventModel);

            // Assert
            var eventAfterDelete = GetEventById(1);
            eventBeforeDelete.Should().BeEquivalentTo(eventModel, options => options.Excluding(t => t.StartDate).Excluding(t => t.EndDate));
            eventAfterDelete.Should().BeNull();
        }

        [Test]
        public void GetById_WhenGetEvent_Event()
        {
            // Arrange
            var repository = new EventRepository(_filePath);
            var eventModel = new ThirdPartyEvent
            {
                Id = 1,
                Name = "First event",
                VenueName = "First venue",
                LayoutName = "First layout",
            };
            // Act
            var gottenEvent = repository.GetById(1);

            // Assert
            gottenEvent.Should().BeEquivalentTo(eventModel, options => options.Excluding(t => t.StartDate).Excluding(t => t.EndDate));
        }

        [Test]
        public void GetAll_WhenGetEvent_EventsList()
        {
            // Arrange
            var repository = new EventRepository(_filePath);
            var events = new List<ThirdPartyEvent> { new ThirdPartyEvent {
                Id = 1,
                Name = "First event",
                VenueName = "First venue",
                LayoutName = "First layout"},new ThirdPartyEvent {
                Id = 2,
                Name = "Second event",
                VenueName = "Second venue",
                LayoutName = "Second layout"} };
            // Act
            var gottenEvents = repository.GetAll();

            // Assert
            gottenEvents.Should().BeEquivalentTo(events, options => options.Excluding(t => t.StartDate).Excluding(t => t.EndDate));
        }

        public void Test()
        {
            var repository = new EventRepository(_filePath);
            var inner = Task.Factory.StartNew(() =>  // вложенная задача
            {
                repository.Create(new ThirdPartyEvent { Name = "New event", Description = "new desc" });
            });
        }

        private ThirdPartyEvent GetEventById(int id)
        {
            var eventsJson = File.ReadAllText(_filePath);
            var events = JsonConvert.DeserializeObject<List<ThirdPartyEvent>>(eventsJson);
            var item = events.FirstOrDefault(e => e.Id == id);
            return item;
        }
    }
}
