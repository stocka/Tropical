Imports Tropical.Models

''' <summary>
''' A command for loading a sprite sheet.
''' </summary>
Public Class LoadSpriteSheetCommand
  Inherits CommandBase(Of SpriteSheetFileInformation)
  Implements ICommand

  Private ReadOnly _containingWindow As Window
  Private _lastPath As String

  ''' <summary>
  ''' Initializes a new instance of the <see cref="LoadSpriteSheetCommand"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="loaded">The method to invoke after a sprite sheet
  ''' has been successfully loaded. Information about the new sprite sheet will be
  ''' passed to this method.</param>
  ''' <param name="containingWindow">The containing window to use
  ''' for any modal dialog boxes.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 loaded As Action(Of SpriteSheetFileInformation),
                 containingWindow As Window)

    MyBase.New(service, canExecute, loaded)
    Me._containingWindow = containingWindow

  End Sub

  ''' <summary>
  ''' Loads a sprite sheet.
  ''' </summary>
  ''' <param name="parameter">A delegate to determine
  ''' if unsaved changes should be discarded. This should be
  ''' a <see cref="SpriteSheetViewModel.CheckUnsavedChangesHandler" />
  ''' value.</param>
  Public Overrides Sub Execute(parameter As Object)

    If CanExecute(parameter) Then

      ' Prompt if we have unsaved changes, and exit out if so desired
      Dim unsavedChangesHandler As Func(Of Boolean) =
        TryCast(parameter, Func(Of Boolean))

      If unsavedChangesHandler IsNot Nothing AndAlso Not unsavedChangesHandler.Invoke() Then
        Return
      End If

      Dim openFileDlg As New Microsoft.Win32.OpenFileDialog()
      With openFileDlg
        .Title = "Select Sprite Sheet File"
        .Filter = "Sprite Sheet Files (*.sprite.xml)|*.sprite.xml|All Files (*.*)|*.*"
        .CheckFileExists = True
        .ValidateNames = True
      End With

      ' Use our last path if we had one
      If Not String.IsNullOrWhiteSpace(Me._lastPath) Then
        openFileDlg.FileName = Me._lastPath
      End If

      ' Now show the dialog
      Dim showDialog As Boolean? = openFileDlg.ShowDialog()
      Dim filePath As String = openFileDlg.FileName

      If showDialog.GetValueOrDefault(False) = True AndAlso
        Not String.IsNullOrWhiteSpace(filePath) Then

        ' Set up a new information dialog
        Dim infoDialog As New InformationDialog()
        With infoDialog
          .Title = "Information"
          .ErrorMessage = "The sprite sheet could not be loaded."
          .WarningMessage = "The sprite sheet was successfully loaded, but issues were encountered."
          .SuccessMessage = "The sprite sheet was successfully loaded."
        End With

        ' Now try to load the sheet, passing in
        ' the dialog's logger
        Dim spriteSheet As SpriteSheet =
          SpriteSheetService.LoadSpriteSheet(filePath, infoDialog.Logger)

        ' Were we successful?
        If spriteSheet IsNot Nothing Then

          ' Try to execute our handler.
          MyBase.TryExecuteHandler(New SpriteSheetFileInformation(spriteSheet, filePath))

          ' Also save our last successful path
          Me._lastPath = filePath

        End If

        ' Now show our information message dialog, if there's
        ' anything worth showing.
        If infoDialog.HasErrorsOrWarnings Then
          infoDialog.Owner = _containingWindow
          infoDialog.ShowDialog()
        End If

      End If
    End If

  End Sub

End Class
