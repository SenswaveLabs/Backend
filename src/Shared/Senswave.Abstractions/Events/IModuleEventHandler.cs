using MediatR;

namespace Senswave.Abstractions.Events;

public interface IModuleEventHandler<T> : INotificationHandler<T> where T : INotification;