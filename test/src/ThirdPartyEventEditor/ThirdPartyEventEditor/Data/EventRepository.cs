using System.Collections.Generic;
using System.Linq;
using ThirdPartyEventEditor.Models;
using Newtonsoft.Json;
using System.IO;

namespace ThirdPartyEventEditor.Data
{
    /// <summary>
    /// Provides access to event entity.
    /// </summary>
    public class EventRepository : IRepository<ThirdPartyEvent>
    {
        private readonly string _filePath;
        private static object locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="EventRepository"/> class.
        /// </summary>
        /// <param name="sourcePath">Source file path.</param>
        public EventRepository(string sourcePath)
        {
            _filePath = sourcePath; 
        }

        /// <inheritdoc />
        public int Create(ThirdPartyEvent entity)
        {
            lock (locker)
            {
                var eventsJson = File.ReadAllText(_filePath);
                var events = JsonConvert.DeserializeObject<List<ThirdPartyEvent>>(eventsJson);
                entity.Id += events.Any() ? events.Max(e => e.Id) + 1 : 1;
                events.Add(entity);
                eventsJson = JsonConvert.SerializeObject(events, Formatting.Indented);
                File.WriteAllText(_filePath, eventsJson);
            }
            return entity.Id;
        }

        /// <inheritdoc />
        public void Update(ThirdPartyEvent entity)
        {
            lock (locker)
            {
                var eventsJson = File.ReadAllText(_filePath);
                var events = JsonConvert.DeserializeObject<List<ThirdPartyEvent>>(eventsJson);
                var item = events.FirstOrDefault(e => e.Id == entity.Id);
                if(item != null)
                {
                    item.Name = entity.Name;
                    item.StartDate = entity.StartDate;
                    item.EndDate = entity.EndDate;
                    item.Description = entity.Description;
                    item.PosterImage = entity.PosterImage;
                    item.VenueName = entity.VenueName;
                    item.LayoutName = entity.LayoutName;
                    eventsJson = JsonConvert.SerializeObject(events, Formatting.Indented);
                    File.WriteAllText(_filePath, eventsJson);
                }
            }
        }

        /// <inheritdoc />
        public void Delete(ThirdPartyEvent entity)
        {
            lock (locker)
            {
                var eventsJson = File.ReadAllText(_filePath);
                var events = JsonConvert.DeserializeObject<List<ThirdPartyEvent>>(eventsJson);
                var eventToRemove = events.FirstOrDefault(e => e.Id == entity.Id);
                if (events.Remove(eventToRemove))
                {
                    eventsJson = JsonConvert.SerializeObject(events, Formatting.Indented);
                    File.WriteAllText(_filePath, eventsJson);
                }
            }
        }

        /// <inheritdoc />
        public ThirdPartyEvent GetById(int id)
        {
            var eventsJson = File.ReadAllText(_filePath);
            var events = JsonConvert.DeserializeObject<List<ThirdPartyEvent>>(eventsJson);
            var item = events.FirstOrDefault(e => e.Id == id);
            return item;
        }

        /// <inheritdoc />
        public List<ThirdPartyEvent> GetAll()
        {
            var eventsJson = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<ThirdPartyEvent>>(eventsJson) ?? new List<ThirdPartyEvent>();
        }
    }
}