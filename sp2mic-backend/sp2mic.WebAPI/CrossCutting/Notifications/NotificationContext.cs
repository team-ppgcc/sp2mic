﻿using FluentValidation.Results;

namespace sp2mic.WebAPI.CrossCutting.Notifications;

public class NotificationContext
{
  private readonly List<Notification> _notifications;
  public IReadOnlyCollection<Notification> Notifications => _notifications;
  public bool HasNotifications => _notifications.Any();

  public NotificationContext () { _notifications = new List<Notification>(); }

  private void AddNotification (string key, string message)
  {
    _notifications.Add(new Notification(key, message));
  }

  public void AddNotification (Notification notification) { _notifications.Add(notification); }

  public void AddNotifications (IList<Notification> notifications)
  {
    _notifications.AddRange(notifications);
  }

  public void AddNotifications (ValidationResult validationResult)
  {
    foreach (var error in validationResult.Errors)
    {
      AddNotification(error.ErrorCode, error.ErrorMessage);
    }
  }
}
