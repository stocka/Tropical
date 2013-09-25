''' <summary>
''' An <see cref="Models.ILogger" /> implementation that logs messages
''' to <see cref="Console.Out">standard output</see> (and in the case of
''' error messages, <see cref="Console.[Error]">standard error</see>).
''' </summary>
Public Class CommandLineLogger
  Implements Models.ILogger

  ''' <summary>
  ''' Describes the different available logging levels.
  ''' </summary>
  Public Enum LogLevel
    None
    [Error]
    Warning
    Information
    Debug
  End Enum

  ''' <summary>
  ''' Gets the logging level.
  ''' </summary>
  ''' <value>
  ''' The logging level.
  ''' </value>
  Public Property Level() As LogLevel = LogLevel.Information

  ''' <summary>
  ''' Logs a debug message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub Debug(message As String, Optional exception As Exception = Nothing) Implements Models.ILogger.Debug

    If Me.Level >= LogLevel.Debug Then
      WriteMessage(Console.Out, "Debug", message, exception)
    End If

  End Sub

  ''' <summary>
  ''' Logs an error message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub [Error](message As String, Optional exception As Exception = Nothing) Implements Models.ILogger.Error

    If Me.Level >= LogLevel.Error Then
      WriteMessage(Console.Error, "Error", message, exception)
    End If

  End Sub

  ''' <summary>
  ''' Logs an informational message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub Information(message As String, Optional exception As Exception = Nothing) Implements Models.ILogger.Information

    If Me.Level >= LogLevel.Information Then
      WriteMessage(Console.Out, Nothing, message, exception)
    End If

  End Sub

  ''' <summary>
  ''' Logs a warning message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub Warning(message As String, Optional exception As Exception = Nothing) Implements Models.ILogger.Warning

    If Me.Level >= LogLevel.Warning Then
      WriteMessage(Console.Out, "Warning", message, exception)
    End If

  End Sub

  ''' <summary>
  ''' Writes a log message to the target text writer.
  ''' </summary>
  ''' <param name="targetWriter">The target text writer.</param>
  ''' <param name="prefix">The prefix to include with the message. Optional.</param>
  ''' <param name="message">The message.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Private Sub WriteMessage(targetWriter As IO.TextWriter, prefix As String, message As String, exception As Exception)

    ' Include our prefix, if necessary
    If Not String.IsNullOrWhiteSpace(prefix) Then
      targetWriter.WriteLine("{0}: {1}", prefix, message)
    Else
      targetWriter.WriteLine(message)
    End If

    ' Write the exception out, if we have one
    If exception IsNot Nothing Then
      WriteException(targetWriter, exception)
    End If

  End Sub

  ''' <summary>
  ''' Writes an exception to the target text writer.
  ''' </summary>
  ''' <param name="targetWriter">The target text writer.</param>
  ''' <param name="exception">The exception.</param>
  Private Sub WriteException(targetWriter As IO.TextWriter, exception As Exception)

    ' Write out the name of the exception
    targetWriter.WriteLine("{0}", exception.ToString())

    ' Write the message if specified
    If Not String.IsNullOrWhiteSpace(exception.Message) Then
      targetWriter.WriteLine(exception.Message)
    End If

    ' Include a stack trace if we have one
    If Not String.IsNullOrWhiteSpace(exception.StackTrace) Then
      targetWriter.WriteLine(exception.StackTrace)
    End If

    ' Include an inner exception if we have one
    If exception.InnerException IsNot Nothing Then
      Console.WriteLine("Inner exception:")
      WriteException(targetWriter, exception.InnerException)
    End If

  End Sub

End Class
