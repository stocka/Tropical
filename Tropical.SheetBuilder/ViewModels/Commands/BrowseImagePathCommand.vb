Imports Tropical.Models

''' <summary>
''' A command for browsing to and updating the
''' <see cref="Sprite.ImagePath" /> of a sprite.
''' </summary>
Public Class BrowseImagePathCommand
  Inherits BrowseFilePathCommandBase

  ''' <summary>
  ''' Initializes a new instance of the <see cref="BrowseImagePathCommand"/> class.
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
  ''' Allows the user to select a new <see cref="Sprite.ImagePath" />.
  ''' </summary>
  ''' <param name="parameter">The sprite to change.</param>
  Public Overrides Sub Execute(parameter As Object)

    ' Try to cast this as a sprite, make sure it exists
    Dim sprite = TryCast(parameter, Sprite)

    If sprite IsNot Nothing Then
      MyBase.ExecuteImageBrowseBase(sprite, "Image", sprite.ImagePath)
    End If

  End Sub

End Class
