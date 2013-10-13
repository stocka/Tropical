Imports Tropical.Models
Imports System.Windows.Forms

''' <summary>
''' A command for saving the contents of a sprite
''' sheet to its equivalent CSS, sprite image,
''' and sample HTML files.
''' </summary>
Public Class SaveSpriteSheetContentsCommand
  Inherits CommandBase(Of SpriteSheet)
  Implements ICommand

  Private ReadOnly _containingWindow As Window
  Private _lastPath As String

  ''' <summary>
  ''' Initializes a new instance of the <see cref="SaveSpriteSheetContentsCommand"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="saved">The method to invoke after a sprite sheet's
  ''' contents have been successfully saved. The sprite sheet will be
  ''' passed to this method.</param>
  ''' <param name="containingWindow">The containing window to use
  ''' for any modal dialog boxes.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 saved As Action(Of SpriteSheet),
                 containingWindow As Window)

    MyBase.New(service, canExecute, saved)
    Me._containingWindow = containingWindow

  End Sub

  ''' <summary>
  ''' Saves the contents of a sprite sheet.
  ''' </summary>
  ''' <param name="parameter">The sprite sheet to use.</param>
  Public Overrides Sub Execute(parameter As Object)

    If CanExecute(parameter) Then

      ' Try to cast this as a sprite sheet, make sure it exists
      Dim spriteSheet = TryCast(parameter, SpriteSheet)

      If spriteSheet IsNot Nothing Then

        ' This is gross, but WPF doesn't provide an equivalent
        ' folder browsing dialog. Ugh.
        Dim folderBrowseDialog As New FolderBrowserDialog()

        With folderBrowseDialog
          .Description = "Select Destination Folder"
          .ShowNewFolderButton = True
        End With

        ' Use our last path if we had one
        If Not String.IsNullOrWhiteSpace(Me._lastPath) Then
          folderBrowseDialog.SelectedPath = Me._lastPath
        End If

        ' More WinForms grossness. *sigh*
        Dim dlgResult As DialogResult =
          folderBrowseDialog.ShowDialog(New WpfWindowAsFormsWindow(_containingWindow))

        If dlgResult = DialogResult.OK AndAlso
          Not String.IsNullOrWhiteSpace(folderBrowseDialog.SelectedPath) Then

          ' Set up a new information dialog
          Dim infoDialog As New InformationDialog()
          With infoDialog
            .Title = "Information"
            .ErrorMessage = "The sprite sheet could not be created."
            .WarningMessage = "The sprite sheet was successfully created, but issues were encountered."
            .SuccessMessage = "The sprite sheet was successfully created."
          End With

          ' Now try to save the sheet, passing in the dialog's logger
          Dim success As Boolean =
            SpriteSheetService.SaveSpriteSheetContent(
              folderBrowseDialog.SelectedPath,
              spriteSheet,
              infoDialog.Logger)

          ' Were we successful?
          If success Then

            ' Try to execute our handler.
            MyBase.TryExecuteHandler(spriteSheet)

            ' Also save our last successful path
            Me._lastPath = folderBrowseDialog.SelectedPath

          End If

          ' Now show our information message dialog
          infoDialog.Owner = _containingWindow
          infoDialog.ShowDialog()

        End If
      End If
    End If
  End Sub

  ''' <summary>
  ''' A class for representing a WPF window as a Windows Forms window.
  ''' </summary>
  Private Class WpfWindowAsFormsWindow
    Implements System.Windows.Forms.IWin32Window

    Private ReadOnly _handle As System.IntPtr

    ''' <summary>
    ''' Initializes a new instance of the <see cref="WpfWindowAsFormsWindow"/> class.
    ''' </summary>
    ''' <param name="window">The WPF window.</param>
    Public Sub New(window As Window)
      Dim source = TryCast(System.Windows.PresentationSource.FromVisual(window), 
        System.Windows.Interop.HwndSource)

      Me._handle = source.Handle
    End Sub

    ''' <summary>
    ''' Gets the handle.
    ''' </summary>
    ''' <value>
    ''' The handle.
    ''' </value>
    Private ReadOnly Property Handle() As System.IntPtr Implements IWin32Window.Handle
      Get
        Return _handle
      End Get
    End Property

  End Class

End Class
