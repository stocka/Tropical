''' <summary>
''' An interface for logging events encountered in the process
''' of generating a stylesheet.
''' </summary>
Public Interface ILogger

  ''' <summary>
  ''' Logs a debug message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Sub Debug(message As String, Optional exception As Exception = Nothing)

  ''' <summary>
  ''' Logs an informational message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Sub Information(message As String, Optional exception As Exception = Nothing)

  ''' <summary>
  ''' Logs a warning message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Sub Warning(message As String, Optional exception As Exception = Nothing)

  ''' <summary>
  ''' Logs an error message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Sub [Error](message As String, Optional exception As Exception = Nothing)

End Interface
