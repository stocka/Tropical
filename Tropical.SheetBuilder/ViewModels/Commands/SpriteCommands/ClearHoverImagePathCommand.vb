Imports Tropical.Models

''' <summary>
''' A command for clearing the <see cref="Sprite.HoverImagePath" /> of a sprite.
''' </summary>
Public Class ClearHoverImagePathCommand
  Inherits ClearFilePathCommandBase

  ''' <summary>
  ''' Initializes a new instance of the <see cref="ClearHoverImagePathCommand"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="changed">The method to invoke after a sprite's
  ''' hover image has been successfully cleared. The affected sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 changed As Action(Of Sprite))
    MyBase.New(service, canExecute, changed)
  End Sub

  ''' <summary>
  ''' Clears the sprite's <see cref="Sprite.HoverImagePath" />.
  ''' </summary>
  ''' <param name="parameter">The sprite to change.</param>
  Public Overrides Sub Execute(parameter As Object)

    ' Try to cast this as a sprite, make sure it exists
    Dim sprite = TryCast(parameter, Sprite)

    If sprite IsNot Nothing Then
      MyBase.ExecuteClearPathBase(sprite, sprite.HoverImagePath)
    End If

  End Sub

End Class
