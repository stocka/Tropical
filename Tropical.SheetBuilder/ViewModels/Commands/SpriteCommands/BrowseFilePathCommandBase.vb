Imports Tropical.Models

''' <summary>
''' A base class for changing the file path of a sprite's image
''' (either the <see cref="Sprite.ImagePath" /> or
''' <see cref="Sprite.HoverImagePath" />).
''' </summary>
Public MustInherit Class BrowseFilePathCommandBase
  Inherits SpriteCommandBase

  ''' <summary>
  ''' Initializes a new instance of the <see cref="BrowseFilePathCommandBase"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="changed">The method to invoke after a sprite's
  ''' image has been successfully changed. The affected sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 changed As Action(Of Sprite))
    MyBase.New(service, canExecute, changed)
  End Sub

  ''' <summary>
  ''' Displays a dialog for browsing to an image file
  ''' and makes changes if the user selects one.
  ''' </summary>
  ''' <param name="sprite">The sprite to update.</param>
  ''' <param name="imageDescription">The description of the
  ''' image that is being browsed.</param>
  ''' <param name="imageValue">A reference to the value 
  ''' of the image to update.</param>
  Protected Sub ExecuteImageBrowseBase(sprite As Sprite,
                                       imageDescription As String,
                                       ByRef imageValue As String)

    If CanExecute(sprite) Then

      Dim openFileDlg As New Microsoft.Win32.OpenFileDialog()
      With openFileDlg
        .Title = "Select " & imageDescription
        .Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|All Files (*.*)|*.*"
        .CheckFileExists = True
        .ValidateNames = True

        ' Use our existing image value as the default path
        If Not String.IsNullOrWhiteSpace(imageValue) Then
          .FileName = imageValue
        End If
      End With

      ' Now show the dialog
      Dim showDialog As Boolean? = openFileDlg.ShowDialog()

      If showDialog.GetValueOrDefault(False) = True AndAlso
        Not String.IsNullOrWhiteSpace(openFileDlg.FileName) Then

        ' We got something back, but make sure it changed
        If Not String.Equals(openFileDlg.FileName, imageValue, StringComparison.InvariantCultureIgnoreCase) Then

          imageValue = openFileDlg.FileName

          ' Invoke the handler if we have one.
          MyBase.TryExecuteHandler(sprite)

        End If
      End If
    End If
  End Sub

End Class
