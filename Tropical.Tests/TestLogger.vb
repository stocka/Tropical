﻿''' <summary>
''' An <see cref="Models.ILogger">ILogge</see> implementation
''' for storing and retrieving log entries.
''' </summary>
Public Class TestLogger
  Implements Models.ILogger

  ''' <summary>
  ''' A representation of a log entry that was provided to a
  ''' <see cref="TestLogger" />.
  ''' </summary>
  Public Class TestLogEntry

    ''' <summary>
    ''' Initializes a new instance of the <see cref="TestLogEntry"/> class.
    ''' </summary>
    ''' <param name="message">The log message.</param>
    ''' <param name="exception">The associated exception.</param>
    Public Sub New(message As String, exception As Exception)
      Me.Message = message
      Me.Exception = exception
    End Sub

    ''' <summary>
    ''' Gets or sets the UTC date/time the entry was logged.
    ''' </summary>
    ''' <value>
    ''' The UTC date/time the entry was logged.
    ''' </value>
    Public Property DateLoggedUtc As DateTime = DateTime.UtcNow

    ''' <summary>
    ''' Gets or sets the log message.
    ''' </summary>
    ''' <value>
    ''' The log message.
    ''' </value>
    Public Property Message As String

    ''' <summary>
    ''' Gets or sets the associated exception.
    ''' </summary>
    ''' <value>
    ''' The associated exception.
    ''' </value>
    Public Property Exception As Exception = Nothing

  End Class

#Region "Entry Retrieval"

  Private _allEntries As New List(Of TestLogEntry)
  Private _warningEntries As New List(Of TestLogEntry)
  Private _debugEntries As New List(Of TestLogEntry)
  Private _errorEntries As New List(Of TestLogEntry)
  Private _infoEntries As New List(Of TestLogEntry)

  Public ReadOnly Property AllEntries() As IEnumerable(Of TestLogEntry)
    Get
      Return _allEntries
    End Get
  End Property

  Public ReadOnly Property DebugEntries() As IEnumerable(Of TestLogEntry)
    Get
      Return _debugEntries
    End Get
  End Property

  Public ReadOnly Property InformationEntries() As IEnumerable(Of TestLogEntry)
    Get
      Return _infoEntries
    End Get
  End Property

  Public ReadOnly Property WarningEntries() As IEnumerable(Of TestLogEntry)
    Get
      Return _warningEntries
    End Get
  End Property

  Public ReadOnly Property ErrorEntries() As IEnumerable(Of TestLogEntry)
    Get
      Return _errorEntries
    End Get
  End Property

  ''' <summary>
  ''' Determines whether the log contains an error that exactly matches
  ''' the specified error message.
  ''' </summary>
  ''' <param name="errorMessage">The error message.</param>
  ''' <returns><c>true</c> if a matching error was found, <c>false</c> otherwise.</returns>
  Public Function ContainsExactError(errorMessage As String) As Boolean
    Return Me.ErrorEntries IsNot Nothing AndAlso Me.ErrorEntries.Any(Function(e) e.Message.Equals(errorMessage))
  End Function

  ''' <summary>
  ''' Determines whether the log contains an error that contains
  ''' the specified error message.
  ''' </summary>
  ''' <param name="partialErrorMessage">The partial error message.</param>
  ''' <returns><c>true</c> if a matching error was found, <c>false</c> otherwise.</returns>
  Public Function ContainsError(partialErrorMessage As String) As Boolean
    Return Me.ErrorEntries IsNot Nothing AndAlso Me.ErrorEntries.Any(Function(e) e.Message.Contains(partialErrorMessage))
  End Function

  ''' <summary>
  ''' Determines whether the log contains an warning that contains
  ''' the specified warning message.
  ''' </summary>
  ''' <param name="partialWarningMessage">The partial warning message.</param>
  ''' <returns><c>true</c> if a matching warning was found, <c>false</c> otherwise.</returns>
  Public Function ContainsWarning(partialWarningMessage As String) As Boolean
    Return Me.WarningEntries IsNot Nothing AndAlso Me.WarningEntries.Any(Function(e) e.Message.Contains(partialWarningMessage))
  End Function

#End Region

#Region "ILogger Implementation"

  ''' <summary>
  ''' Logs a debug message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub Debug(message As String, Optional exception As Exception = Nothing) Implements Models.ILogger.Debug

    Dim entry As New TestLogEntry(message, exception)
    _allEntries.Add(entry)
    _debugEntries.Add(entry)

  End Sub

  ''' <summary>
  ''' Logs an informational message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub Information(message As String, Optional exception As Exception = Nothing) Implements Models.ILogger.Information

    Dim entry As New TestLogEntry(message, exception)
    _allEntries.Add(entry)
    _infoEntries.Add(entry)

  End Sub

  ''' <summary>
  ''' Logs a warning message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub Warning(message As String, Optional exception As Exception = Nothing) Implements Models.ILogger.Warning

    Dim entry As New TestLogEntry(message, exception)
    _allEntries.Add(entry)
    _warningEntries.Add(entry)

  End Sub

  ''' <summary>
  ''' Logs an error message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Public Sub [Error](message As String, Optional exception As Exception = Nothing) Implements Models.ILogger.Error

    Dim entry As New TestLogEntry(message, exception)
    _allEntries.Add(entry)
    _errorEntries.Add(entry)

  End Sub

#End Region

End Class
