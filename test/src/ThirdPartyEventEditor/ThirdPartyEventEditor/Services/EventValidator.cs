using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThirdPartyEventEditor.Data;
using ThirdPartyEventEditor.Models;
using FluentValidation;

namespace ThirdPartyEventEditor.Services
{
    /// <summary>
    /// Validator for the event with different rule sets.
    /// </summary>
    public class EventValidator : AbstractValidator<ThirdPartyEvent>
    {
        private readonly IRepository<ThirdPartyEvent> _eventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventValidator"/> class.
        /// </summary>
        /// <param name="eventRepository">Event repository.</param>
        public EventValidator(IRepository<ThirdPartyEvent> eventRepository)
        {
            _eventRepository = eventRepository;
            RuleSet("Create", () =>
            {
                RuleFor(e => e)
                    .Must(e => IsNotExistDate(e))
                    .WithMessage("Can not create event for the same venue in the same time.");
                RuleFor(e => e)
                    .Must(e => e.StartDate < e.EndDate)
                    .WithMessage("Event end can not be before event start.");
                RuleFor(e => e)
                    .Must(e => e.StartDate > DateTime.UtcNow && e.EndDate > DateTime.UtcNow)
                    .WithMessage("Can not create event in the past.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(e => e.Id)
                    .Must(eventId => IsExistEvent(eventId))
                    .WithMessage("There is no event with this id.");
                RuleFor(e => e)
                    .Must(e => IsNotExistDate(e))
                    .WithMessage("Can not update event for the same venue in the same time.");
                RuleFor(e => e)
                    .Must(e => e.StartDate < e.EndDate)
                    .WithMessage("Event end can not be before event start.");
                RuleFor(e => e)
                    .Must(e => e.StartDate > DateTime.UtcNow && e.EndDate > DateTime.UtcNow)
                    .WithMessage("Can not update event in the past.");
            });

            RuleSet("Delete", () =>
            {
                RuleFor(e => e.Id)
                    .Must(eventId => IsExistEvent(eventId))
                    .WithMessage("There is no event with this id.");
            });
        }

        /// <summary>
        /// Сhecks if there is no event with the same time and date in venue.
        /// </summary>
        /// <param name="eventEntity">Event entity.</param>
        /// <returns>True if not exist otherwise false.</returns>
        private bool IsNotExistDate(ThirdPartyEvent eventEntity)
        {
            var events = _eventRepository.GetAll().Where(e => e.VenueName == eventEntity.VenueName 
                && e.LayoutName == eventEntity.LayoutName && e.Id != eventEntity.Id);
            return !events.Any() || events.All(e => e.StartDate > eventEntity.EndDate || eventEntity.StartDate > e.EndDate);
        }

        /// <summary>
        /// Checks if the event  exists.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <returns>True if exists otherwise false.</returns>
        private bool IsExistEvent(int eventId)
        {
            return _eventRepository.GetById(eventId) != null;
        }
    }
}