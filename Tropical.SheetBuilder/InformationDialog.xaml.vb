Public Class InformationDialog

  Protected ReadOnly InformationVM As New InformationBoxViewModel()

  ''' <summary>
  ''' Gets the logger for the dialog.
  ''' </summary>
  ''' <value>
  ''' The logger for the dialog.
  ''' </value>
  Public ReadOnly Property Logger() As Models.ILogger
    Get
      Return InformationVM
    End Get
  End Property

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
      Return Me.InformationVM.SuccessMessage
    End Get
    Set(value As String)
      Me.InformationVM.SuccessMessage = value
    End Set
  End Property

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
      Return Me.InformationVM.WarningMessage
    End Get
    Set(value As String)
      Me.InformationVM.WarningMessage = value
    End Set
  End Property

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
      Return Me.InformationVM.ErrorMessage
    End Get
    Set(value As String)
      Me.InformationVM.ErrorMessage = value
    End Set
  End Property

  ''' <summary>
  ''' Gets a value indicating whether this dialog
  ''' has errors or warnings to display.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the dialog has errors or warnings to display;
  ''' otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property HasErrorsOrWarnings() As Boolean
    Get
      Return Me.InformationVM.HasErrors Or Me.InformationVM.HasWarnings
    End Get
  End Property

  Private Sub InformationDialog_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

    ' Set our information box VM as our data context
    Me.DataContext = Me.InformationVM

  End Sub

  Private Sub btnOk_Click(sender As Object, e As RoutedEventArgs) Handles btnOk.Click
    MyBase.Close()
  End Sub

End Class
