Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports Tropical.Models

Public Class InformationBoxViewModel
  Inherits BaseNotifyPropertyChanged
  Implements ILogger

#Region "Messages and Icons"

  Private _successMessage As String = "The operation was completed successfully."

  ''' <summary>
  ''' Gets or sets the message that will be displayed
  ''' when no warnings or errors were logged.
  ''' </summary>
  ''' <value>
  ''' The message that will be displayed
  ''' when no warnings or errors were logged.
  ''' </value>
  Public Property SuccessMessage() As String
    Get
      Return Me._successMessage
    End Get
    Set(value As String)

      If value <> Me._successMessage Then

        RaisePropertyChanging(Function() Me.SuccessMessage)
        RaisePropertyChanging(Function() Me.CurrentMessage)

        Me._successMessage = value

        RaisePropertyChanged(Function() Me.SuccessMessage)
        RaisePropertyChanged(Function() Me.CurrentMessage)

      End If

    End Set
  End Property

  Private _warningMessage As String = "The operation was successful, but issues were encountered."

  ''' <summary>
  ''' Gets or sets the warning message that will be displayed
  ''' when one or more warnings were logged.
  ''' </summary>
  ''' <value>
  ''' The warning message that will be displayed
  ''' when one or more warnings were logged.
  ''' </value>
  Public Property WarningMessage() As String
    Get
      Return Me._warningMessage
    End Get
    Set(value As String)

      If value <> Me._warningMessage Then

        RaisePropertyChanging(Function() Me.WarningMessage)
        RaisePropertyChanging(Function() Me.CurrentMessage)

        Me._warningMessage = value

        RaisePropertyChanged(Function() Me.WarningMessage)
        RaisePropertyChanged(Function() Me.CurrentMessage)

      End If

    End Set
  End Property

  Private _errorMessage As String = "The operation was unsuccessful."

  ''' <summary>
  ''' Gets or sets the error message that will be displayed
  ''' when one or more errors were logged.
  ''' </summary>
  ''' <value>
  ''' The error message that will be displayed
  ''' when one or more errors were logged.
  ''' </value>
  Public Property ErrorMessage() As String
    Get
      Return Me._errorMessage
    End Get
    Set(value As String)

      If value <> Me._errorMessage Then

        RaisePropertyChanging(Function() Me.ErrorMessage)
        RaisePropertyChanging(Function() Me.CurrentMessage)

        Me._errorMessage = value

        RaisePropertyChanged(Function() Me.ErrorMessage)
        RaisePropertyChanged(Function() Me.CurrentMessage)

      End If

    End Set
  End Property

  Private _hasErrors As Boolean = False

  ''' <summary>
  ''' Gets or sets a value indicating whether one or more errors
  ''' have been logged.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if one or more errors have been logged; 
  ''' otherwise, <c>false</c>.
  ''' </value>
  Public Property HasErrors() As Boolean
    Get
      Return Me._hasErrors
    End Get
    Protected Set(value As Boolean)

      If value <> _hasErrors Then

        RaisePropertyChanging(Function() HasErrors)
        RaisePropertyChanging(Function() CurrentMessage)
        RaisePropertyChanging(Function() CurrentIcon)

        Me._hasErrors = value

        RaisePropertyChanged(Function() HasErrors)
        RaisePropertyChanged(Function() CurrentMessage)
        RaisePropertyChanged(Function() CurrentIcon)

      End If

    End Set
  End Property

  Private _hasWarnings As Boolean = False

  ''' <summary>
  ''' Gets or sets a value indicating whether one or more warnings
  ''' have been logged.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if one or more warnings have been logged; 
  ''' otherwise, <c>false</c>.
  ''' </value>
  Public Property HasWarnings() As Boolean
    Get
      Return Me._hasWarnings
    End Get
    Protected Set(value As Boolean)

      If value <> Me._hasWarnings Then

        RaisePropertyChanging(Function() HasWarnings)
        RaisePropertyChanging(Function() CurrentMessage)
        RaisePropertyChanging(Function() CurrentIcon)

        Me._hasWarnings = value

        RaisePropertyChanged(Function() HasWarnings)
        RaisePropertyChanged(Function() CurrentMessage)
        RaisePropertyChanged(Function() CurrentIcon)

      End If

    End Set
  End Property

  ''' <summary>
  ''' Gets the current message to display, depending on if
  ''' any warnings and/or errors have been logged.
  ''' </summary>
  ''' <value>
  ''' The current message to display.
  ''' </value>
  Public ReadOnly Property CurrentMessage() As String
    Get

      If Me.HasErrors Then
        Return Me.ErrorMessage
      ElseIf Me.HasWarnings Then
        Return Me.WarningMessage
      Else
        Return Me.SuccessMessage
      End If

    End Get
  End Property

  ''' <summary>
  ''' Gets the current icon to display, depending on if
  ''' any warnings and/or errors have been logged.
  ''' </summary>
  ''' <value>
  ''' The current icon to display.
  ''' </value>
  Public ReadOnly Property CurrentIcon() As BitmapSource
    Get

      Dim curIcon As System.Drawing.Icon

      If Me.HasErrors Then
        curIcon = System.Drawing.SystemIcons.Error
      ElseIf Me.HasWarnings Then
        curIcon = System.Drawing.SystemIcons.Warning
      Else
        curIcon = System.Drawing.SystemIcons.Information
      End If

      Return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
        curIcon.Handle,
        Int32Rect.Empty,
        BitmapSizeOptions.FromEmptyOptions())

    End Get
  End Property

#End Region

#Region "Log Message Display"

  Private _logBuilder As New Text.StringBuilder()

  ''' <summary>
  ''' Gets the text to display in the messages pane.
  ''' </summary>
  ''' <value>
  ''' The text to display in the messages pane.
  ''' </value>
  Public ReadOnly Property LogMessageText() As String
    Get
      Return _logBuilder.ToString()
    End Get
  End Property

  ''' <summary>
  ''' Gets the visibility of the messages pane,
  ''' based on if there is any text to display.
  ''' </summary>
  ''' <value>
  ''' The visibility of the messages pane.
  ''' </value>
  Public ReadOnly Property LogMessageVisibility() As Visibility
    Get

      If Me._logBuilder.Length > 0 Then
        Return Visibility.Visible
      Else
        Return Visibility.Collapsed
      End If

    End Get
  End Property

  ''' <summary>
  ''' Appends the log message to the <see cref="LogMessageText" />
  ''' (without a trailing newline) and raises the appropriate
  ''' property changing/changed events.
  ''' </summary>
  ''' <param name="messageText">The message text.</param>
  Private Sub AppendLogMessage(messageText As String)

    RaisePropertyChanging(Function() Me.LogMessageText)
    RaisePropertyChanging(Function() Me.LogMessageVisibility)
    _logBuilder.Append(messageText)
    RaisePropertyChanged(Function() Me.LogMessageText)
    RaisePropertyChanged(Function() Me.LogMessageVisibility)

  End Sub

  ''' <summary>
  ''' Appends a log message and 
  ''' </summary>
  ''' <param name="prefixText">The prefix for the message,
  ''' such as &quot;Warning&quot; or &quot;Error&quot;.</param>
  ''' <param name="messageText">The message text.</param>
  ''' <param name="exception">The associated exception.</param>
  Private Sub AppendLogMessage(prefixText As String,
                               messageText As String,
                               exception As Exception)

    Dim messageBuilder As New Text.StringBuilder()

    If Not String.IsNullOrWhiteSpace(prefixText) Then
      messageBuilder.Append(prefixText)
      messageBuilder.Append(": ")
    End If

    messageBuilder.AppendLine(messageText)

#If DEBUG Then

    ' Add the exception, if we're debugging
    If exception IsNot Nothing AndAlso
      Not TypeOf exception Is System.ComponentModel.DataAnnotations.ValidationException Then

      messageBuilder.AppendLine("Exception:")
      AppendExceptionToMessage(exception, messageBuilder)

    End If

#End If

    AppendLogMessage(messageBuilder.ToString())

  End Sub

  ''' <summary>
  ''' Appends the contents of an exception, and any inner exceptions,
  ''' to the provided string buider.
  ''' </summary>
  ''' <param name="exception">The exception.</param>
  ''' <param name="messageBuilder">The message builder.</param>
  Private Sub AppendExceptionToMessage(exception As Exception,
                                       messageBuilder As Text.StringBuilder)

    ' Write out the name of the exception
    messageBuilder.AppendLine(exception.ToString())

    ' Write the message if specified
    If Not String.IsNullOrWhiteSpace(exception.Message) Then
      messageBuilder.AppendLine(exception.Message)
    End If

    ' Include a stack trace if we have one
    If Not String.IsNullOrWhiteSpace(exception.StackTrace) Then
      messageBuilder.AppendLine(exception.StackTrace)
    End If

    ' Include an inner exception if we have one
    If exception.InnerException IsNot Nothing Then
      messageBuilder.AppendLine("Inner exception:")
      Me.AppendExceptionToMessage(exception.InnerException, messageBuilder)
    End If

  End Sub

#End Region

#Region "ILogger Implementation"

  Public Sub Debug(message As String, Optional exception As Exception = Nothing) Implements ILogger.Debug

    ' Only log messages if we're debugging.
#If DEBUG Then
    Me.AppendLogMessage("Debug", message, exception)
#End If

  End Sub

  Public Sub [Error](message As String, Optional exception As Exception = Nothing) Implements ILogger.Error

    Me.AppendLogMessage("Error", message, exception)

    ' Indicate we have errors
    Me.HasErrors = True

  End Sub

  Public Sub Information(message As String, Optional exception As Exception = Nothing) Implements ILogger.Information

    ' Don't put any prefix in.
    Me.AppendLogMessage("", message, exception)

  End Sub

  Public Sub Warning(message As String, Optional exception As Exception = Nothing) Implements ILogger.Warning

    Me.AppendLogMessage("Warning", message, exception)

    ' Indicate we have warnings
    Me.HasWarnings = True

  End Sub

#End Region

End Class
