''' <summary>
''' An empty <see cref="Models.ILogger" /> implementation
''' that will discard any messages that have been logged to it.
''' </summary>
Public Class BlackholeLogger
  Implements ILogger

  ''' <summary>
  ''' Logs a debug message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub Debug(message As String, Optional exception As Exception = Nothing) Implements ILogger.Debug
  End Sub

  ''' <summary>
  ''' Logs an error message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub [Error](message As String, Optional exception As Exception = Nothing) Implements ILogger.Error
  End Sub

  ''' <summary>
  ''' Logs an informational message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub Information(message As String, Optional exception As Exception = Nothing) Implements ILogger.Information
  End Sub

  ''' <summary>
  ''' Logs a warning message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub Warning(message As String, Optional exception As Exception = Nothing) Implements ILogger.Warning
  End Sub

End Class
