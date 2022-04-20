using FluentValidation.Results;
using System;
using System.Collections.Generic;
using ThirdPartyEventEditor.Models;
using ThirdPartyEventEditor.Data;
using FluentValidation;

namespace ThirdPartyEventEditor.Services
{
    /// <summary>
    /// Responsible for proxy all event repository calls with validation logic.
    /// </summary>
    public class EventService : IEventService
    {
        private readonly AbstractValidator<ThirdPartyEvent> _validator;
        private readonly IRepository<ThirdPartyEvent> _eventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventService"/> class.
        /// </summary>
        /// <param name="eventRepository">Event repository.</param>
        public EventService(IRepository<ThirdPartyEvent> eventRepository)
        {
            _eventRepository = eventRepository;
            _validator = new EventValidator(eventRepository);
        }

        /// <inheritdoc />
        public ValidationResult CreateEvent(ThirdPartyEvent eventEntity)
        {
            var result = TryValidate(eventEntity, "Create");
            if (result.IsValid)
            {
                eventEntity.Id = _eventRepository.Create(eventEntity);
            }

            return result;
        }

        /// <inheritdoc />
        public ValidationResult UpdateEvent(ThirdPartyEvent eventEntity)
        {
            var result = TryValidate(eventEntity, "Update");
            if (result.IsValid)
            {
                _eventRepository.Update(eventEntity);
            }

            return result;
        }

        /// <inheritdoc />
        public ValidationResult DeleteEvent(ThirdPartyEvent eventEntity)
        {
            var result = TryValidate(eventEntity, "Delete");
            if (result.IsValid)
            {
                _eventRepository.Delete(eventEntity);
            }

            return result;
        }

        /// <inheritdoc />
        public ThirdPartyEvent GetEvent(int eventEntityId)
        {
            return _eventRepository.GetById(eventEntityId);
        }

        /// <inheritdoc />
        public List<ThirdPartyEvent> GetEvents()
        {
            return _eventRepository.GetAll() ?? new List<ThirdPartyEvent>();
        }

        /// <inheritdoc />
        private ValidationResult TryValidate(ThirdPartyEvent entity, string ruleSetName)
        {
            ValidationResult result = null;
            try
            {
                result = _validator.Validate(entity, options => options.IncludeRuleSets(ruleSetName));
            }
            catch (Exception exception)
            {
                result = result ?? new ValidationResult(new List<ValidationFailure>());
                result.Errors.Add(new ValidationFailure("", exception.Message));
            }

            return result;
        }
    }
}