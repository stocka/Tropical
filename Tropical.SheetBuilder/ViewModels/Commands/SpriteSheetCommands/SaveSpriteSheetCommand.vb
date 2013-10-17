Imports Tropical.Models

''' <summary>
''' A command for saving a sprite sheet.
''' </summary>
Public Class SaveSpriteSheetCommand
  Inherits CommandBase(Of SpriteSheetFileInformation)
  Implements ICommand

  Private ReadOnly _containingWindow As Window
  Private _lastPath As String

  ''' <summary>
  ''' Initializes a new instance of the <see cref="SaveSpriteSheetCommand"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="saved">The method to invoke after a sprite sheet
  ''' has been successfully saved. Information about the sprite sheet will be
  ''' passed to this method.</param>
  ''' <param name="containingWindow">The containing window to use
  ''' for any modal dialog boxes.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 saved As Action(Of SpriteSheetFileInformation),
                 containingWindow As Window)

    MyBase.New(service, canExecute, saved)
    Me._containingWindow = containingWindow

  End Sub

  ''' <summary>
  ''' Saves a sprite sheet.
  ''' </summary>
  ''' <param name="parameter">The sprite sheet to save.</param>
  Public Overrides Sub Execute(parameter As Object)

    If CanExecute(parameter) Then

      ' Try to cast this as a sprite sheet, make sure it exists
      Dim spriteSheet = TryCast(parameter, SpriteSheet)

      If spriteSheet IsNot Nothing Then

        Dim saveFileDlg As New Microsoft.Win32.SaveFileDialog()
        With saveFileDlg
          .Title = "Save Sprite Sheet File"
          .Filter = "Sprite Sheet Files (*.sprite.xml)|*.sprite.xml|All Files (*.*)|*.*"
          .ValidateNames = True
          .CheckFileExists = False
        End With

        ' Use our last path if we had one
        If Not String.IsNullOrWhiteSpace(Me._lastPath) Then
          saveFileDlg.FileName = Me._lastPath
        End If

        ' Now show the dialog
        Dim showDialog As Boolean? = saveFileDlg.ShowDialog()
        Dim filePath As String = saveFileDlg.FileName

        If showDialog.GetValueOrDefault(False) = True AndAlso
          Not String.IsNullOrWhiteSpace(filePath) Then

          ' Set up a new information dialog
          Dim infoDialog As New InformationDialog()
          With infoDialog
            .Title = "Information"
            .ErrorMessage = "The sprite sheet could not be saved."
            .WarningMessage = "The sprite sheet was successfully saved, but issues were encountered."
            .SuccessMessage = "The sprite sheet was successfully saved."
          End With

          ' Now try to save the sheet, passing in the dialog's logger
          Dim success As Boolean =
            SpriteSheetService.SaveSpriteSheet(filePath, spriteSheet, infoDialog.Logger)

          ' Were we successful?
          If success Then

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
    End If
  End Sub

End Class
