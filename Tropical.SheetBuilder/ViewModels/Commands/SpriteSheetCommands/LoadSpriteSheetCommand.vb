Imports Tropical.Models

''' <summary>
''' A command for loading a sprite sheet.
''' </summary>
Public Class LoadSpriteSheetCommand
  Inherits CommandBase(Of SpriteSheet)
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
  ''' has been successfully loaded. The new sprite sheet will be
  ''' passed to this method.</param>
  ''' <param name="containingWindow">The containing window to use
  ''' for any modal dialog boxes.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 loaded As Action(Of SpriteSheet),
                 containingWindow As Window)

    MyBase.New(service, canExecute, loaded)
    Me._containingWindow = containingWindow

  End Sub

  ''' <summary>
  ''' Loads a sprite sheet.
  ''' </summary>
  ''' <param name="parameter">This parameter is ignored.</param>
  Public Overrides Sub Execute(parameter As Object)

    If CanExecute(parameter) Then

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

      If showDialog.GetValueOrDefault(False) = True AndAlso
        Not String.IsNullOrWhiteSpace(openFileDlg.FileName) Then

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
          SpriteSheetService.LoadSpriteSheet(openFileDlg.FileName, infoDialog.Logger)

        ' Were we successful?
        If spriteSheet IsNot Nothing Then

          ' Try to execute our handler.
          MyBase.TryExecuteHandler(spriteSheet)

          ' Also save our last successful path
          Me._lastPath = openFileDlg.FileName

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
